using LNF.CommonTools;
using LNF.Models.Data;
using LNF.Models.Scheduler;
using LNF.Models.Worker;
using LNF.PhysicalAccess;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;
using LNF.Scheduler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Impl.Scheduler
{
    public class ReservationManager : ManagerBase, IReservationManager
    {
        public ReservationManager(IProvider provider) : base(provider) { }

        public IEnumerable<IClientAccount> AvailableAccounts(IReservation rsv)
        {
            IList<IClientAccount> result = null;

            DateTime sd = rsv.CreatedOn.Date;
            DateTime ed = sd.AddDays(1);

            if (rsv.ActivityAccountType == ActivityAccountType.Reserver || rsv.ActivityAccountType == ActivityAccountType.Both)
            {
                //Load reserver's accounts
                result = Provider.Data.Client.ActiveClientAccounts(rsv.ClientID, sd, ed).ToList();
            }

            if (rsv.ActivityAccountType == ActivityAccountType.Invitee || rsv.ActivityAccountType == ActivityAccountType.Both)
            {
                //Loads each of the invitee's accounts
                foreach (var ri in GetInvitees(rsv.ReservationID))
                {
                    var temp = Provider.Data.Client.ActiveClientAccounts(ri.InviteeID, sd, ed);

                    if (result == null)
                        result = temp.ToList();
                    else
                    {
                        foreach (IClientAccount t in temp)
                        {
                            //Check if account already exists
                            if (!result.Any(i => i.AccountID == t.AccountID))
                            {
                                result.Add(t);
                            }
                        }
                    }
                }
            }

            return result;
        }

        public IReservation CreateReservation(int resourceId, int clientId, int accountId, int activityId, DateTime beginDateTime, DateTime endDateTime, double duration, string notes, bool autoEnd, bool hasProcessInfo, bool hasInvitees, int? recurrenceId, bool isActive, bool keepAlive, double maxReservedDuration, int? modifiedByClientId)
        {
            var rsv = Session.NamedQuery("CreateReservation").SetParameters(new
            {
                ResourceID = resourceId,
                ClientID = clientId,
                AccountID = accountId,
                ActivityID = activityId,
                BeginDateTime = beginDateTime,
                EndDateTime = endDateTime,
                Duration = duration,
                Notes = notes,
                AutoEnd = autoEnd,
                HasProcessInfo = hasProcessInfo,
                HasInvitees = hasInvitees,
                RecurrenceID = recurrenceId,
                IsActive = isActive,
                CreatedOn = DateTime.Now,
                KeepAlive = keepAlive,
                MaxReservedDuration = maxReservedDuration,
                ModifiedByClientID = modifiedByClientId
            }).List<Reservation>().FirstOrDefault();

            var result = rsv.CreateModel<IReservation>();

            return result;
        }

        public void CancelReservation(int reservationId, int? modifiedByClientId)
        {
            var r = GetExistingReservation(reservationId);

            // Cancel reservation
            r.IsActive = false;
            r.LastModifiedOn = DateTime.Now;
            r.CancelledDateTime = DateTime.Now;

            Session.SaveOrUpdate(r);

            // also add an entry into history
            InsertReservationHistory("ByReservationID", "procReservationDelete", r, modifiedByClientId);
        }

        public void CancelAndForgive(int reservationId, int? modifiedByClientId)
        {
            // This is all that happens in procReservationDelete @Action = 'WithForgive'

            //-- Set Reservation to inactive
            //UPDATE dbo.Reservation
            //SET IsActive = 0, LastModifiedOn = GETDATE(), CancelledDateTime = GETDATE(), ChargeMultiplier = 0
            //WHERE ReservationID = @ReservationID

            var r = GetExistingReservation(reservationId);

            r.IsActive = false;
            r.LastModifiedOn = DateTime.Now;
            r.CancelledDateTime = DateTime.Now;
            r.ChargeMultiplier = 0;

            //-- Delete Reservation ProcessInfos
            //DELETE FROM dbo.ReservationProcessInfo
            //WHERE ReservationID = @ReservationID

            Session.Delete(Session.Query<ReservationProcessInfo>().Where(x => x.Reservation.ReservationID == r.ReservationID));

            //-- Delete Reservation Invitees
            //DELETE FROM dbo.ReservationInvitee
            //WHERE ReservationID = @ReservationID

            Session.Delete(Session.Query<ReservationInvitee>().Where(x => x.Reservation.ReservationID == r.ReservationID));

            // also an entry into history is made
            InsertReservationHistory("WithForgive", "procReservationDelete", r, modifiedByClientId);
        }

        public int CancelByGroup(int groupId, int? modifiedByClientId)
        {
            // procReservationDelete @Action = 'ByGroupID'

            //UPDATE dbo.Reservation
            //SET IsActive = 0
            //WHERE GroupID = @GroupID

            var query = Session.Query<Reservation>().Where(x => x.GroupID == groupId).ToList();

            foreach (var rsv in query)
            {
                rsv.IsActive = false;
                rsv.LastModifiedOn = DateTime.Now;

                Session.SaveOrUpdate(rsv);

                // also an entry into history is made for each reservation
                InsertReservationHistory("ByGroupID", "procReservationDelete", rsv, modifiedByClientId);
            }

            return query.Count;
        }

        public int CancelByRecurrence(int recurrenceId, int? modifiedByClientId)
        {
            // procReservationDelete @Action = 'ByRecurrenceID'

            //UPDATE dbo.Reservation
            //SET IsActive = 0, LastModifiedOn = GETDATE()
            //WHERE RecurrenceID = @RecurrenceID AND BeginDateTime > GETDATE()

            var query = Session.Query<Reservation>().Where(x => x.RecurrenceID == recurrenceId && x.BeginDateTime > DateTime.Now).ToList();

            foreach (var rsv in query)
            {
                rsv.IsActive = false;
                rsv.LastModifiedOn = DateTime.Now;

                Session.SaveOrUpdate(rsv);

                // also an entry into history is made for each reservation
                InsertReservationHistory("ByRecurrenceID", "procReservationDelete", rsv, modifiedByClientId);
            }

            return query.Count;
        }

        /// <summary>
        /// Ends a reservation.
        /// </summary>
        public void EndReservation(int reservationId, int? endedByClientId, int? modifiedByClientId)
        {
            var r = GetExistingReservation(reservationId);

            r.ActualEndDateTime = DateTime.Now;
            r.ClientIDEnd = endedByClientId.GetValueOrDefault(-1);

            if (r.ActualBeginDateTime.Value > r.ActualEndDateTime.Value)
                throw new Exception(string.Format("ActualDateTime is greater than ActualEndDateTime [ReservationID: {0}, ResourceID: {1}, ActualBeginDateTime: {2:yyyy-MM-dd HH:mm:ss}, ActualEndDateTime: {3:yyyy-MM-dd HH:mm:ss}", r.ReservationID, r.Resource.ResourceID, r.ActualBeginDateTime, r.ActualEndDateTime));

            if (!r.Activity.Editable)
            {
                r.Resource.State = ResourceState.Online;
                Session.SaveOrUpdate(r.Resource);
            }

            Session.SaveOrUpdate(r);

            // also an entry into history is made
            InsertReservationHistory("End", "procReservationUpdate", r, modifiedByClientId);
        }

        public void EndForRepair(int reservationId, int? endedByClientId, int? modifiedByClientId)
        {
            // This is all that happens in procReservationUpdate @Action = 'EndForRepair'

            //UPDATE dbo.Reservation
            //SET ActualEndDateTime = GETDATE(),
            //    ChargeMultiplier = 0,
            //    ApplyLateChargePenalty = 0,
            //    ClientIDEnd = @ClientID
            //WHERE ReservationID = @ReservationID

            var r = GetExistingReservation(reservationId);

            r.ActualEndDateTime = DateTime.Now;
            r.ChargeMultiplier = 0;
            r.ApplyLateChargePenalty = false;
            r.ClientIDEnd = endedByClientId.GetValueOrDefault(-1);

            Session.SaveOrUpdate(r);

            // also an entry into history is made
            InsertReservationHistory("EndForRepair", "procReservationUpdate", r, modifiedByClientId);
        }

        /// <summary>
        /// Ends a past unstarted reservation.
        /// </summary>
        public int EndPastUnstarted(int reservationId, DateTime endDate, int? endedByClientId)
        {
            //ClientID might be -1
            return Session.NamedQuery("EndPastUnstartedReservations").SetParameters(new
            {
                ReservationID = reservationId,
                EndDateTime = endDate,
                ClientID = endedByClientId.GetValueOrDefault(-1)
            }).Result<int>();
        }

        public IEnumerable<ReservationHistoryFilterItem> FilterCancelledReservations(IEnumerable<IReservation> items, bool includeCanceledForModification)
        {
            if (items == null || items.Count() == 0)
                return new List<ReservationHistoryFilterItem>();

            int minReservationId = items.Min(x => x.ReservationID);

            IList<ReservationHistory> hist = Session.Query<ReservationHistory>().Where(x => x.LinkedReservationID.HasValue && x.LinkedReservationID.Value >= minReservationId && x.UserAction == ReservationHistoryItem.INSERT_FOR_MODIFICATION).ToList();
            IList<ReservationHistoryFilterItem> result = new List<ReservationHistoryFilterItem>();

            foreach (var i in items)
            {
                bool isCanceledForModification = hist.Any(x => x.LinkedReservationID.HasValue && x.LinkedReservationID.Value == i.ReservationID);

                if (!isCanceledForModification || includeCanceledForModification)
                {
                    result.Add(new ReservationHistoryFilterItem()
                    {
                        Reservation = i,
                        IsCanceledForModification = hist.Any(x => x.LinkedReservationID.HasValue && x.LinkedReservationID.Value == i.ReservationID)
                    });
                }
            }

            return result;
        }

        public IReservation FromDataRow(DataRow dr)
        {
            if (dr.Table.Columns.Contains("ReservationID") && int.TryParse(dr["ReservationID"].ToString(), out int id))
            {
                var result = Session.Single<Reservation>(id);
                if (result != null)
                    return result.CreateModel<IReservation>();
            }

            return null;
        }

        public IEnumerable<IResourceClient> GetResourceClients(int resourceId)
        {
            return Session.Query<ResourceClientInfo>().Where(x => x.ResourceID == resourceId).CreateModels<IResourceClient>();
        }

        public int GetAvailableSchedMin(int resourceId, int clientId)
        {
            string sql = "EXEC sselScheduler.dbo.procReservationSelect @Action = 'GetAvailableSchedMin', @ResourceID = :resourceId, @ClientID = :clientId";
            return Session.SqlQuery(sql).SetParameters(new { resourceId, clientId }).Result<int>();
        }

        public IEnumerable<IReservation> GetCurrentReservations()
        {
            return Session.Query<Reservation>()
                .Where(x => x.IsActive && x.IsStarted && x.ActualBeginDateTime != null && x.ActualEndDateTime == null)
                .OrderBy(x => x.ActualBeginDateTime)
                .CreateModels<IReservation>();
        }

        public IEnumerable<IReservationHistory> GetHistory(int reservationId)
        {
            return Session.Query<ReservationHistory>().Where(x => x.Reservation.ReservationID == reservationId).CreateModels<IReservationHistory>();
        }

        public IEnumerable<IReservationInvitee> GetInvitees(int reservationId)
        {
            return Session.Query<ReservationInviteeInfo>().Where(x => x.ReservationID == reservationId).CreateModels<IReservationInvitee>();
        }

        public IReservation InsertReservation(InsertReservationArgs args)
        {
            // procReservationInsert @Action = 'Insert'

            //INSERT INTO dbo.Reservation(ResourceID, ClientID, AccountID, ActivityID,
            //  BeginDateTime, EndDateTime, CreatedOn, LastModifiedOn,
            //  Duration, Notes, AutoEnd, ChargeMultiplier, RecurrenceID, ApplyLateChargePenalty, 
            //  HasProcessInfo, HasInvitees, IsActive, IsStarted, IsUnloaded, KeepAlive, MaxReservedDuration)
            //VALUES(@ResourceID, @ClientID, @AccountID, @ActivityID,
            //  @BeginDateTime, @EndDateTime, @CreatedOn, GETDATE(),
            //  @Duration, @Notes, @AutoEnd, 1.00, @RecurrenceID, 1,
            //  @HasProcessInfo, @HasInvitees, @IsActive, 0, 0, @KeepAlive, @MaxReservedDuration)

            var r = PrivateInsert(args);

            // also an entry into history is made
            InsertReservationHistory("Insert", "procReservationInsert", r, args.ModifiedByClientID);

            return r.CreateModel<IReservation>();
        }

        public IReservation InsertForModification(InsertReservationArgs args, IReservation linkedReservation)
        {
            // The idea here is that linkedReservation is an existing reservation that has been modified (all propeties already set), and
            // we are using it to create a new reservation for modification. The ReservationID of linkedReservation will be used to link the
            // two reservations in ReservationHistory and the newly created reservation will be returned.

            // procReservationInsert @Action = 'InsertForModification'

            //INSERT INTO dbo.Reservation(ResourceID, ClientID, AccountID, ActivityID,
            //  BeginDateTime, EndDateTime, CreatedOn, LastModifiedOn,
            //  Duration, Notes, AutoEnd, ChargeMultiplier, RecurrenceID, ApplyLateChargePenalty, 
            //  HasProcessInfo, HasInvitees, IsActive, IsStarted, IsUnloaded, KeepAlive, MaxReservedDuration, TotalProcessRuns)
            //VALUES(@ResourceID, @ClientID, @AccountID, @ActivityID,
            //  @BeginDateTime, @EndDateTime, @CreatedOn, GETDATE(),
            //  @Duration, @Notes, @AutoEnd, 1.00, @RecurrenceID, 1,
            //  @HasProcessInfo, @HasInvitees, @IsActive, 0, 0, @KeepAlive, @MaxReservedDuration, @TotalProcessRuns)

            var r = PrivateInsert(args);

            r.ChargeMultiplier = linkedReservation.ChargeMultiplier;
            r.OriginalBeginDateTime = linkedReservation.OriginalBeginDateTime.GetValueOrDefault(linkedReservation.BeginDateTime);
            r.OriginalEndDateTime = linkedReservation.OriginalEndDateTime.GetValueOrDefault(linkedReservation.EndDateTime);
            r.OriginalModifiedOn = linkedReservation.OriginalModifiedOn.GetValueOrDefault(linkedReservation.LastModifiedOn);

            Session.SaveOrUpdate(r);

            InsertReservationHistory("InsertForModification", "procReservationInsert", r, args.ModifiedByClientID, linkedReservation.ReservationID);

            return r.CreateModel<IReservation>();
        }

        public IReservation InsertFacilityDownTime(int resourceId, int clientId, int groupId, DateTime beginDateTime, DateTime endDateTime, string notes, int? modifiedByClientId)
        {
            // Important considerations:
            //      1. Activity is always the FacilityDownTime activity as defined in sselScheduler.dbo.Activity
            //      2. Account is always the LabAccount as defined by LNF.Scheduler.Properties.LabAccount
            //      3. ActualBeginDateTime and ActualEndDateTime are set to beginDateTime and endDateTime
            //      4. ClientIDBegin and ClientIDEnd are set to clientId, and IsStarted is set to true
            //      5. #3 and #4 means that FacilityDownTime reservations are already started and ended when created, regardless scheduled begin/end

            // This is all that happens in procReservationInsert @Action = 'InsertFacilityDownTime'

            //INSERT INTO dbo.Reservation(ResourceID, ClientID, AccountID, ActivityID,
            //  BeginDateTime, EndDateTime, ActualBeginDateTime, ActualEndDateTime, ClientIDBegin, ClientIDEnd, CreatedOn, LastModifiedOn,
            //  Duration, Notes, AutoEnd, ChargeMultiplier, RecurrenceID, GroupID, ApplyLateChargePenalty, 
            //  HasProcessInfo, HasInvitees, IsActive, IsStarted, IsUnloaded, MaxReservedDuration, KeepAlive)
            //VALUES(@ResourceID, @ClientID, @AccountID, @ActivityID,
            //  @BeginDateTime, @EndDateTime, @ActualBeginDateTime, @ActualEndDateTime, @ClientID, @ClientID, @CreatedOn, GETDATE(),
            //  @Duration, @Notes, @AutoEnd, 1.00, @RecurrenceID, @GroupID, 1,
            //  @HasProcessInfo, @HasInvitees, @IsActive, 0, 0, @Duration, 0)

            var act = Session.Single<Activity>(Properties.Current.Activities.FacilityDownTime.ActivityID);

            if (act == null)
                throw new InvalidOperationException("Cannot start a facility downtime reservation because a facility downtime activity is not defined.");

            double duration = (endDateTime - beginDateTime).TotalMinutes;

            int accountId = Properties.Current.LabAccount.AccountID;

            var r = new Reservation
            {
                Resource = Session.Single<Resource>(resourceId),
                Client = Session.Single<Client>(clientId),
                Account = Session.Single<Account>(accountId),
                Activity = act,
                BeginDateTime = beginDateTime,
                EndDateTime = endDateTime,
                // Facility down time must not need to be activated manually by person
                ActualBeginDateTime = beginDateTime,
                ActualEndDateTime = endDateTime,
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
                KeepAlive = false,
                HasProcessInfo = false,
                HasInvitees = false,
                IsActive = true,
                IsStarted = true,
                IsUnloaded = false,
                MaxReservedDuration = duration,
                CancelledDateTime = null,
                GroupID = groupId,
                OriginalBeginDateTime = null,
                OriginalEndDateTime = null,
                OriginalModifiedOn = null
            };

            Session.Insert(r);

            InsertReservationHistory("InsertFacilityDownTime", "procReservationInsert", r, modifiedByClientId);

            return r.CreateModel<IReservation>();
        }

        public IReservation InsertRepair(int resourceId, int clientId, DateTime beginDateTime, DateTime endDateTime, DateTime actualBeginDateTime, string notes, int? modifiedByClientId)
        {
            // procReservationInsert @Action = 'InsertRepair'

            //SELECT @ActivityID = ActivityID
            //FROM dbo.Activity
            //WHERE Editable = 0

            var repairActivity = Session.Single<Activity>(Properties.Current.Activities.Repair.ActivityID);

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

            int accountId = Properties.Current.LabAccount.AccountID;

            var r = new Reservation
            {
                Resource = Session.Single<Resource>(resourceId),
                Client = Session.Single<Client>(clientId),
                Account = Session.Single<Account>(accountId),
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
                KeepAlive = false,
                HasProcessInfo = false,
                HasInvitees = false,
                IsActive = true,
                IsStarted = true, // was false - fixed on 2017-08-21 [jg]
                IsUnloaded = false,
                MaxReservedDuration = duration,
                CancelledDateTime = null,
                GroupID = null,
                OriginalBeginDateTime = null,
                OriginalEndDateTime = null,
                OriginalModifiedOn = null
            };

            Session.Insert(r);

            InsertReservationHistory("InsertRepair", "procReservationInsert", r, modifiedByClientId);

            return r.CreateModel<IReservation>();
        }

        public void AppendNotes(int reservationId, string notes)
        {
            var rsv = Session.Single<Reservation>(reservationId);

            if (rsv != null)
                rsv.AppendNotes(notes);
        }

        public bool IsInvited(int reservationId, int clientId)
        {
            return Session.Query<ReservationInvitee>().Any(x => x.Reservation.ReservationID == reservationId && x.Invitee.ClientID == clientId);
        }

        public IEnumerable<IReservation> ReservationsInGranularityWindow(IResource res)
        {
            DateTime edate = DateTime.Now.AddMinutes(res.Granularity);
            var result = Session.Query<ReservationInfo>().Where(x => x.BeginDateTime >= DateTime.Now && x.BeginDateTime < edate && x.ResourceID == res.ResourceID).CreateModels<IReservation>();
            return result;
        }

        public IEnumerable<IReservation> SelectAutoEnd()
        {
            return Session.NamedQuery("SelectAutoEndReservations").List<ReservationInfo>().AsQueryable().CreateModels<IReservation>();
        }

        public IEnumerable<IReservation> SelectByClient(int clientId, DateTime startDate, DateTime endDate, bool includeDeleted)
        {
            List<IReservation> result = new List<IReservation>();

            if (includeDeleted)
            {
                result.AddRange(Session.Query<Reservation>().Where(x => x.Client.ClientID == clientId && x.BeginDateTime < endDate && x.EndDateTime > startDate).CreateModels<IReservation>());
                result.AddRange(Session.Query<ReservationInvitee>().Where(x => x.Invitee.ClientID == clientId && x.Reservation.BeginDateTime < endDate && x.Reservation.EndDateTime > startDate).Select(x => x.Reservation).CreateModels<IReservation>());
            }
            else
            {
                result.AddRange(Session.Query<Reservation>().Where(x => x.Client.ClientID == clientId && x.BeginDateTime < endDate && x.EndDateTime > startDate && x.IsActive).CreateModels<IReservation>());
                result.AddRange(Session.Query<ReservationInvitee>().Where(x => x.Invitee.ClientID == clientId && x.Reservation.BeginDateTime < endDate && x.Reservation.EndDateTime > startDate && x.Reservation.IsActive).Select(x => x.Reservation).CreateModels<IReservation>());
            }

            return result;
        }

        public IEnumerable<IReservation> SelectByDateRange(DateTime startDate, DateTime endDate, int clientId = 0)
        {
            var result = Session.Query<ReservationInfo>().Where(x =>
                ((x.BeginDateTime < endDate && x.EndDateTime > startDate) ||
                (x.ActualBeginDateTime < endDate && x.ActualEndDateTime > startDate)) &&
                x.ClientID == (clientId > 0 ? clientId : x.ClientID)
            ).CreateModels<IReservation>();

            return result;
        }

        public IEnumerable<IReservation> SelectByGroup(int groupId)
        {
            // procReservationSelect @Action = 'ByGroupID'

            //SELECT res.ResourceID, res.ResourceName
            //FROM Reservation r
            //INNER JOIN [Resource] res ON res.ResourceID = r.ResourceID
            //WHERE r.GroupID = @GroupID

            var result = Session.Query<ReservationInfo>().Where(x => x.GroupID == groupId).CreateModels<IReservation>();

            return result;
        }

        public IEnumerable<IReservation> SelectByProcessTech(int processTechId, DateTime startDate, DateTime endDate, bool includeDeleted)
        {
            if (includeDeleted)
                return Session.Query<ReservationInfo>().Where(x => x.ProcessTechID == processTechId && x.BeginDateTime < endDate && x.EndDateTime > startDate).CreateModels<IReservation>();
            else
                return Session.Query<ReservationInfo>().Where(x => x.ProcessTechID == processTechId && x.BeginDateTime < endDate && x.EndDateTime > startDate && x.IsActive).CreateModels<IReservation>();
        }

        public IEnumerable<IReservation> SelectByResource(int resourceId, DateTime startDate, DateTime endDate, bool includeDeleted)
        {
            if (includeDeleted)
                return Session.Query<ReservationInfo>().Where(x => x.ResourceID == resourceId &&
                    ((x.BeginDateTime < endDate && x.EndDateTime > startDate) || (x.ActualBeginDateTime < endDate && x.ActualEndDateTime > startDate))).CreateModels<IReservation>();
            else
                return Session.Query<ReservationInfo>().Where(x => x.IsActive && x.ResourceID == resourceId &&
                    ((x.BeginDateTime < endDate && x.EndDateTime > startDate) || (x.ActualBeginDateTime < endDate && x.ActualEndDateTime > startDate))).CreateModels<IReservation>();
        }

        public IEnumerable<IReservation> SelectEndableReservations(int resourceId)
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

            var result = Session.Query<ReservationInfo>()
                .Where(x =>
                    x.ResourceID == resourceId
                    && x.IsActive
                    && x.ActualBeginDateTime != null
                    && x.ActualEndDateTime == null)
                .OrderByDescending(x => x.EndDateTime)
                .CreateModels<IReservation>();

            return result;
        }

        public IEnumerable<IReservation> SelectExisting(int resourceId)
        {
            DateTime now = DateTime.Now;
            var reservationsWithFutureEndDateTime = Session.Query<ReservationInfo>().Where(x => x.EndDateTime > DateTime.Now && x.ActualEndDateTime == null && x.ResourceID == resourceId).CreateModels<IReservation>();
            var reservationsWithFutureEndDateTimeWhereChargeBeginDateTimeLessThanNow = reservationsWithFutureEndDateTime.Where(x => x.ChargeBeginDateTime < now).ToList();
            return reservationsWithFutureEndDateTimeWhereChargeBeginDateTimeLessThanNow;
        }

        public IEnumerable<IReservation> SelectHistory(int clientId, DateTime sd, DateTime ed)
        {
            var query = Session.Query<ReservationInfo>()
                .Where(x => x.ClientID == clientId && (x.BeginDateTime < ed) && (x.EndDateTime > sd));

            var result = query.CreateModels<IReservation>();

            return result;
        }

        public IEnumerable<IReservation> SelectHistoryToForgiveForRepair(int resourceId, DateTime sd, DateTime ed)
        {
            // [2013-05-20 jg] We no longer care if the reservation was canceled or not, all need to be forgiven
            //      because of the booking fee on uncancelled reservations.

            // [2018-08-29 jg] The only problem is when a repair is extened the email will be sent again to
            //      a reservation that was already forgiven for repair (when the repair was made originally)

            int repairActivityId = 14;

            var result = Session.Query<ReservationInfo>().Where(x =>
                x.ResourceID == resourceId
                && ((x.BeginDateTime < ed && x.EndDateTime > sd) || (x.ActualBeginDateTime < ed && x.ActualEndDateTime > sd)) //will include all overlapping reservations
                && x.ActivityID != repairActivityId).OrderBy(x => x.BeginDateTime).CreateModels<IReservation>();

            return result;
        }

        public DateTime? SelectLastRepairEndTime(int resourceId)
        {
            // This is all that happens in procReservationSelect @Action = 'SelectLastRepairReservEndTime'

            //SELECT TOP 1 Rv.ActualEndDateTime
            //FROM dbo.Reservation Rv, dbo.Activity A
            //WHERE Rv.ActivityID = A.ActivityID
            //    AND A.Editable = 0
            //    AND Rv.ResourceID = @ResourceID
            //    AND Rv.ActualEndDateTime < GETDATE()
            //ORDER BY Rv.ActualEndDateTime DESC

            var rsv = Session.Query<ReservationInfo>().Where(x =>
                !x.Editable
                && x.ResourceID == resourceId
                && (x.ActualEndDateTime != null && x.ActualEndDateTime < DateTime.Now)
            ).OrderByDescending(x => x.ActualEndDateTime).FirstOrDefault();

            if (rsv != null)
                return rsv.ActualEndDateTime;
            else
                return null;
        }

        public IEnumerable<IReservation> SelectOverwritable(int resourceId, DateTime sd, DateTime ed)
        {
            // procReservationSelect @Action = 'SelectOverwrittable'

            //SELECT Rv.*, R.ResourceName, R.IsSchedulable, A.Editable, sselData.dbo.udf_GetDisplayName(RV.ClientID) AS DisplayName
            //FROM dbo.Reservation Rv, dbo.[Resource] R, dbo.Activity A
            //WHERE Rv.ActivityID = A.ActivityID
            //    AND Rv.ResourceID = R.ResourceID
            //    AND R.ResourceID = @ResourceID
            //    AND BeginDateTime < @EndDateTime
            //    AND EndDateTime > @BeginDateTime
            //    AND Rv.IsActive = 1
            //    AND Rv.ActualEndDateTime IS NULL
            //ORDER BY BeginDateTime

            var result = Session.Query<ReservationInfo>().Where(x =>
                x.ResourceID == resourceId
                && x.BeginDateTime < ed
                && x.EndDateTime > sd
                && x.IsActive
                && x.ActualEndDateTime == null).CreateModels<IReservation>();

            return result;

        }

        public IEnumerable<IReservation> SelectPastEndableRepair()
        {
            var dt = DA.Command()
                .Param("Action", "SelectPastEndableRepair")
                .FillDataTable("sselScheduler.dbo.procReservationSelect");

            var result = new List<IReservation>();

            foreach (DataRow dr in dt.Rows)
            {
                result.Add(new ReservationItem
                {
                    ReservationID = dr.Field<int>("ReservationID"),
                    ClientID = dr.Field<int>("ClientID"),
                    UserName = dr.Field<string>("UserName"),
                    LName = dr.Field<string>("LName"),
                    MName = dr.Field<string>("MName"),
                    FName = dr.Field<string>("FName"),
                    Phone = dr.Field<string>("Phone"),
                    Email = dr.Field<string>("Email"),
                    Privs = dr.Field<ClientPrivilege>("Privs"),
                    AccountID = dr.Field<int>("AccountID"),
                    AccountName = dr.Field<string>("AccountName"),
                    ShortCode = dr.Field<string>("ShortCode"),
                    ChargeTypeID = dr.Field<int>("ChargeTypeID"),
                    ActivityID = dr.Field<int>("ActivityID"),
                    ActivityName = dr.Field<string>("ActivityName"),
                    ActivityAccountType = dr.Field<ActivityAccountType>("ActivityAccountType"),
                    StartEndAuth = dr.Field<ClientAuthLevel>("StartEndAuth"),
                    Editable = dr.Field<bool>("Editable"),
                    IsFacilityDownTime = dr.Field<bool>("IsFacilityDownTime"),
                    BeginDateTime = dr.Field<DateTime>("BeginDateTime"),
                    EndDateTime = dr.Field<DateTime>("EndDateTime"),
                    ActualBeginDateTime = dr.Field<DateTime?>("ActualBeginDateTime"),
                    ActualEndDateTime = dr.Field<DateTime?>("ActualEndDateTime"),
                    ClientIDBegin = dr.Field<int?>("ClientIDBegin"),
                    ClientBeginLName = dr.Field<string>("ClientBeginLName"),
                    ClientBeginFName = dr.Field<string>("ClientBeginFName"),
                    ClientIDEnd = dr.Field<int?>("ClientIDEnd"),
                    ClientEndLName = dr.Field<string>("ClientEndLName"),
                    ClientEndFName = dr.Field<string>("ClientEndFName"),
                    CreatedOn = dr.Field<DateTime>("CreatedOn"),
                    LastModifiedOn = dr.Field<DateTime>("LastModifiedOn"),
                    Duration = dr.Field<double>("Duration"),
                    Notes = dr.Field<string>("Notes"),
                    ChargeMultiplier = dr.Field<double>("ChargeMultiplier"),
                    ApplyLateChargePenalty = dr.Field<bool>("ApplyLateChargePenalty"),
                    AutoEnd = dr.Field<bool>("AutoEnd"),
                    HasProcessInfo = dr.Field<bool>("HasProcessInfo"),
                    HasInvitees = dr.Field<bool>("HasInvitees"),
                    IsActive = dr.Field<bool>("IsActive"),
                    IsStarted = dr.Field<bool>("IsStarted"),
                    IsUnloaded = dr.Field<bool>("IsUnloaded"),
                    RecurrenceID = dr.Field<int?>("RecurrenceID"),
                    GroupID = dr.Field<int?>("GroupID"),
                    MaxReservedDuration = dr.Field<double>("MaxReservedDuration"),
                    CancelledDateTime = dr.Field<DateTime?>("CancelledDateTime"),
                    KeepAlive = dr.Field<bool>("KeepAlive"),
                    OriginalBeginDateTime = dr.Field<DateTime?>("OriginalBeginDateTime"),
                    OriginalEndDateTime = dr.Field<DateTime?>("OriginalEndDateTime"),
                    OriginalModifiedOn = dr.Field<DateTime?>("OriginalModifiedOn"),
                    AuthDuration = dr.Field<int>("AuthDuration"),
                    AuthState = dr.Field<bool>("AuthState"),
                    BuildingDescription = dr.Field<string>("BuildingDescription"),
                    BuildingID = dr.Field<int>("BuildingID"),
                    BuildingIsActive = dr.Field<bool>("BuildingIsActive"),
                    BuildingName = dr.Field<string>("BuildingName"),
                    GracePeriod = dr.Field<int>("GracePeriod"),
                    Granularity = dr.Field<int>("Granularity"),
                    HelpdeskEmail = dr.Field<string>("HelpdeskEmail"),
                    IsReady = dr.Field<bool>("IsReady"),
                    IsSchedulable = dr.Field<bool>("IsSchedulable"),
                    LabDescription = dr.Field<string>("LabDescription"),
                    LabDisplayName = dr.Field<string>("LabDisplayName"),
                    LabID = dr.Field<int>("LabID"),
                    LabIsActive = dr.Field<bool>("LabIsActive"),
                    LabName = dr.Field<string>("LabName"),
                    MaxAlloc = dr.Field<int>("MaxAlloc"),
                    MaxReservTime = dr.Field<int>("MaxReservTime"),
                    MinCancelTime = dr.Field<int>("MinCancelTime"),
                    MinReservTime = dr.Field<int>("MinReservTime"),
                    Offset = dr.Field<int>("Offset"),
                    OTFSchedTime = dr.Field<int?>("OTFSchedTime"),
                    ProcessTechDescription = dr.Field<string>("ProcessTechDescription"),
                    ProcessTechGroupID = dr.Field<int>("ProcessTechGroupID"),
                    ProcessTechGroupName = dr.Field<string>("ProcessTechGroupName"),
                    ProcessTechID = dr.Field<int>("ProcessTechID"),
                    ProcessTechIsActive = dr.Field<bool>("ProcessTechIsActive"),
                    ProcessTechName = dr.Field<string>("ProcessTechName"),
                    ReservFence = dr.Field<int>("ReservFence"),
                    ResourceAutoEnd = dr.Field<int>("ResourceAutoEnd"),
                    ResourceDescription = dr.Field<string>("ResourceDescription"),
                    ResourceID = dr.Field<int>("ResourceID"),
                    ResourceIsActive = dr.Field<bool>("ResourceIsActive"),
                    ResourceName = dr.Field<string>("ResourceName"),
                    RoomDisplayName = dr.Field<string>("RoomDisplayName"),
                    RoomID = dr.Field<int>("RoomID"),
                    RoomName = dr.Field<string>("RoomName"),
                    State = dr.Field<ResourceState>("State"),
                    StateNotes = dr.Field<string>("StateNotes"),
                    UnloadTime = dr.Field<int?>("UnloadTime"),
                    WikiPageUrl = dr.Field<string>("WikiPageUrl")
                });
            }

            return result;
        }

        public IEnumerable<IReservation> SelectPastUnstarted()
        {
            return Session.NamedQuery("SelectPastUnstartedReservations").List<ReservationInfo>().AsQueryable().CreateModels<IReservation>();
        }

        public double SelectReservableMinutes(int resourceId, int clientId, TimeSpan reservFence, TimeSpan maxAlloc, DateTime now)
        {
            // this is all that happens in udf_SelectReservableMinutes

            //DECLARE @MaxAlloc integer
            //SELECT @MaxAlloc = MaxAlloc
            //FROM dbo.Resource
            //WHERE ResourceID = @ResourceID

            //DECLARE @ReservedMinutes int
            //SELECT @ReservedMinutes = ISNULL(SUM(Rv.Duration), 0)
            //FROM dbo.Reservation Rv, dbo.Resource R
            //WHERE Rv.ResourceID = R.ResourceID
            //  AND Rv.ResourceID = @ResourceID
            //  AND Rv.ClientID = @ClientID
            //  AND Rv.BeginDateTime < DATEADD(minute, R.ReservFence, @CurDate)
            //  AND Rv.EndDateTime >= @CurDate
            //  AND Rv.ActualEndDateTime IS NULL
            //  AND Rv.IsActive = 1
            //  AND Rv.ActivityID in (6, 7, 16)

            TimeSpan reserved = TimeSpan.Zero;
            int[] activityFilter = { 6, 7, 16 };

            var query = Session.Query<Reservation>().Where(x =>
                x.Resource.ResourceID == resourceId
                && x.Client.ClientID == clientId
                && x.BeginDateTime < now.Add(reservFence)
                && x.EndDateTime >= now
                && x.ActualEndDateTime == null
                && x.IsActive
                && activityFilter.Contains(x.Activity.ActivityID)).ToList();

            if (query != null)
                reserved = TimeSpan.FromMinutes(query.Sum(x => x.Duration));

            //RETURN @MaxAlloc - @ReservedMinutes

            return (maxAlloc - reserved).TotalMinutes;
        }

        public IEnumerable<IReservation> SelectReservationsByPeriod(DateTime period)
        {
            DateTime sdate = period;
            DateTime edate = sdate.AddMonths(1);

            var result = Session.Query<ReservationInfo>()
                .Where(x =>
                    x.ActualBeginDateTime != null &&
                    x.ActualEndDateTime != null &&
                    ((x.BeginDateTime < edate && x.EndDateTime > sdate) || (x.ActualBeginDateTime < edate && x.ActualEndDateTime > sdate))
                ).OrderBy(x => x.ClientID)
                .ThenBy(x => x.ResourceID)
                .ThenBy(x => x.AccountID)
                .CreateModels<IReservation>();

            return result;
        }

        public void StartReservation(int reservationId, int? modifiedByClientId)
        {
            // This is all that happens in procReservationUpdate @Action = 'Start'

            //UPDATE dbo.Reservation
            //SET ActualBeginDateTime = GETDATE(),
            //    ClientIDBegin = @ClientID,
            //    IsStarted = 1		
            //WHERE ReservationID = @ReservationID

            var r = GetExistingReservation(reservationId);

            r.ActualBeginDateTime = DateTime.Now;
            r.ClientIDBegin = modifiedByClientId.GetValueOrDefault(-1);
            r.IsStarted = true;

            Session.SaveOrUpdate(r);

            // also an entry into history is made
            InsertReservationHistory("Start", "procReservationUpdate", r, modifiedByClientId);
        }

        public IReservation UpdateReservation(UpdateReservationArgs args)
        {
            // This is all that happens in procReservationUpdate @Action = 'Update'

            //DECLARE @MaxReservedDuration FLOAT
            //SELECT @MaxReservedDuration = MaxReservedDuration
            //FROM Reservation 
            //WHERE ReservationID = @ReservationID

            //DECLARE @OriginalModifiedDateTime DATETIME2 = NULL
            //IF @OriginalBeginDateTime IS NOT NULL
            //BEGIN
            //    SELECT @OriginalModifiedDateTime = OriginalModifiedOn FROM Reservation WHERE ReservationID = @ReservationID
            //    IF @OriginalModifiedDateTime IS NULL	
            //        SET @OriginalModifiedDateTime = GETDATE()
            //END

            //UPDATE dbo.Reservation
            //SET AccountID = @AccountID,
            //    ActivityID = @ActivityID,
            //    BeginDateTime = @BeginDateTime,
            //    EndDateTime = @EndDateTime,
            //    LastModifiedOn = GETDATE(),
            //    Duration = @Duration,
            //    MaxReservedDuration = CASE WHEN @Duration > @MaxReservedDuration THEN @Duration ELSE MaxReservedDuration END,
            //    Notes = @Notes,
            //    AutoEnd = @AutoEnd,
            //    HasProcessInfo = @HasProcessInfo,
            //    HasInvitees = @HasInvitees,
            //    RecurrenceID = @RecurrenceID,
            //    KeepAlive = @KeepAlive,
            //    OriginalBeginDateTime = @OriginalBeginDateTime,
            //    OriginalEndDateTime = @OriginalEndDateTime,
            //    OriginalModifiedOn = @OriginalModifiedDateTime
            //WHERE ReservationID = @ReservationID

            var r = PrivateUpdate(args);

            // also an entry into history is made
            InsertReservationHistory("Update", "procReservationUpdate", r, args.ModifiedByClientID);

            return r.CreateModel<IReservation>();
        }

        public void UpdateAccount(int reservationId, int accountId, int? modifiedByClientId)
        {
            var rsv = Session.Single<Reservation>(reservationId);
            rsv.Account = Session.Single<Account>(accountId);
            InsertReservationHistory("ReservationUtility", "UpdateAccount", rsv, modifiedByClientId);
        }

        public int UpdateByGroup(int groupId, DateTime sd, DateTime ed, string notes, int? modifiedByClientId)
        {
            // This is all that happens in procReservationUpdate @Action = 'ByGroupID'

            //UPDATE dbo.Reservation
            //SET BeginDateTime = @BeginDateTime,
            //    EndDateTime = @EndDateTime,
            //    ActualBeginDateTime = @BeginDateTime,
            //    ActualEndDateTime = @EndDateTime
            //WHERE GroupID = @GroupID

            IList<Reservation> query = Session.Query<Reservation>().Where(x => x.GroupID == groupId).ToList();

            foreach (Reservation rsv in query)
            {
                rsv.BeginDateTime = sd;
                rsv.EndDateTime = ed;
                rsv.ActualBeginDateTime = sd;
                rsv.ActualEndDateTime = ed;
                rsv.AppendNotes(notes);

                Session.SaveOrUpdate(rsv);

                // also an entry into history is made for each reservation
                InsertReservationHistory("ByGroupID", "procReservationUpdate", rsv, modifiedByClientId);
            }

            return query.Count;
        }

        public void UpdateCharges(int reservationId, double chargeMultiplier, bool applyLateChargePenalty, int? modifiedByClientId)
        {
            // procReservationUpdate @Action = 'UpdateCharges'

            //UPDATE dbo.Reservation
            //SET ChargeMultiplier = @ChargeMultiplier,
            //    ApplyLateChargePenalty = @ApplyLateChargePenalty
            //WHERE ReservationID = @ReservationID

            var r = GetExistingReservation(reservationId);

            r.ChargeMultiplier = chargeMultiplier;
            r.ApplyLateChargePenalty = applyLateChargePenalty;

            Session.SaveOrUpdate(r);

            // also an entry into history is made
            InsertReservationHistory("UpdateCharges", "procReservationUpdate", r, modifiedByClientId);
        }

        public IReservation UpdateFacilityDownTime(int reservationId, DateTime beginDateTime, DateTime endDateTime, int? modifiedByClientId)
        {
            // this is all that happens in procReservationUpdate @Action = 'UpdateFacilityDownTime'

            //UPDATE dbo.Reservation
            //SET AccountID = @AccountID,
            //    ActivityID = @ActivityID,
            //    BeginDateTime = @BeginDateTime,
            //    EndDateTime = @EndDateTime,
            //    ActualBeginDateTime = @ActualBeginDateTime,
            //    ActualEndDateTime = @ActualEndDateTime,		
            //    LastModifiedOn = GETDATE(),
            //    Duration = @Duration,
            //    Notes = @Notes,
            //    AutoEnd = @AutoEnd,
            //    HasProcessInfo = @HasProcessInfo,
            //    HasInvitees = @HasInvitees,
            //    RecurrenceID = @RecurrenceID,
            //    TotalProcessRuns = @TotalProcessRuns
            //WHERE ReservationID = @ReservationID

            var r = GetExistingReservation(reservationId);

            if (r.Activity.ActivityID != Properties.Current.Activities.FacilityDownTime.ActivityID)
                throw new Exception($"{reservationId} is not a facility downtime reservation.");

            UpdateDuration(r, beginDateTime, endDateTime, DateTime.Now);

            // Facility down time must not need to be activated manually by person
            r.ActualBeginDateTime = beginDateTime;
            r.ActualEndDateTime = endDateTime;

            Session.SaveOrUpdate(r);

            // also an entry into history is made
            InsertReservationHistory("UpdateFacilityDownTime", "procReservationUpdate", r, modifiedByClientId);

            return r.CreateModel<IReservation>();
        }

        public IReservation UpdateRepair(int reservationId, DateTime endDateTime, string notes, int? modifiedByClientId)
        {
            var r = GetExistingReservation(reservationId);

            if (r.Activity.ActivityID != Properties.Current.Activities.Repair.ActivityID)
                throw new Exception($"{reservationId} is not a repair reservation.");

            UpdateDuration(r, r.BeginDateTime, endDateTime, DateTime.Now);

            Session.SaveOrUpdate(r);

            InsertReservationHistory("UpdateRepair", "procReservationUpdate", r, modifiedByClientId);

            return r.CreateModel<IReservation>();
        }

        public void UpdateNotes(int reservationId, string notes)
        {
            var rsv = Session.Single<Reservation>(reservationId);

            rsv.Notes = notes;

            Session.SaveOrUpdate(rsv);

            // there is no reason to add reservation history
            // because notes are not tracked in the ReservationHistory table
        }

        public IReservation GetReservation(int reservationId) => Session.Single<ReservationInfo>(reservationId).CreateModel<IReservation>();

        public IReservationWithInvitees GetReservationWithInvitees(int reservationId) => Session.Single<ReservationInfo>(reservationId).CreateModel<IReservationWithInvitees>();

        public IEnumerable<IReservation> GetReservations(int resourceId, DateTime sd, DateTime ed, bool? isActive = null, bool? isStarted = null)
        {
            return Session.Query<ReservationInfo>().Where(x =>
                x.ResourceID == resourceId
                && x.IsStarted == isStarted.GetValueOrDefault(x.IsStarted)
                && x.IsActive == isActive.GetValueOrDefault(x.IsActive)
                && (x.BeginDateTime < ed && x.EndDateTime > sd)
            ).CreateModels<IReservation>();
        }

        public TimeSpan GetTimeUntilNextReservation(IResource res, int reservationId, int clientId, DateTime beginDateTime)
        {
            // procReservationSelect @Action = 'TimeTillNextReservation'

            //DECLARE @NextBeginTime datetime
            //DECLARE @TimeTillNext int
            //SET @NextBeginTime = NULL
            //SET @TimeTillNext = 9999999 -- arbitrarily large number

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

            nextBeginTime = Session.Query<Reservation>()
                .Where(x =>
                    x.Resource.ResourceID == res.ResourceID
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

            double reservableMinutes = SelectReservableMinutes(res.ResourceID, clientId, TimeSpan.FromMinutes(res.ReservFence), TimeSpan.FromMinutes(res.MaxAlloc), DateTime.Now);

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
                Reservation rsv = Session.Single<Reservation>(reservationId);

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
                var maxReservTime = res.MaxReservTime;
                if (reservableMinutes >= maxReservTime)
                    return TimeSpan.FromMinutes(reservableMinutes);
                else
                    return TimeSpan.FromMinutes(-1 * reservableMinutes);
            }
        }

        public IEnumerable<IResource> GetResources(IEnumerable<IReservation> reservations)
        {
            int[] ids = reservations.Select(x => x.ResourceID).ToArray();
            var result = Session.Query<ResourceInfo>().Where(x => ids.Contains(x.ResourceID)).CreateModels<IResource>();
            return result;
        }

        public SaveReservationHistoryResult SaveReservationHistory(IReservation rsv, int accountId, double forgivenPct, string notes, bool emailClient)
        {
            double chargeMultiplier = 1.00 - (forgivenPct / 100.0);

            bool udpated = UpdateReservationHistory(new ReservationHistoryUpdate
            {
                ReservationID = rsv.ReservationID,
                AccountID = accountId,
                ChargeMultiplier = chargeMultiplier,
                Notes = notes,
                EmailClient = emailClient
            });

            var period = rsv.ChargeBeginDateTime.FirstOfMonth();
            var clientId = rsv.ClientID;

            string logmsg = Provider.Worker.Execute(new UpdateBillingWorkerRequest(period, clientId, new[] { "tool", "room" }));

            return new SaveReservationHistoryResult
            {
                ReservationUpdated = udpated,
                BillingLog = logmsg
            };
        }

        public int PurgeReservation(int reservationId)
        {
            string sql = "DELETE FROM sselScheduler.dbo.ReservationHistory WHERE ReservationID = @ReservationID; DELETE FROM sselScheduler.dbo.ReservationProcessInfo WHERE ReservationID = @ReservationID; DELETE FROM sselScheduler.dbo.ReservationInvitee WHERE ReservationID = @ReservationID; DELETE FROM sselScheduler.dbo.Reservation WHERE ReservationID = @ReservationID";

            int result = DA.Command(CommandType.Text)
                .Param("ReservationID", reservationId)
                .ExecuteNonQuery(sql).Value;

            return result;
        }

        public int PurgeReservations(int resourceId, DateTime sd, DateTime ed)
        {
            string sql;

            sql = "SELECT ReservationID FROM sselScheduler.dbo.Reservation WHERE ResourceID = @resourceId AND (BeginDateTime < @ed AND EndDateTime > @sd)";

            int[] reservationIds = DA.Command(CommandType.Text)
                .Param(new { resourceId, sd, ed })
                .FillDataTable(sql)
                .AsEnumerable()
                .Select(x => x.Field<int>("ReservationID"))
                .ToArray();

            sql = "DELETE FROM sselScheduler.dbo.ReservationHistory WHERE ReservationID IN (:p); DELETE FROM sselScheduler.dbo.ReservationProcessInfo WHERE ReservationID IN (:p); DELETE FROM sselScheduler.dbo.ReservationInvitee WHERE ReservationID IN (:p); DELETE FROM sselScheduler.dbo.Reservation WHERE ReservationID IN (:p)";

            int result = DA.Command(CommandType.Text)
                .ParamList("p", reservationIds)
                .ExecuteNonQuery(sql).Value;

            return result;
        }

        public IReservationRecurrence GetReservationRecurrence(int recurrenceId)
        {
            return Session.Single<ReservationRecurrence>(recurrenceId).CreateModel<IReservationRecurrence>();
        }

        public IEnumerable<IReservationRecurrence> GetReservationRecurrencesByResource(int resourceId)
        {
            return Session.Query<ReservationRecurrence>().Where(x => x.Resource.ResourceID == resourceId).CreateModels<IReservationRecurrence>();
        }

        public IEnumerable<IReservationRecurrence> GetReservationRecurrencesByProcessTech(int processTechId)
        {
            var query = Session.Query<Resource>().Where(x => x.ProcessTech.ProcessTechID == processTechId);
            var join = query.Join(Session.Query<ReservationRecurrence>(), o => o.ResourceID, i => i.Resource.ResourceID, (o, i) => i);
            return join.CreateModels<IReservationRecurrence>();
        }

        public IEnumerable<IReservationRecurrence> GetReservationRecurrencesByClient(int clientId)
        {
            var query = Session.Query<ReservationRecurrence>().Where(x => x.Client.ClientID == clientId);
            return query.CreateModels<IReservationRecurrence>();
        }

        public bool SaveReservationRecurrence(int recurrenceId, int patternId, int param1, int? param2, DateTime beginDate, TimeSpan beginTime, double duration, DateTime? endDate, bool autoEnd, bool keepAlive, string notes)
        {
            var rr = Session.Single<ReservationRecurrence>(recurrenceId);

            if (rr == null) return false;

            var endTime = beginTime.Add(TimeSpan.FromMinutes(duration));

            rr.Pattern = Session.Single<RecurrencePattern>(patternId);
            rr.AutoEnd = autoEnd;
            rr.KeepAlive = keepAlive;
            rr.PatternParam1 = param1;
            rr.PatternParam2 = param2;
            rr.Notes = notes;
            rr.BeginDate = beginDate.Date;
            rr.BeginTime = beginDate.Date.Add(beginTime);
            rr.Duration = duration;
            rr.EndDate = endDate;
            rr.EndTime = beginDate.Date.Add(endTime);

            Session.SaveOrUpdate(rr);

            return true;
        }

        public IEnumerable<IReservation> GetRecurringReservations(int recurrenceId, DateTime? sd, DateTime? ed)
        {
            var query = Session.Query<ReservationInfo>()
                .Where(rsv =>
                    rsv.RecurrenceID.HasValue
                    && rsv.RecurrenceID.Value == recurrenceId
                    && rsv.BeginDateTime < ed.GetValueOrDefault(rsv.EndDateTime)
                    && rsv.EndDateTime > sd.GetValueOrDefault(rsv.BeginDateTime))
                .OrderByDescending(x => x.ReservationID);

            return query.CreateModels<IReservation>();
        }

        public bool UpdateReservationHistory(ReservationHistoryUpdate model)
        {
            //this does not update billing, that needs to be done separately

            bool saveReservation = false;
            bool addReservationHistory = false;
            bool sendEmail = false;
            double forgivenAmount = 0;

            Reservation rsvMain = Session.Single<Reservation>(model.ReservationID);

            if (rsvMain == null) return false;

            Reservation rsv = rsvMain;

            while (true)  // this will loop through all linked reservations and update them.
            {
                if (model.AccountID.HasValue && rsv.Account.AccountID != model.AccountID.Value)
                {
                    Account acct = DA.Current.Single<Account>(model.AccountID.Value);
                    if (acct == null) return false;
                    rsv.Account = acct;
                    saveReservation = true;
                    addReservationHistory = true;
                }

                if (model.ChargeMultiplier.HasValue && rsv.ChargeMultiplier != model.ChargeMultiplier.Value)
                {
                    rsv.ChargeMultiplier = model.ChargeMultiplier.Value;
                    saveReservation = true;
                    addReservationHistory = true;
                    sendEmail = model.EmailClient;
                    forgivenAmount = (1 - model.ChargeMultiplier.Value) * 100.0;
                }

                if (model.Notes != null && model.Notes != rsv.Notes)
                {
                    rsv.Notes = model.Notes;
                    saveReservation = true;
                    // no need to add ReservationHistory for a notes change
                }

                if (saveReservation)
                {
                    DA.Current.SaveOrUpdate(rsv);

                    if (addReservationHistory)
                    {
                        //need to add ReservationHistory
                        DA.Current.SaveOrUpdate(new ReservationHistory()
                        {
                            Reservation = rsv,
                            UserAction = "UpdateReservationHistory",
                            ActionSource = "LNF.WebApi.Scheduler.Repository",
                            Account = rsv.Account,
                            BeginDateTime = rsv.BeginDateTime,
                            EndDateTime = rsv.EndDateTime,
                            ChargeMultiplier = rsv.ChargeMultiplier,
                            ModifiedByClientID = model.ClientID,
                            ModifiedDateTime = DateTime.Now
                        });
                    }
                }

                ReservationHistory rh = DA.Current.Query<ReservationHistory>().FirstOrDefault(x => x.Reservation == rsv && x.LinkedReservationID != null && x.UserAction == ReservationHistoryItem.INSERT_FOR_MODIFICATION);

                if (rh == null)  // if there are no more linked reservations
                    break;
                else
                {
                    rsv = DA.Current.Query<Reservation>().FirstOrDefault(x => x.ReservationID == rh.LinkedReservationID.Value);

                    if (rsv == null)
                    {
                        // this should never happen
                        break;
                    }
                }
            }

            if (sendEmail)
                ServiceProvider.Current.EmailManager.EmailOnForgiveCharge(rsv.CreateModel<IReservation>(), forgivenAmount, true, model.ClientID);

            return true;
        }

        public IEnumerable<IReservation> GetReservations(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0, int activityId = 0, bool? started = null, bool? active = null)
        {
            var query = ReservationsQuery(sd, ed, clientId, resourceId, activityId, started, active);
            var result = query.CreateModels<IReservation>();
            return result;
        }

        public IEnumerable<IReservationWithInvitees> GetReservationsWithInvitees(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0, int activityId = 0, bool? started = null, bool? active = null)
        {
            var query = ReservationsQuery(sd, ed, clientId, resourceId, activityId, started, active);
            var result = query.CreateModels<IReservationWithInvitees>();
            return result;
        }

        public IEnumerable<IReservationInvitee> GetReservationInvitees(int reservationId)
        {
            return Session.Query<ReservationInvitee>()
                .Where(x => x.Reservation.ReservationID == reservationId)
                .CreateModels<IReservationInvitee>();
        }

        public IResource GetResource(int reservationId)
        {
            return Session.Single<Reservation>(reservationId).Resource.CreateModel<IResource>();
        }

        public IEnumerable<ReservationStateItem> GetReservationStates(DateTime sd, DateTime ed, string kioskIp, int? clientId = null, int? resourceId = null, int? reserverId = null)
        {
            var invitees = DA.Current.Query<ReservationInvitee>().CreateModels<IReservationInvitee>();

            var resourceClients = DA.Current.Query<ResourceClientInfo>().Where(x => x.ResourceID == resourceId.GetValueOrDefault(x.ResourceID)).ToList();

            var clients = DA.Current.Query<ClientInfo>().CreateModels<IClient>();

            var query = DA.Current.Query<ReservationInfo>()
                .Where(x => x.ResourceID == resourceId.GetValueOrDefault(x.ResourceID)
                    && x.ClientID == reserverId.GetValueOrDefault(x.ClientID)
                    && ((x.BeginDateTime < ed && x.EndDateTime > sd) || (x.ActualBeginDateTime.HasValue && x.ActualBeginDateTime < ed && x.ActualEndDateTime.HasValue && x.ActualEndDateTime > sd)))
                .OrderBy(x => x.ResourceName).ThenBy(x => x.BeginDateTime).ThenBy(x => x.EndDateTime);

            var items = query.CreateModels<IReservationWithInvitees>();

            var util = new ReservationUtility(DateTime.Now, Provider);

            var access = new PhysicalAccessUtility(kioskIp);

            return items.Select(x =>
            {
                var reserverUser = clients.First(c => c.ClientID == x.ClientID);

                var currentUser = clients.First(c => c.ClientID == clientId.GetValueOrDefault(x.ClientID));

                var inLab = access.ClientInLab(currentUser.ClientID);

                var args = ReservationStateArgs.Create(x, GetReservationClient(x, currentUser, inLab));

                try
                {
                    var state = util.GetReservationState(args);

                    return new ReservationStateItem()
                    {
                        ReservationID = x.ReservationID,
                        ResourceID = x.ResourceID,
                        ResourceName = x.ResourceName,
                        Reserver = reserverUser,
                        CurrentUser = currentUser,
                        State = state,
                        BeginDateTime = x.BeginDateTime,
                        EndDateTime = x.EndDateTime,
                        ActualBeginDateTime = x.ActualBeginDateTime,
                        ActualEndDateTime = x.ActualEndDateTime,
                        IsToolEngineer = args.IsToolEngineer,
                        IsReserver = args.IsReserver,
                        IsInvited = args.IsInvited,
                        IsAuthorized = args.IsAuthorized,
                        BeforeMinCancelTime = args.IsBeforeMinCancelTime
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception($"{ex.Message} ReservationID: {x.ReservationID}, Resource: {x.ResourceName} [{x.ResourceID}], Client: {x.LName}, {x.FName} [{x.ClientID}]");
                }
            });
        }

        public IEnumerable<IReservationInvitee> ToReservationInviteeList(DataTable dt, int reservationId)
        {
            if (dt == null) return null;

            if (dt.Columns.Contains("ReservationID") && dt.Columns.Contains("InviteeID"))
            {
                var result = new List<IReservationInvitee>();

                foreach (DataRow dr in dt.Rows)
                {
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        int rid = Utility.ConvertTo(dr["ReservationID"], 0);

                        if (reservationId <= 0) rid = reservationId;

                        int inviteeId = Utility.ConvertTo(dr["InviteeID"], 0);

                        if (rid > 0 && inviteeId > 0)
                        {
                            var ri = GetReservationInvitee(rid, inviteeId);

                            if (ri != null)
                                result.Add(ri);
                        }
                    }
                }

                return result;
            }
            else
                throw new Exception("ReservationID and InviteeID columns are required.");
        }

        public IReservationInvitee GetReservationInvitee(int reservationId, int inviteeId)
        {
            var key = new ReservationInvitee()
            {
                Reservation = Session.Single<Reservation>(reservationId),
                Invitee = Session.Single<Client>(inviteeId)
            };

            return Session.Single<ReservationInvitee>(key).CreateModel<IReservationInvitee>();
        }

        public bool ReservationInviteeExists(int reservationId, int inviteeId)
        {
            return GetReservationInvitee(reservationId, inviteeId) != null;
        }

        public void InsertReservationInvitee(int reservationId, int inviteeId)
        {
            if (!ReservationInviteeExists(reservationId, inviteeId))
            {
                var ri = new ReservationInvitee()
                {
                    Reservation = Session.Single<Reservation>(reservationId),
                    Invitee = Session.Single<Client>(inviteeId)
                };

                Session.Insert(ri);
            }
        }

        public void DeleteReservationInvitee(int reservationId, int inviteeId)
        {
            var ri = GetExistingReservationInvitee(reservationId, inviteeId);
            Session.Delete(ri);
        }

        private ReservationHistory InsertReservationHistory(string action, string actionSource, Reservation rsv, int? modifiedByClientId = null, int? linkedReservationId = null)
        {
            // procReservationHistoryInsert [note that @Reservations is a user-defined type (basically a temp table)]

            //INSERT dbo.ReservationHistory (ReservationID, LinkedReservationID, UserAction, ActionSource, ModifiedByClientID, ModifiedDateTime, AccountID, BeginDateTime, EndDateTime, ChargeMultiplier)
            //SELECT r.ReservationID, r.LinkedReservationID, @UserAction, @ActionSource, r.ClientID, GETDATE(), r.AccountID, r.BeginDateTime, r.EndDateTime, r.ChargeMultiplier
            //FROM @Reservations r

            ReservationHistory result = new ReservationHistory()
            {
                Reservation = rsv,
                LinkedReservationID = linkedReservationId,
                UserAction = action,
                ActionSource = actionSource,
                ModifiedByClientID = modifiedByClientId,
                ModifiedDateTime = DateTime.Now,
                Account = rsv.Account,
                BeginDateTime = rsv.BeginDateTime,
                EndDateTime = rsv.EndDateTime,
                ChargeMultiplier = rsv.ChargeMultiplier
            };

            Session.Insert(result);

            return result;
        }

        private Reservation PrivateUpdate(UpdateReservationArgs args)
        {
            var r = Session.Single<Reservation>(args.ReservationID);

            if (r == null)
                throw new Exception($"Cannot find Reservation with ReservationID = {args.ReservationID}");

            r.Account = Session.Single<Account>(args.AccountID);
            r.Notes = args.Notes;
            r.ChargeMultiplier = args.ChargeMultiplier;
            r.AutoEnd = args.AutoEnd;
            r.KeepAlive = args.KeepAlive;
            r.HasProcessInfo = args.HasProcessInfo;
            r.HasInvitees = args.HasInvitees;
            r.OriginalModifiedOn = r.LastModifiedOn;
            r.LastModifiedOn = args.Now;

            // can't hurt
            Session.SaveOrUpdate(r);

            return r;
        }

        private Reservation PrivateInsert(InsertReservationArgs args)
        {
            var res = Session.Single<Resource>(args.ResourceID);
            var client = Session.Single<Client>(args.ClientID);
            var account = Session.Single<Account>(args.AccountID);
            var activity = Session.Single<Activity>(args.ActivityID);

            CanCreateCheck(args, client, res);

            var r = new Reservation
            {
                Resource = res,
                Client = client,
                Account = account,
                Activity = activity,
                BeginDateTime = args.BeginDateTime,
                EndDateTime = args.EndDateTime,
                ActualBeginDateTime = null,
                ActualEndDateTime = null,
                ClientIDBegin = null,
                ClientIDEnd = null,
                CreatedOn = args.Now,
                LastModifiedOn = args.Now,
                Duration = args.Duration.TotalMinutes,
                Notes = args.Notes,
                ChargeMultiplier = 1,
                ApplyLateChargePenalty = true,
                AutoEnd = args.AutoEnd,
                HasProcessInfo = args.HasProcessInfo,
                HasInvitees = args.HasInvitees,
                IsActive = true,
                IsStarted = false,
                IsUnloaded = false,
                RecurrenceID = args.RecurrenceID,
                GroupID = null,
                MaxReservedDuration = args.Duration.TotalMinutes,
                CancelledDateTime = null,
                KeepAlive = args.KeepAlive,
                OriginalBeginDateTime = null,
                OriginalEndDateTime = null,
                OriginalModifiedOn = null
            };

            Session.Insert(r);

            return r;
        }

        private void CanCreateCheck(InsertReservationArgs args, Client client, Resource res)
        {
            // ignore recurring reservations
            if (!args.RecurrenceID.HasValue)
            {
                // These two activites allow for creating a reservation in the past
                if (args.ActivityID != Properties.Current.Activities.FacilityDownTime.ActivityID && args.ActivityID != Properties.Current.Activities.Repair.ActivityID)
                {
                    // Granularity			: stored in minutes and entered in minutes
                    // Offset				: stored in hours and entered in hours 
                    DateTime granStartTime = ResourceUtility.GetNextGranularity(res.CreateModel<IResource>(), args.Now, GranularityDirection.Previous);

                    if (args.BeginDateTime < granStartTime)
                    {
                        string body = $"Unable to create a reservation. BeginDateTime is in the past.\n--------------------\nClient: {client.DisplayName} [{client.ClientID}]\nResource: {res.ResourceName} [{res.ResourceID}]\nBeginDateTime: {args.BeginDateTime:yyyy-MM-dd HH:mm:ss}\nEndDateTime: {args.EndDateTime:yyyy-MM-dd HH:mm:ss}\nModifiedByClientID: {args.ModifiedByClientID}";
                        SendEmail.SendDeveloperEmail("LNF.Repository.Scheduler.Reservation.CanCreateCheck", "Create reservation failed", body);
                        throw new Exception("Your reservation was not created. Cannot create a reservation in the past.");
                    }
                }
            }

            if (args.EndDateTime <= args.BeginDateTime)
            {
                string body = $"Unable to create a reservation. EndDateTime is before BeginDateTime.\n--------------------\nClient: {client.DisplayName} [{client.ClientID}]\nResource: {res.ResourceName} [{res.ResourceID}]\nBeginDateTime: {args.BeginDateTime:yyyy-MM-dd HH:mm:ss}\nEndDateTime: {args.EndDateTime:yyyy-MM-dd HH:mm:ss}\nModifiedByClientID: {args.ModifiedByClientID}";
                SendEmail.SendDeveloperEmail("LNF.Repository.Scheduler.Reservation.CanCreateCheck", "Create reservation failed", body);
                throw new Exception("Your reservation was not created. Cannot create a reservation that ends before it starts.");
            }

            // conflicting reservations must be:
            //      1) same resource
            //      2) not canceled
            //      3) date ranges overlap

            var conflict = GetReservations(res.ResourceID, args.BeginDateTime, args.EndDateTime, true).FirstOrDefault(x => !x.ActualEndDateTime.HasValue);

            if (conflict != null)
            {
                string body = $"Unable to create a reservation. There is a conflict with an existing reservation.\n--------------------\nClient: {client.DisplayName} {client.ClientID}\nResource: {res.ResourceName} [{res.ResourceID}]\nBeginDateTime: {args.BeginDateTime:yyyy-MM-dd HH:mm:ss}\nEndDateTime: {args.EndDateTime:yyyy-MM-dd HH:mm:ss}\nConflictingReservationID: {conflict.ReservationID}\nModifiedByClientID: {args.ModifiedByClientID}";
                SendEmail.SendDeveloperEmail("LNF.Repository.Scheduler.Reservation.CanCreateCheck", "Create reservation failed", body);
                throw new Exception(string.Format("Your reservation was not created. There is a conflict with an existing reservation [#{0}].", conflict.ReservationID));
            }
        }

        private void UpdateDuration(Reservation r, DateTime beginDateTime, DateTime endDateTime, DateTime now)
        {
            r.OriginalBeginDateTime = r.BeginDateTime;
            r.OriginalEndDateTime = r.EndDateTime;
            r.OriginalModifiedOn = r.LastModifiedOn;
            r.LastModifiedOn = now;
            r.BeginDateTime = beginDateTime;
            r.EndDateTime = endDateTime;
            r.Duration = (endDateTime - beginDateTime).TotalMinutes;
            r.MaxReservedDuration = Math.Max(r.Duration, r.MaxReservedDuration);
        }

        private IQueryable<Reservation> ReservationsQuery(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0, int activityId = 0, bool? started = null, bool? active = null)
        {
            var query = Session.Query<Reservation>().Where(x =>
                    (x.BeginDateTime < ed && x.EndDateTime > sd)
                    || ((x.ActualBeginDateTime == null ? false : x.ActualBeginDateTime < ed) && (x.ActualEndDateTime == null ? true : x.ActualEndDateTime > sd)));

            if (clientId != 0)
                query = query.Where(x => x.Client.ClientID == clientId);

            if (resourceId != 0)
                query = query.Where(x => x.Resource.ResourceID == resourceId);

            if (activityId != 0)
                query = query.Where(x => x.Activity.ActivityID == activityId);

            if (started.HasValue)
                query = query.Where(x => x.IsStarted == started.Value);

            if (active.HasValue)
                query = query.Where(x => x.IsActive == active.Value);

            return query;
        }

        private ReservationClient GetReservationClient(IReservationWithInvitees rsv, IClient client, bool inLab)
        {
            var resourceClients = GetResourceClients(rsv.ResourceID);
            var userAuth = ReservationUtility.GetAuthLevel(resourceClients, client);

            var result = new ReservationClient
            {
                ClientID = client.ClientID,
                ReservationID = rsv.ReservationID,
                ResourceID = rsv.ResourceID,
                IsReserver = rsv.ClientID == client.ClientID,
                IsInvited = rsv.Invitees.Any(x => x.InviteeID == client.ClientID),
                InLab = inLab,
                UserAuth = userAuth
            };

            return result;
        }

        private Reservation GetExistingReservation(int reservationId)
        {
            var result = Session.Single<Reservation>(reservationId);
            if (result == null)
                throw new Exception($"Cannot find Reservation with ReservationID = {reservationId}");
            return result;
        }

        private ReservationInvitee GetExistingReservationInvitee(int reservationId, int inviteeId)
        {
            var r = Session.Single<Reservation>(reservationId);
            var i = Session.Single<Client>(inviteeId);

            var result = Session.Single<ReservationInvitee>(new ReservationInvitee { Reservation = r, Invitee = i });

            if (result == null)
                throw new Exception($"Cannot find ReservationInvitee with ReservationID = {reservationId} and InviteeID = {inviteeId}");

            return result;
        }



        private void AddNewRow(IReservationRecurrence rr, DateTime startTime, IList<IReservation> reservations)
        {

        }
    }
}
