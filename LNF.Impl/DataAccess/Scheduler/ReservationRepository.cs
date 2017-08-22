using LNF.Cache;
using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;
using LNF.Scheduler;
using NHibernate;
using NHibernate.Context;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.DataAccess.Scheduler
{
    public class ReservationRepository<TContext> : Repository<TContext, Reservation>, IReservationRepository where TContext : ICurrentSessionContext
    {
        private IRepository<Activity> Activity
        {
            get { return SchedulerRepositoryCollection<TContext>.Current.Activity; }
        }

        private IReservationHistoryRepository ReservationHistory
        {
            get { return SchedulerRepositoryCollection<TContext>.Current.ReservationHistory; }
        }

        private IRepository<Resource> Resource
        {
            get { return SchedulerRepositoryCollection<TContext>.Current.Resource; }
        }

        private IRepository<ReservationInvitee> ReservationInvitee
        {
            get { return SchedulerRepositoryCollection<TContext>.Current.ReservationInvitee; }
        }

        public Reservation GetRepairReservationInProgress(int resourceId)
        {
            // procReservationSelect @Action = 'SelectRepairReserv'

            //SELECT Rv.*
            //FROM dbo.Reservation Rv
            //INNER JOIN dbo.Activity A ON Rv.ActivityID = A.ActivityID
            //WHERE A.Editable = 0
            //    AND ResourceID = @ResourceID
            //    AND ActualEndDateTime IS NULL
            //    AND EndDateTime > GETDATE()

            Reservation result = Query().FirstOrDefault(x =>
                !x.Activity.Editable
                && x.Resource.ResourceID == resourceId
                && x.ActualEndDateTime == null
                && x.EndDateTime > DateTime.Now);

            return result;
        }

        public Reservation InsertRepair(int resourceId, int clientId, DateTime beginDateTime, DateTime endDateTime, DateTime actualBeginDateTime, string notes, int? modifiedByClientId)
        {
            // procReservationInsert @Action = 'InsertRepair'

            //SELECT @ActivityID = ActivityID
            //FROM dbo.Activity
            //WHERE Editable = 0

            Activity repairActivity = Activity.Query().FirstOrDefault(x => !x.Editable);

            if (repairActivity == null)
                throw new InvalidOperationException("Cannot start a repair reservation because a repair activity is not defined (Editable = false).");

            /*
            INSERT INTO dbo.Reservation(ResourceID, ClientID, AccountID, ActivityID,
              BeginDateTime, EndDateTime, ActualBeginDateTime, ActualEndDateTime,
              ClientIDBegin, ClientIDEnd, CreatedOn, LastModifiedOn,
              Duration, Notes, ChargeMultiplier, RecurrenceID, ApplyLateChargePenalty, AutoEnd,
              HasProcessInfo, HasInvitees, IsActive, IsStarted, IsUnloaded, MaxReservedDuration, KeepAlive)
            VALUES
            (
                @ResourceID, @ClientID, sselData.dbo.udf_GetLabAccountID(), @ActivityID,
                @BeginDateTime, @EndDateTime
                , @ActualBeginDateTime
                , NULL      --ActualEndDateTime
                , @ClientID --ClientIDBegin
                , NULL      --ClientIDEnd
                , GETDATE() --CreatedOn
                , GETDATE() --LastModifiedOn
                , DATEDIFF(minute, @BeginDateTime, @EndDateTime)    --Duration
                , @Notes    --Notes
                , 1         --ChargeMultiplier
                , -1        --RecurrenceID
                , 0         --ApplyLateChargePenalty
                , 0         --AutoEnd
                , 0         --HasProcessInfo
                , 0         --HasInvitees
                , 1         --IsActive
                , 1         --IsStarted
                , 1         --IsUnloaded
                , DATEDIFF(minute, @BeginDateTime, @EndDateTime)    --MaxReservedDuration
                , 0         --KeepAlive
            )
            */

            double duration = (endDateTime - beginDateTime).TotalMinutes;

            Reservation result = new Reservation()
            {
                Resource = Session.Get<Resource>(resourceId),
                Client = Session.Get<Client>(clientId),
                Account = Properties.Current.LabAccount,
                Activity = repairActivity,
                BeginDateTime = beginDateTime,
                EndDateTime = endDateTime,
                ActualBeginDateTime = actualBeginDateTime,
                ActualEndDateTime = null,
                ClientIDBegin = clientId,
                ClientIDEnd = null,
                CreatedOn = DateTime.Now,
                LastModifiedOn = DateTime.Now,
                Duration = duration,
                Notes = notes,
                ChargeMultiplier = 1,
                RecurrenceID = -1,
                ApplyLateChargePenalty = false,
                AutoEnd = false,
                HasProcessInfo = false,
                HasInvitees = false,
                IsActive = true,
                IsStarted = true,
                IsUnloaded = true,
                MaxReservedDuration = duration,
                KeepAlive = false
            };

            Session.SaveOrUpdate(result);

            ReservationHistory.Insert("InsertRepair", "procReservationInsert", result, modifiedByClientId);

            return result;
        }

        public int DeleteByRecurrence(int recurrenceId, int? modifiedByClientId)
        {
            // procReservationDelete @Action = 'ByRecurrenceID'

            //UPDATE dbo.Reservation
            //SET IsActive = 0, LastModifiedOn = GETDATE()
            //WHERE RecurrenceID = @RecurrenceID AND BeginDateTime > GETDATE()

            IList<Reservation> query = Query().Where(x => x.RecurrenceID == recurrenceId && x.BeginDateTime > DateTime.Now).ToList();

            foreach (Reservation rsv in query)
            {
                rsv.IsActive = false;
                rsv.LastModifiedOn = DateTime.Now;

                Session.SaveOrUpdate(rsv);

                // also an entry into history is made for each reservation
                ReservationHistory.Insert("ByRecurrenceID", "procReservationDelete", rsv, modifiedByClientId);
            }

            return query.Count;
        }

        public int DeleteByGroup(int groupId, int? modifiedByClientId)
        {
            // procReservationDelete @Action = 'ByGroupID'

            //UPDATE dbo.Reservation
            //SET IsActive = 0
            //WHERE GroupID = @GroupID

            IList<Reservation> query = Query().Where(x => x.GroupID == groupId).ToList();

            foreach (Reservation rsv in query)
            {
                rsv.IsActive = false;

                // [2016-04-08 jg] This is not in the original sp but why would we not set this? Also it is set in DeleteByRecurrence so I'm adding it for consistency.
                rsv.LastModifiedOn = DateTime.Now;

                Session.SaveOrUpdate(rsv);

                // also an entry into history is made for each reservation
                ReservationHistory.Insert("ByGroupID", "procReservationDelete", rsv, modifiedByClientId);
            }

            return query.Count;
        }

        public TimeSpan GetTimeUntilNextReservation(int resourceId, int reservationId, int clientId, DateTime beginDateTime)
        {
            // procReservationSelect @Action = 'TimeTillNextReservation'

            //DECLARE @NextBeginTime datetime
            //DECLARE @TimeTillNext int
            //SET @NextBeginTime = NULL
            //SET @TimeTillNext = 9999999 -- arbitrarily large number

            ResourceModel res = CacheManager.Current.GetResource(resourceId);

            DateTime? nextBeginTime = null;
            double timeUntilNext = 9999999;

            //SELECT TOP 1 @NextBeginTime = Rv.BeginDateTime
            //FROM dbo.Reservation Rv
            //WHERE Rv.ResourceID = @ResourceID
            //    AND Rv.BeginDateTime >= @BeginDateTime
            //    AND Rv.IsActive = 1
            //    AND Rv.ReservationID <> @ReservationID
            //    AND Rv.ActualEndDateTime IS NULL
            //ORDER BY BeginDateTime

            nextBeginTime = Query()
                .Where(x =>
                    x.Resource.ResourceID == resourceId
                    && x.IsActive
                    && x.BeginDateTime >= beginDateTime
                    && x.ReservationID != reservationId 
                    && x.ActualEndDateTime == null)
                .OrderBy(x => x.BeginDateTime)
                .Select(x => x.BeginDateTime)
                .FirstOrDefault();

            //IF NOT @NextBeginTime IS NULL
            //    SET @TimeTillNext = DATEDIFF(minute, @BeginDateTime, @NextBeginTime)

            if (nextBeginTime != null)
                timeUntilNext = nextBeginTime.Value.Subtract(beginDateTime).TotalMinutes;

            //DECLARE @ReservableMinutes int
            //SET @ReservableMinutes = dbo.udf_SelectReservableMinutes (@ResourceID, @ClientID, GETDATE())

            double reservableMinutes = res.SelectReservableMinutes(clientId, DateTime.Now);

            //IF @ReservableMinutes < 0 
            //BEGIN
            //    SELECT TimeTillNextReservation = 0
            //    RETURN
            //END

            if (reservableMinutes <= 0)
                return TimeSpan.FromMinutes(0);

            //SET @ReservedMinutes = 0

            //SELECT @ReservedMinutes = Duration 
            //FROM dbo.Reservation Rv
            //WHERE Rv.ReservationID = @ReservationID

            double reservedMinutes = 0;

            if (reservationId > 0)
            {
                Reservation rsv = Single(reservationId);

                if (rsv != null)
                    reservedMinutes = rsv.Duration;
            }

            //-- A reservation can be increased in duration by any available time
            //SET @ReservableMinutes = @ReservableMinutes + @ReservedMinutes

            reservableMinutes = reservableMinutes + reservedMinutes;

            //-- return the min of reservable minutes or time till next
            //IF @ReservableMinutes > @TimeTillNext
            //    SELECT TimeTillNextReservation = @TimeTillNext
            //ELSE
            //BEGIN
            //    DECLARE @MaxReservTime integer
            //    SELECT @MaxReservTime = MaxReservTime
            //    FROM dbo.[Resource]
            //    WHERE ResourceID = @ResourceID

            //    IF @ReservableMinutes >= @MaxReservTime
            //        SELECT TimeTillNextReservation = @ReservableMinutes
            //    ELSE
            //        SELECT TimeTillNextReservation = -1 * @ReservableMinutes
            //END

            if (reservableMinutes > timeUntilNext)
                return TimeSpan.FromMinutes(timeUntilNext);
            else
            {
                double maxReservTime = res.MaxReservTime.TotalMinutes;
                if (reservableMinutes >= maxReservTime)
                    return TimeSpan.FromMinutes(reservableMinutes);
                else
                    return TimeSpan.FromMinutes(-1 * reservableMinutes);
            }
        }

        public IList<Reservation> SelectEndableReservations(int resourceId)
        {
            // procReservationSelect @Action = 'SelectEndableReserv'

            //SELECT Rv.*, R.ResourceName, A.Editable
            //FROM dbo.Reservation Rv, dbo.Activity A, dbo.[Resource] R
            //WHERE
            //    --A.Editable = 1 AND
            //    Rv.ActivityID = A.ActivityID
            //    AND Rv.ResourceID = @ResourceID
            //    AND Rv.IsActive = 1
            //    AND Rv.ActualBeginDateTime IS NOT NULL
            //    AND Rv.ActualEndDateTime IS NULL
            //    AND Rv.ResourceID = R.ResourceID
            //ORDER BY Rv.EndDateTime DESC

            IList<Reservation> result = Query()
                .Where(x =>
                    x.Resource.ResourceID == resourceId
                    && x.IsActive
                    && x.ActualBeginDateTime != null
                    && x.ActualEndDateTime == null)
                .OrderByDescending(x => x.EndDateTime)
                .ToList();

            return result;
        }

        public IList<Reservation> SelectByResource(int resourceId, DateTime startDate, DateTime endDate, bool includeDeleted)
        {
            if (includeDeleted)
                return Query().Where(x => x.Resource.ResourceID == resourceId &&
                    ((x.BeginDateTime < endDate && x.EndDateTime > startDate) || (x.ActualBeginDateTime < endDate && x.ActualEndDateTime > startDate))).ToList();
            else
                return Query().Where(x => x.IsActive && x.Resource.ResourceID == resourceId &&
                    ((x.BeginDateTime < endDate && x.EndDateTime > startDate) || (x.ActualBeginDateTime < endDate && x.ActualEndDateTime > startDate))).ToList();
        }

        public IList<Reservation> SelectByProcessTech(int processTechId, DateTime startDate, DateTime endDate, bool includeDeleted)
        {
            if (includeDeleted)
                return Query().Where(x => x.Resource.ProcessTech.ProcessTechID == processTechId && x.BeginDateTime < endDate && x.EndDateTime > startDate).ToList();
            else
                return Query().Where(x => x.Resource.ProcessTech.ProcessTechID == processTechId && x.BeginDateTime < endDate && x.EndDateTime > startDate && x.IsActive).ToList();
        }

        public IList<Reservation> SelectByClient(int clientId, DateTime startDate, DateTime endDate, bool includeDeleted)
        {
            List<Reservation> result = new List<Reservation>();

            if (includeDeleted)
            {
                result.AddRange(Query().Where(x => x.Client.ClientID == clientId && x.BeginDateTime < endDate && x.EndDateTime > startDate));
                result.AddRange(ReservationInvitee.Query().Where(x => x.Invitee.ClientID == clientId && x.Reservation.BeginDateTime < endDate && x.Reservation.EndDateTime > startDate).Select(x => x.Reservation));
            }
            else
            {
                result.AddRange(Query().Where(x => x.Client.ClientID == clientId && x.BeginDateTime < endDate && x.EndDateTime > startDate && x.IsActive));
                result.AddRange(ReservationInvitee.Query().Where(x => x.Invitee.ClientID == clientId && x.Reservation.BeginDateTime < endDate && x.Reservation.EndDateTime > startDate && x.Reservation.IsActive).Select(x => x.Reservation));
            }

            return result;
        }

        public IList<Reservation> SelectRecent(int resourceId)
        {
            // procReservationSelect @Action = 'SelectRecent'

            //SELECT TOP 6 Rv.ReservationID, Rv.BeginDateTime, Rv.EndDateTime,
            //  Rv.ClientID, sselData.dbo.udf_GetDisplayName(RV.ClientID) AS DisplayName
            //FROM dbo.Reservation Rv
            //WHERE Rv.ResourceID = @ResourceID
            //ORDER BY ABS (datediff (second, Rv.BeginDateTime, getdate())) ASC

            // Need to use criterion because of the complicated order by clause.

            var result = Session.CreateCriteria<Reservation>()
                .Add(Restrictions.Eq("Resource.ResourceID", resourceId))
                .AddOrder(Order.Asc(Projections.SqlFunction("abs", NHibernateUtil.Int32, DateProjections.DateDiff(DatePart.Second, Projections.Property<Reservation>(x => x.BeginDateTime), DateProjections.GetDate()))))
                .SetMaxResults(6)
                .List<Reservation>();

            return result;
        }
    }
}
