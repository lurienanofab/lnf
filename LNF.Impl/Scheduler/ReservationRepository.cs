using LNF.Billing.Process;
using LNF.CommonTools;
using LNF.Data;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Data;
using LNF.Impl.Repository.Scheduler;
using LNF.PhysicalAccess;
using LNF.Scheduler;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Impl.Scheduler
{
    public class ReservationRepository : RepositoryBase, IReservationRepository
    {
        protected IClientRepository Client { get; }
        protected IProcessRepository Process { get; }
        protected IEmailRepository Email { get; }
        protected IKioskRepository Kiosk { get; }

        public ReservationRepository(ISessionManager mgr, IClientRepository client, IProcessRepository process, IEmailRepository email, IKioskRepository kiosk) : base(mgr)
        {
            Client = client;
            Process = process;
            Email = email;
            Kiosk = kiosk;
        }

        public IReservation GetReservation(int reservationId) => Session.Get<ReservationInfo>(reservationId);

        public IReservationItem GetReservationItem(int reservationId) => Session.Get<ReservationItem>(reservationId);

        public IEnumerable<IReservation> GetReservations(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0, int activityId = 0, bool? started = null, bool? active = null)
        {
            var query = ReservationInfoQuery(sd, ed, clientId, resourceId, activityId, started, active);
            var result = query.ToList();
            return result;
        }

        public IReservationWithInvitees GetReservationWithInvitees(int reservationId) => CreateReservationWithInviteesModel(Session.Get<ReservationInfo>(reservationId));

        public IEnumerable<IReservationWithInvitees> GetReservationsWithInvitees(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0, int activityId = 0, bool? started = null, bool? active = null)
        {
            var query = ReservationInfoQuery(sd, ed, clientId, resourceId, activityId, started, active);
            var result = query.ToList().Select(x => CreateReservationWithInviteesModel(x)).ToList();
            return result;
        }

        public IEnumerable<IClientAccount> AvailableAccounts(int reservationId, ActivityAccountType accountType)
        {
            IList<IClientAccount> result = null;

            var rsv = GetReservation(reservationId);

            DateTime sd = rsv.CreatedOn.Date;
            DateTime ed = sd.AddDays(1);

            if (accountType == ActivityAccountType.Reserver || accountType == ActivityAccountType.Both)
            {
                //Load reserver's accounts
                result = Client.GetActiveClientAccounts(rsv.ClientID, sd, ed).ToList();
            }

            if (accountType == ActivityAccountType.Invitee || accountType == ActivityAccountType.Both)
            {
                //Loads each of the invitee's accounts
                foreach (var ri in GetInvitees(rsv.ReservationID))
                {
                    var temp = Client.GetActiveClientAccounts(ri.InviteeID, sd, ed);

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
            var rsv = Session.GetNamedQuery("CreateReservation")
                .SetParameter("ResourceID", resourceId)
                .SetParameter("ClientID", clientId)
                .SetParameter("AccountID", accountId)
                .SetParameter("ActivityID", activityId)
                .SetParameter("BeginDateTime", beginDateTime)
                .SetParameter("EndDateTime", endDateTime)
                .SetParameter("Duration", duration)
                .SetParameter("Notes", notes)
                .SetParameter("AutoEnd", autoEnd)
                .SetParameter("HasProcessInfo", hasProcessInfo)
                .SetParameter("HasInvitees", hasInvitees)
                .SetParameter("RecurrenceID", recurrenceId)
                .SetParameter("IsActive", isActive)
                .SetParameter("CreatedOn", DateTime.Now)
                .SetParameter("KeepAlive", keepAlive)
                .SetParameter("MaxReservedDuration", maxReservedDuration)
                .SetParameter("ModifiedByClientID", modifiedByClientId)
                .List<Reservation>().FirstOrDefault();

            var result = CreateReservationModel<ReservationInfo>(rsv);

            return result;
        }

        public void CancelReservation(int reservationId, string note, int? modifiedByClientId)
        {
            var r = Require<Reservation>(reservationId);

            // Cancel reservation
            r.IsActive = false;
            r.LastModifiedOn = DateTime.Now;
            r.CancelledDateTime = DateTime.Now;
            r.AppendNotes(note);

            Session.SaveOrUpdate(r);

            // also add an entry into history
            InsertReservationHistory("ByReservationID", "procReservationDelete", r, modifiedByClientId);
        }

        public void CancelAndForgive(int reservationId, string note, int? modifiedByClientId)
        {
            // This is all that happens in procReservationDelete @Action = 'WithForgive'

            //-- Set Reservation to inactive
            //UPDATE dbo.Reservation
            //SET IsActive = 0, LastModifiedOn = GETDATE(), CancelledDateTime = GETDATE(), ChargeMultiplier = 0
            //WHERE ReservationID = @ReservationID

            var r = Require<Reservation>(reservationId);

            r.IsActive = false;
            r.LastModifiedOn = DateTime.Now;
            r.CancelledDateTime = DateTime.Now;
            r.ChargeMultiplier = 0;
            r.AppendNotes(note);

            //-- Delete Reservation ProcessInfos
            //DELETE FROM dbo.ReservationProcessInfo
            //WHERE ReservationID = @ReservationID

            Session.DeleteMany(Session.Query<ReservationProcessInfo>().Where(x => x.ReservationID == r.ReservationID));

            //-- Delete Reservation Invitees
            //DELETE FROM dbo.ReservationInvitee
            //WHERE ReservationID = @ReservationID

            Session.DeleteMany(Session.Query<ReservationInvitee>().Where(x => x.Reservation.ReservationID == r.ReservationID));

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
        public void EndReservation(EndReservationArgs args)
        {
            var rsv = Require<Reservation>(args.ReservationID);

            if (rsv.ActualBeginDateTime == null)
                throw new Exception($"Cannot end Reservation #{rsv.ReservationID} because it was never started (ActualBeginDateTime is null).");

            if (rsv.ActualBeginDateTime.Value > args.ActualEndDateTime)
                throw new Exception($"ActualBeginDateTime is greater than ActualEndDateTime [ReservationID: {rsv.ReservationID}, ResourceID: {rsv.Resource.ResourceID}, ActualBeginDateTime: {rsv.ActualBeginDateTime:yyyy-MM-dd HH:mm:ss}, ActualEndDateTime: {args.ActualEndDateTime:yyyy-MM-dd HH:mm:ss}");

            rsv.ActualEndDateTime = args.ActualEndDateTime;
            rsv.ClientIDEnd = args.EndedByClientID;

            if (!rsv.Activity.Editable)
            {
                rsv.Resource.State = ResourceState.Online;
                Session.SaveOrUpdate(rsv.Resource);
            }

            Session.SaveOrUpdate(rsv);

            // also an entry into history is made
            InsertReservationHistory("End", "procReservationUpdate", rsv, args.EndedByClientID);
        }

        public void EndAndForgiveForRepair(int reservationId, string notes, int? endedByClientId, int? modifiedByClientId)
        {
            //thinking about switching back to this
            //DataCommand()
            //    .Param("Action", "EndForRepair")
            //    .Param("ReservationID", reservationId)
            //    .Param("Notes", notes)
            //    .Param("ClientID", endedByClientId)
            //    .Param("ModifiedByClientID", modifiedByClientId)
            //    .ExecuteNonQuery("sselScheduler.dbo.procReservationUpdate");

            // This is all that happens in procReservationUpdate @Action = 'EndForRepair'

            //UPDATE dbo.Reservation
            //SET ActualEndDateTime = GETDATE(),
            //    ChargeMultiplier = 0,
            //    ApplyLateChargePenalty = 0,
            //    ClientIDEnd = @ClientID
            //WHERE ReservationID = @ReservationID

            var r = Require<Reservation>(reservationId);

            r.ActualEndDateTime = DateTime.Now;
            r.ChargeMultiplier = 0;
            r.ApplyLateChargePenalty = false;
            r.ClientIDEnd = endedByClientId.GetValueOrDefault(-1);
            r.AppendNotes(notes);

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
            return Session.GetNamedQuery("EndPastUnstartedReservations")
                .SetParameter("ReservationID", reservationId)
                .SetParameter("EndDateTime", endDate)
                .SetParameter("ClientID", endedByClientId.GetValueOrDefault(-1))
                .UniqueResult<int>();
        }

        public IEnumerable<ReservationHistoryFilterItem> FilterCancelledReservations(IEnumerable<IReservationItem> items, bool includeCanceledForModification)
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
                var result = Session.Get<ReservationInfo>(id);
                if (result != null)
                    return result;
            }

            return null;
        }

        public IEnumerable<IResourceClient> GetResourceClients(int resourceId)
        {
            return Session.Query<ResourceClientInfo>().Where(x => x.ResourceID == resourceId).ToList();
        }

        public int GetAvailableSchedMin(int resourceId, int clientId)
        {
            string sql = "EXEC sselScheduler.dbo.procReservationSelect @Action = 'GetAvailableSchedMin', @ResourceID = :ResourceID, @ClientID = :ClientID";
            return Session.CreateSQLQuery(sql)
                .SetParameter("ResourceID", resourceId)
                .SetParameter("ClientID", clientId)
                .UniqueResult<int>();
        }

        public IEnumerable<IReservation> GetCurrentReservations()
        {
            return Session.Query<ReservationInfo>()
                .Where(x => x.IsActive && x.IsStarted && x.ActualBeginDateTime != null && x.ActualEndDateTime == null)
                .OrderBy(x => x.ActualBeginDateTime)
                .ToList();
        }

        public IEnumerable<IReservationHistory> GetHistory(int reservationId)
        {
            return Session.Query<ReservationHistory>()
                .Where(x => x.Reservation.ReservationID == reservationId)
                .ToList()
                .Select(x => CreateReservationHistoryModel(x))
                .ToList();
        }

        public IReservationItem InsertReservation(InsertReservationArgs args)
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

            return CreateReservationItem(r);
        }

        public IReservationItem InsertForModification(InsertReservationArgs args)
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

            var linkedReservation = Require<Reservation>(args.LinkedReservationID);

            var r = PrivateInsert(args);

            r.ChargeMultiplier = linkedReservation.ChargeMultiplier;
            r.OriginalBeginDateTime = linkedReservation.OriginalBeginDateTime.GetValueOrDefault(linkedReservation.BeginDateTime);
            r.OriginalEndDateTime = linkedReservation.OriginalEndDateTime.GetValueOrDefault(linkedReservation.EndDateTime);
            r.OriginalModifiedOn = linkedReservation.OriginalModifiedOn.GetValueOrDefault(linkedReservation.LastModifiedOn);
            r.MaxReservedDuration = Math.Max(r.MaxReservedDuration, linkedReservation.MaxReservedDuration);

            Session.Update(r);

            InsertReservationHistory("InsertForModification", "procReservationInsert", r, args.ModifiedByClientID, linkedReservation.ReservationID);

            return CreateReservationItem(r);
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

            var act = Session.Get<Activity>(Properties.Current.Activities.FacilityDownTime.ActivityID);

            if (act == null)
                throw new InvalidOperationException("Cannot start a facility downtime reservation because a facility downtime activity is not defined.");

            double duration = (endDateTime - beginDateTime).TotalMinutes;

            int accountId = Properties.Current.LabAccount.AccountID;

            var r = new Reservation
            {
                Resource = Session.Get<Resource>(resourceId),
                Client = Session.Get<Client>(clientId),
                Account = Session.Get<Account>(accountId),
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

            Session.Save(r);

            InsertReservationHistory("InsertFacilityDownTime", "procReservationInsert", r, modifiedByClientId);

            return CreateReservationModel<ReservationInfo>(r);
        }

        public IReservation InsertRepair(int resourceId, int clientId, DateTime beginDateTime, DateTime endDateTime, DateTime actualBeginDateTime, string notes, int? modifiedByClientId)
        {
            // procReservationInsert @Action = 'InsertRepair'

            //SELECT @ActivityID = ActivityID
            //FROM dbo.Activity
            //WHERE Editable = 0

            var repairActivity = Session.Get<Activity>(Properties.Current.Activities.Repair.ActivityID);

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
                Resource = Session.Get<Resource>(resourceId),
                Client = Session.Get<Client>(clientId),
                Account = Session.Get<Account>(accountId),
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

            Session.Save(r);

            InsertReservationHistory("InsertRepair", "procReservationInsert", r, modifiedByClientId);

            return CreateReservationModel<ReservationInfo>(r);
        }

        public void AppendNotes(int reservationId, string notes)
        {
            var rsv = Session.Get<Reservation>(reservationId);

            if (rsv != null)
                rsv.AppendNotes(notes);
        }

        public bool IsInvited(int reservationId, int clientId)
        {
            return Session.Query<ReservationInviteeItem>().Any(x => x.ReservationID == reservationId && x.InviteeID == clientId);
        }

        public IEnumerable<IReservation> ReservationsInGranularityWindow(IResource res)
        {
            DateTime edate = DateTime.Now.AddMinutes(res.Granularity);
            var result = Session.Query<ReservationInfo>().Where(x => x.BeginDateTime >= DateTime.Now && x.BeginDateTime < edate && x.ResourceID == res.ResourceID).ToList();
            return result;
        }

        public IEnumerable<IReservation> SelectAutoEnd()
        {
            // returns reservation that should be ended based on
            //      1) Reservation.AutoEnd (true or false)
            //      2) Resource.AutoEnd (number of minutes to auto end, only used when Reservation.AutoEnd is false)
            return Session.GetNamedQuery("SelectAutoEndReservations").List<ReservationInfo>();
        }

        public IEnumerable<IReservationItem> SelectByClient(int clientId, DateTime sd, DateTime ed, bool includeDeleted)
        {
            List<IReservationItem> result = new List<IReservationItem>();

            result.AddRange(ByClientQuery(clientId, sd, ed, includeDeleted));
            result.AddRange(ByInviteeQuery(clientId, sd, ed, includeDeleted));

            return result;
        }

        public IEnumerable<IReservationItem> SelectByDateRange(DateTime sd, DateTime ed, bool includeDeleted)
        {
            object active = DBNull.Value;
            if (!includeDeleted)
                active = true;

            var dt = DataCommand()
                .Param("Action", "SelectByDateRange")
                .Param("StartDate", sd)
                .Param("EndDate", ed)
                .Param("IsActive", active)
                .FillDataTable("sselScheduler.dbo.procReservationItemSelect");

            var result = CreateReservationItems(dt);

            return result;
        }

        public IEnumerable<IReservationItem> SelectByGroup(int groupId)
        {
            // procReservationSelect @Action = 'ByGroupID'

            //SELECT res.ResourceID, res.ResourceName
            //FROM Reservation r
            //INNER JOIN [Resource] res ON res.ResourceID = r.ResourceID
            //WHERE r.GroupID = @GroupID

            var dt = DataCommand()
                .Param("Action", "SelectByGroup")
                .Param("GroupID", groupId)
                .FillDataTable("sselScheduler.dbo.procReservationItemSelect");

            var result = CreateReservationItems(dt);

            return result;
        }

        public IEnumerable<IReservationItem> SelectByProcessTech(int procTechId, DateTime sd, DateTime ed, bool includeDeleted)
        {
            object active = DBNull.Value;
            if (!includeDeleted)
                active = true;

            var dt = DataCommand()
                .Param("Action", "SelectByProcessTech")
                .Param("ProcessTechID", procTechId)
                .Param("StartDate", sd)
                .Param("EndDate", ed)
                .Param("IsActive", active)
                .FillDataTable("sselScheduler.dbo.procReservationItemSelect");

            var result = CreateReservationItems(dt);

            return result;
        }

        public IEnumerable<IReservationItem> SelectByLabLocation(int labLocationId, DateTime sd, DateTime ed, bool includeDeleted)
        {
            object active = DBNull.Value;
            if (!includeDeleted)
                active = true;

            var dt = DataCommand()
                .Param("Action", "SelectByLabLocation")
                .Param("LabLocationID", labLocationId)
                .Param("StartDate", sd)
                .Param("EndDate", ed)
                .Param("IsActive", active)
                .FillDataTable("sselScheduler.dbo.procReservationItemSelect");

            var result = CreateReservationItems(dt);

            return result;
        }

        public IEnumerable<IReservationItem> SelectByResource(int resourceId, DateTime sd, DateTime ed, bool includeDeleted)
        {
            object active = DBNull.Value;
            if (!includeDeleted)
                active = true;

            var dt = DataCommand()
                .Param("Action", "SelectByResource")
                .Param("ResourceID", resourceId)
                .Param("StartDate", sd)
                .Param("EndDate", ed)
                .Param("IsActive", active)
                .FillDataTable("sselScheduler.dbo.procReservationItemSelect");

            var result = CreateReservationItems(dt);

            return result;
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
                .ToList();

            return result;
        }

        public IEnumerable<IReservation> SelectExisting(int resourceId)
        {
            DateTime now = DateTime.Now;
            var reservationsWithFutureEndDateTime = Session.Query<ReservationInfo>().Where(x => x.EndDateTime > DateTime.Now && x.ActualEndDateTime == null && x.ResourceID == resourceId).ToList();
            var reservationsWithFutureEndDateTimeWhereChargeBeginDateTimeLessThanNow = reservationsWithFutureEndDateTime.Where(x => x.ChargeBeginDateTime < now).ToList();
            return reservationsWithFutureEndDateTimeWhereChargeBeginDateTimeLessThanNow;
        }

        public IEnumerable<IReservation> SelectHistory(int clientId, DateTime sd, DateTime ed)
        {
            var query = Session.Query<ReservationInfo>()
                .Where(x => x.ClientID == clientId && (x.BeginDateTime < ed) && (x.EndDateTime > sd));

            var result = query.ToList();

            return result;
        }

        public IEnumerable<ReservationToForgiveForRepair> SelectHistoryToForgiveForRepair(int resourceId, DateTime sd, DateTime ed)
        {
            // [2013-05-20 jg] We no longer care if the reservation was canceled or not, all need to be forgiven
            //      because of the booking fee on uncancelled reservations.

            // [2018-08-29 jg] The only problem is when a repair is extened the email will be sent again to
            //      a reservation that was already forgiven for repair (when the repair was made originally)

            int repairActivityId = 14;

            var query = Session.Query<Reservation>().Where(x =>
                x.Resource.ResourceID == resourceId
                && ((x.BeginDateTime < ed && x.EndDateTime > sd) || (x.ActualBeginDateTime < ed && x.ActualEndDateTime > sd)) //will include all overlapping reservations
                && x.Activity.ActivityID != repairActivityId).OrderBy(x => x.BeginDateTime).ToList();

            var result = query.Select(x => new ReservationToForgiveForRepair
            {
                ReservationID = x.ReservationID,
                ChargeMultiplier = x.ChargeMultiplier
            }).ToList();

            return result;
        }

        public DateTime? GetLastRepairEndTime(int resourceId)
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
                && x.ActualEndDateTime == null).ToList();

            return result;

        }

        public IEnumerable<IReservation> SelectPastEndableRepair()
        {
            var dt = Session.Command()
                .Param("Action", "SelectPastEndableRepair")
                .FillDataTable("sselScheduler.dbo.procReservationSelect");

            var result = new List<IReservation>();

            foreach (DataRow dr in dt.Rows)
            {
                result.Add(new ReservationInfo
                {
                    ReservationID = dr.Field<int>("ReservationID"),
                    ClientID = dr.Field<int>("ClientID"),
                    UserName = dr.Field<string>("UserName"),
                    LName = dr.Field<string>("LName"),
                    MName = dr.Field<string>("MName"),
                    FName = dr.Field<string>("FName"),
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
                    ClientIDEnd = dr.Field<int?>("ClientIDEnd"),
                    CreatedOn = dr.Field<DateTime>("CreatedOn"),
                    LastModifiedOn = dr.Field<DateTime>("LastModifiedOn"),
                    Duration = dr.Field<double>("Duration"),
                    Notes = dr.Field<string>("Notes"),
                    ChargeMultiplier = dr.Field<double>("ChargeMultiplier"),
                    ApplyLateChargePenalty = dr.Field<bool>("ApplyLateChargePenalty"),
                    ReservationAutoEnd = dr.Field<bool>("AutoEnd"),
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
                    State = dr.Field<ResourceState>("State"),
                    StateNotes = dr.Field<string>("StateNotes"),
                    UnloadTime = dr.Field<int?>("UnloadTime"),
                    WikiPageUrl = dr.Field<string>("WikiPageUrl"),
                    ClientAddressID = dr.Field<int>("ClientAddressID"),
                    ClientOrgActive = dr.Field<bool>("ClientOrgActive"),
                    ClientOrgID = dr.Field<int>("ClientOrgID"),
                    Email = dr.Field<string>("Email"),
                    Phone = dr.Field<string>("Phone"),
                    IsFinManager = GetBoolValue(dr, "IsFinManager"),
                    IsManager = GetBoolValue(dr, "IsManager"),
                    NewFacultyStartDate = dr.Field<DateTime?>("NewFacultyStartDate"),
                    SubsidyStartDate = dr.Field<DateTime?>("SubsidyStartDate")
                });
            }

            return result;
        }

        private bool GetBoolValue(DataRow dr, string col)
        {
            if (!dr.Table.Columns.Contains(col))
                return false;

            if (dr[col] == DBNull.Value)
                return false;

            if (dr.Table.Columns[col].DataType == typeof(bool))
            {
                bool val = Convert.ToBoolean(dr[col]);
                return val;
            }

            if (dr.Table.Columns[col].DataType == typeof(short))
            {
                short val = Convert.ToInt16(dr[col]);
                return val != 0;
            }

            if (dr.Table.Columns[col].DataType == typeof(int))
            {
                int val = Convert.ToInt32(dr[col]);
                return val != 0;
            }

            if (dr.Table.Columns[col].DataType == typeof(long))
            {
                long val = Convert.ToInt64(dr[col]);
                return val != 0;
            }

            if (dr.Table.Columns[col].DataType == typeof(double))
            {
                double val = Convert.ToDouble(dr[col]);
                return val != 0;
            }

            if (dr.Table.Columns[col].DataType == typeof(decimal))
            {
                decimal val = Convert.ToDecimal(dr[col]);
                return val != 0;
            }

            if (dr.Table.Columns[col].DataType == typeof(byte))
            {
                byte val = Convert.ToByte(dr[col]);
                return val != 0;
            }

            if (dr.Table.Columns[col].DataType == typeof(string))
            {
                string val = dr[col].ToString().ToLower();
                return val == "true";
            }

            return false;
        }

        public IEnumerable<IReservation> SelectPastUnstarted()
        {
            return Session.GetNamedQuery("SelectPastUnstartedReservations").List<ReservationInfo>();
        }

        public IEnumerable<IReservation> SelectUnstarted(int resourceId, DateTime sd, DateTime ed)
        {
            var result = Session.Query<ReservationInfo>()
                .Where(x =>
                    x.ResourceID == resourceId
                    && x.IsActive
                    && x.ActualBeginDateTime == null
                    && (x.BeginDateTime < ed && x.EndDateTime > sd))
                .ToList();

            return result;
        }

        public double GetReservableMinutes(int resourceId, int clientId, TimeSpan reservFence, TimeSpan maxAlloc, DateTime now)
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

            var query = Session.Query<ReservationItem>().Where(x =>
                x.ResourceID == resourceId
                && x.ClientID == clientId
                && x.BeginDateTime < now.Add(reservFence)
                && x.EndDateTime >= now
                && x.ActualEndDateTime == null
                && x.IsActive
                && activityFilter.Contains(x.ActivityID)).ToList();

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
                .ToList();

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

            var r = Require<Reservation>(reservationId);

            r.ActualBeginDateTime = DateTime.Now;
            r.ClientIDBegin = modifiedByClientId.GetValueOrDefault(-1);
            r.IsStarted = true;

            Session.SaveOrUpdate(r);

            // also an entry into history is made
            InsertReservationHistory("Start", "procReservationUpdate", r, modifiedByClientId);
        }

        public IReservationItem UpdateReservation(UpdateReservationArgs args)
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

            return CreateReservationItem(r);
        }

        public void UpdateAccount(int reservationId, int accountId, int? modifiedByClientId)
        {
            var rsv = Session.Get<Reservation>(reservationId);
            rsv.Account = Session.Get<Account>(accountId);
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
                rsv.Notes = notes;

                Session.SaveOrUpdate(rsv);

                // also an entry into history is made for each reservation
                InsertReservationHistory("ByGroupID", "procReservationUpdate", rsv, modifiedByClientId);
            }

            return query.Count;
        }

        public void UpdateCharges(int reservationId, string notes, double chargeMultiplier, bool applyLateChargePenalty, int? modifiedByClientId)
        {
            // procReservationUpdate @Action = 'UpdateCharges'

            //UPDATE dbo.Reservation
            //SET ChargeMultiplier = @ChargeMultiplier,
            //    ApplyLateChargePenalty = @ApplyLateChargePenalty
            //WHERE ReservationID = @ReservationID

            var r = Require<Reservation>(reservationId);

            r.ChargeMultiplier = chargeMultiplier;
            r.ApplyLateChargePenalty = applyLateChargePenalty;
            r.AppendNotes(notes);

            Session.SaveOrUpdate(r);

            // also an entry into history is made
            InsertReservationHistory("UpdateCharges", "procReservationUpdate", r, modifiedByClientId);
        }

        public IReservationItem UpdateFacilityDownTime(int reservationId, DateTime beginDateTime, DateTime endDateTime, int? modifiedByClientId)
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

            var rsv = Require<Reservation>(reservationId);

            if (rsv.Activity.ActivityID != Properties.Current.Activities.FacilityDownTime.ActivityID)
                throw new Exception($"{reservationId} is not a facility downtime reservation.");

            UpdateDuration(rsv, beginDateTime, endDateTime, DateTime.Now);

            // Facility down time must not need to be activated manually by person
            rsv.ActualBeginDateTime = beginDateTime;
            rsv.ActualEndDateTime = endDateTime;

            Session.Update(rsv);

            // also an entry into history is made
            InsertReservationHistory("UpdateFacilityDownTime", "procReservationUpdate", rsv, modifiedByClientId);

            return CreateReservationItem(rsv);
        }

        public IReservation UpdateRepair(int reservationId, DateTime endDateTime, string notes, int? modifiedByClientId)
        {
            var rsv = Require<Reservation>(reservationId);

            if (rsv.Activity.ActivityID != Properties.Current.Activities.Repair.ActivityID)
                throw new Exception($"{reservationId} is not a repair reservation.");

            UpdateDuration(rsv, rsv.BeginDateTime, endDateTime, DateTime.Now);

            Session.Update(rsv);

            InsertReservationHistory("UpdateRepair", "procReservationUpdate", rsv, modifiedByClientId);

            return CreateReservationModel<ReservationInfo>(rsv);
        }

        public void UpdateNotes(int reservationId, string notes)
        {
            var rsv = Session.Get<Reservation>(reservationId);

            rsv.Notes = notes;

            Session.SaveOrUpdate(rsv);

            // there is no reason to add reservation history
            // because notes are not tracked in the ReservationHistory table
        }

        // fka GetTimeUntilNextReservaton
        public AvailableReservationMinutesResult GetAvailableReservationMinutes(IResource res, int reservationId, int clientId, DateTime beginDateTime)
        {
            var dt = DataCommand()
                .Param("Action", "TimeTillNextReservation")
                .Param("ReservationID", reservationId)
                .Param("ResourceID", res.ResourceID)
                .Param("ClientID", clientId)
                .Param("BeginDateTime", beginDateTime)
                .FillDataTable("sselScheduler.dbo.procReservationSelect");

            if (dt.Rows.Count == 0)
                return null;

            var dr = dt.Rows[0];

            var result = new AvailableReservationMinutesResult
            {
                AvailableReservationMinutes = dr.Field<int>("AvailableReservationMinutes"),
                ReservableMinutes = dr.Field<int>("ReservableMinutes"),
                ReservedMinutes = dr.Field<int>("ReservedMinutes"),
                TimeUntilNext = dr.Field<int>("TimeUntilNext"),
                Reason = dr.Field<string>("Reason")
            };

            return result;
        }

        public IReservation GetNextReservation(int resourceId, int reservationId)
        {
            var result = Session.Query<ReservationInfo>()
                .OrderBy(x => x.BeginDateTime)
                .FirstOrDefault(x => x.ResourceID == resourceId
                    && x.IsActive
                    && !x.IsStarted
                    && x.BeginDateTime > DateTime.Now
                    && x.ActualBeginDateTime == null
                    && x.ReservationID != reservationId);

            return result;
        }

        public IEnumerable<IResource> GetResources(IEnumerable<IReservationItem> reservations)
        {
            int[] ids = reservations.Select(x => x.ResourceID).ToArray();
            var result = Session.Query<ResourceInfo>().Where(x => ids.Contains(x.ResourceID));
            return result;
        }

        public SaveReservationHistoryResult SaveReservationHistory(IReservationItem rsv, int accountId, double? forgivenPct, string notes, bool emailClient, int modifiedByClientId)
        {
            // If forgivenPct is null then do not update ChargeMultiplier

            double? chargeMultiplier = null;

            if (forgivenPct.HasValue)
                chargeMultiplier = 1.00 - (forgivenPct / 100.0);

            bool updateBilling = UpdateReservationHistory(new ReservationHistoryUpdate
            {
                ReservationID = rsv.ReservationID,
                AccountID = accountId,
                ChargeMultiplier = chargeMultiplier,
                Notes = notes,
                EmailClient = emailClient,
                ClientID = modifiedByClientId
            });

            var result = new SaveReservationHistoryResult
            {
                ReservationUpdated = true,
                UpdateBilling = updateBilling
            };

            return result;
        }

        public int PurgeReservation(int reservationId)
        {
            string sql = "DELETE FROM sselScheduler.dbo.ReservationHistory WHERE ReservationID = :ReservationID; DELETE FROM sselScheduler.dbo.ReservationProcessInfo WHERE ReservationID = @ReservationID; DELETE FROM sselScheduler.dbo.ReservationInvitee WHERE ReservationID = @ReservationID; DELETE FROM sselScheduler.dbo.Reservation WHERE ReservationID = :ReservationID";

            int result = Session.CreateSQLQuery(sql)
                .SetParameter("ReservationID", reservationId)
                .ExecuteUpdate();

            return result;
        }

        public int PurgeReservations(int resourceId, DateTime sd, DateTime ed)
        {
            string sql;

            sql = "SELECT ReservationID FROM sselScheduler.dbo.Reservation WHERE ResourceID = @resourceId AND (BeginDateTime < @ed AND EndDateTime > @sd)";

            int[] reservationIds = Session.Command(CommandType.Text)
                .Param(new { resourceId, sd, ed })
                .FillDataTable(sql)
                .AsEnumerable()
                .Select(x => x.Field<int>("ReservationID"))
                .ToArray();

            sql = "DELETE FROM sselScheduler.dbo.ReservationHistory WHERE ReservationID IN (:p); DELETE FROM sselScheduler.dbo.ReservationProcessInfo WHERE ReservationID IN (:p); DELETE FROM sselScheduler.dbo.ReservationInvitee WHERE ReservationID IN (:p); DELETE FROM sselScheduler.dbo.Reservation WHERE ReservationID IN (:p)";

            int result = Session.Command(CommandType.Text)
                .ParamList("p", reservationIds)
                .ExecuteNonQuery(sql).Value;

            return result;
        }

        public IReservationRecurrence GetReservationRecurrence(int recurrenceId)
        {
            return Session.Get<ReservationRecurrenceInfo>(recurrenceId);
        }

        public IReservation GetPreviousRecurrence(int recurrenceId, int exclude = 0)
        {
            var result = Session.Query<ReservationInfo>()
                .Where(x => x.RecurrenceID == recurrenceId && x.ReservationID != exclude)
                .OrderByDescending(x => x.BeginDateTime)
                .FirstOrDefault();

            return result;
        }

        public IEnumerable<IReservationRecurrence> GetReservationRecurrencesByResource(int resourceId)
        {
            return Session.Query<ReservationRecurrenceInfo>().Where(x => x.ResourceID == resourceId).ToList();
        }

        public IEnumerable<IReservationRecurrence> GetReservationRecurrencesByProcessTech(int processTechId)
        {
            return Session.Query<ReservationRecurrenceInfo>().Where(x => x.ProcessTechID == processTechId).ToList();
        }

        public IEnumerable<IReservationRecurrence> GetReservationRecurrencesByLabLocation(int labLocationId)
        {
            ReservationRecurrenceInfo recurrence = null;
            ResourceLabLocation location = null;

            var query = Session.QueryOver(() => recurrence)
                .JoinEntityAlias(() => location, () => recurrence.ResourceID == location.ResourceID)
                .Where(() => location.LabLocationID == labLocationId);

            var result = query.List();

            return result;
        }

        public IEnumerable<IReservationRecurrence> GetReservationRecurrencesByClient(int clientId)
        {
            return Session.Query<ReservationRecurrenceInfo>().Where(x => x.ClientID == clientId).ToList();
        }

        public bool SaveReservationRecurrence(int recurrenceId, int patternId, int param1, int? param2, DateTime beginDate, TimeSpan beginTime, double duration, DateTime? endDate, bool autoEnd, bool keepAlive, string notes)
        {
            var rr = Session.Get<ReservationRecurrence>(recurrenceId);

            if (rr == null) return false;

            rr.Pattern = Session.Get<RecurrencePattern>(patternId);
            rr.AutoEnd = autoEnd;
            rr.KeepAlive = keepAlive;
            rr.PatternParam1 = param1;
            rr.PatternParam2 = param2;
            rr.Notes = notes;
            rr.BeginDate = beginDate.Date;
            rr.BeginTime = beginDate.Date.Add(beginTime);
            rr.Duration = duration;
            rr.EndDate = endDate;
            //rr.EndTime = beginDate.Date.Add(endTime);

            Session.SaveOrUpdate(rr);

            return true;
        }

        public int InsertReservationRecurrence(int resourceId, int clientId, int accountId, int activityId, int patternId, int param1, int? param2, DateTime beginDate, DateTime? endDate, DateTime beginTime, double duration, bool autoEnd, bool keepAlive, string notes)
        {
            var rr = new ReservationRecurrence
            {
                Resource = Require<Resource>(resourceId),
                Client = Require<Client>(clientId),
                Account = Require<Account>(accountId),
                Activity = Require<Activity>(activityId),
                Pattern = Require<RecurrencePattern>(patternId),
                PatternParam1 = param1,
                PatternParam2 = param2,
                BeginTime = beginTime,
                BeginDate = beginDate,
                Duration = duration,
                //EndTime = null,
                EndDate = endDate,
                AutoEnd = autoEnd,
                KeepAlive = keepAlive,
                Notes = notes,
                CreatedOn = DateTime.Now,
                IsActive = true
            };

            Session.Save(rr);

            var result = rr.RecurrenceID;

            return result;
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

            return query.ToList();
        }



        public bool UpdateReservationHistory(ReservationHistoryUpdate update)
        {
            //this does not update billing, that needs to be done separately

            // result is true if a billing update is needed, otherwise false

            bool updateAccount = false;
            bool updateChargeMultiplier = false;
            bool saveReservation = false;
            bool addReservationHistory = false;
            bool sendEmail = false;
            double forgivenAmount = 0;

            Reservation main = Require<Reservation>(update.ReservationID);
            Account acct = null;

            // Before we start looping through reservation history, check the main one to see what has changed.
            if (update.AccountID.HasValue && main.Account.AccountID != update.AccountID.Value)
            {
                updateAccount = true;
                acct = Require<Account>(update.AccountID.Value);
            }

            if (update.ChargeMultiplier.HasValue && main.ChargeMultiplier != update.ChargeMultiplier.Value)
                updateChargeMultiplier = true;

            bool updateBilling = updateAccount || updateChargeMultiplier;

            Reservation rsv = main;

            // Now loop through all linked reservations and update them.
            while (true)
            {
                if (updateAccount)
                {
                    rsv.Account = acct;
                    saveReservation = true;
                    addReservationHistory = true;
                }

                if (updateChargeMultiplier)
                {
                    rsv.ChargeMultiplier = update.ChargeMultiplier.Value;
                    saveReservation = true;
                    addReservationHistory = true;
                    sendEmail = update.EmailClient;
                    forgivenAmount = (1 - update.ChargeMultiplier.Value) * 100.0;
                }

                if (update.Notes != null && update.Notes != rsv.Notes)
                {
                    rsv.Notes = update.Notes;
                    saveReservation = true;
                    // no need to add ReservationHistory for a notes change
                }

                if (saveReservation)
                {
                    Session.Update(rsv);

                    if (addReservationHistory)
                    {
                        //need to add ReservationHistory
                        Session.Save(new ReservationHistory()
                        {
                            Reservation = rsv,
                            UserAction = "UpdateReservationHistory",
                            ActionSource = "LNF.WebApi.Scheduler.Repository",
                            Account = rsv.Account,
                            BeginDateTime = rsv.BeginDateTime,
                            EndDateTime = rsv.EndDateTime,
                            ChargeMultiplier = rsv.ChargeMultiplier,
                            ModifiedByClientID = update.ClientID,
                            ModifiedDateTime = DateTime.Now
                        });
                    }
                }

                ReservationHistory rh = Session.Query<ReservationHistory>().FirstOrDefault(x =>
                    x.Reservation == rsv && x.LinkedReservationID != null && x.UserAction == ReservationHistoryItem.INSERT_FOR_MODIFICATION);

                if (rh == null)
                {
                    // no linked reservation found so exit loop
                    break;
                }
                else
                {
                    rsv = Require<Reservation>(rh.LinkedReservationID.Value);
                }
            }

            if (sendEmail)
            {
                Email.EmailOnForgiveCharge(rsv.ReservationID, forgivenAmount, true, rsv.Client.ClientID);
            }

            return updateBilling;
        }

        public IResource GetResource(int reservationId)
        {
            return Session.Get<ReservationInfo>(reservationId);
        }

        public IEnumerable<ReservationStateItem> GetReservationStates(DateTime sd, DateTime ed, string kioskIp, int? clientId = null, int? resourceId = null, int? reserverId = null)
        {
            IEnumerable<IReservationInviteeItem> invitees = Session.Query<ReservationInviteeItem>().ToList();

            IEnumerable<IResourceClient> resourceClients = Session.Query<ResourceClientInfo>().Where(x => x.ResourceID == resourceId.GetValueOrDefault(x.ResourceID)).ToList();

            IEnumerable<IClient> clients = Session.Query<ClientInfo>().ToList();

            var query = Session.Query<ReservationInfo>()
                .Where(x => x.ResourceID == resourceId.GetValueOrDefault(x.ResourceID)
                    && x.ClientID == reserverId.GetValueOrDefault(x.ClientID)
                    && ((x.BeginDateTime < ed && x.EndDateTime > sd) || (x.ActualBeginDateTime.HasValue && x.ActualBeginDateTime < ed && x.ActualEndDateTime.HasValue && x.ActualEndDateTime > sd)))
                .OrderBy(x => x.ResourceName).ThenBy(x => x.BeginDateTime).ThenBy(x => x.EndDateTime);

            IEnumerable<IReservationWithInvitees> items = query.ToList().Select(x => CreateReservationWithInviteesModel(x)).ToList();

            var now = DateTime.Now;

            var util = ReservationStateUtility.Create(now);

            var inlab = PhysicalAccess.Repository.Prowatch.GetCurrentlyInArea("all");
            var onKiosk = Kiosks.Create(Kiosk).IsOnKiosk(kioskIp);
            var access = new PhysicalAccessUtility(inlab, onKiosk);

            return items.Select(x =>
            {
                var item = (IReservationItem)x;

                var reserverUser = clients.First(c => c.ClientID == item.ClientID);

                var currentUser = clients.First(c => c.ClientID == clientId.GetValueOrDefault(item.ClientID));

                var inLab = access.ClientInLab(currentUser.ClientID);

                var args = ReservationStateArgs.Create(x, GetReservationClient(x, currentUser, inLab), now);

                try
                {
                    var state = util.GetReservationState(args);

                    return new ReservationStateItem()
                    {
                        ReservationID = item.ReservationID,
                        ResourceID = item.ResourceID,
                        ResourceName = item.ResourceName,
                        Reserver = reserverUser,
                        CurrentUser = currentUser,
                        State = state,
                        BeginDateTime = item.BeginDateTime,
                        EndDateTime = item.EndDateTime,
                        ActualBeginDateTime = item.ActualBeginDateTime,
                        ActualEndDateTime = item.ActualEndDateTime,
                        IsToolEngineer = args.IsToolEngineer,
                        IsReserver = args.IsReserver,
                        IsInvited = args.IsInvited,
                        IsAuthorized = args.IsAuthorized,
                        BeforeMinCancelTime = args.IsBeforeMinCancelTime(now)
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception($"{ex.Message} ReservationID: {item.ReservationID}, Resource: {item.ResourceName} [{item.ResourceID}], Client: {item.LName}, {item.FName} [{item.ClientID}]");
                }
            });
        }

        public IReservationInviteeItem AddInvitee(int reservationId, int inviteeId)
        {
            var key = new ReservationInvitee()
            {
                Reservation = Require<Reservation>(reservationId),
                Invitee = Require<Client>(inviteeId)
            };

            var ri = FindReservationInvitee(key);

            if (ri == null)
            {
                Session.Save(key);
                return CreateReservationInviteeModel(key);
            }
            else
            {
                return CreateReservationInviteeModel(ri);
            }
        }

        public bool DeleteInvitee(int reservationId, int inviteeId)
        {
            // The invitee might not exist yet if they are added and then removed before the reservation is updated.
            var ri = FindReservationInvitee(reservationId, inviteeId);

            if (ri != null)
            {
                Session.Delete(ri);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool InviteeExists(int reservationId, int inviteeId)
        {
            var invitee = FindReservationInvitee(reservationId, inviteeId);
            return invitee != null;
        }

        public IEnumerable<IReservationInviteeItem> GetInvitees(int reservationId)
        {
            return Session.Query<ReservationInviteeItem>().Where(x => x.ReservationID == reservationId).ToList();
        }

        public IEnumerable<IReservationInviteeItem> GetInvitees(int[] reservations)
        {
            return Session.Query<ReservationInviteeItem>().Where(x => reservations.Contains(x.ReservationID)).ToList();
        }

        public IReservationInviteeItem GetInvitee(int reservationId, int inviteeId)
        {
            return CreateReservationInviteeModel(FindReservationInvitee(reservationId, inviteeId));
        }

        public IEnumerable<AvailableInvitee> GetAvailableInvitees(int reservationId, int resourceId, int activityId, int clientId)
        {
            var dt = Session.Command()
                .Param("Action", "SelectAvailInvitees")
                .Param("ReservationID", reservationId)
                .Param("ResourceID", resourceId)
                .Param("ActivityID", activityId)
                .Param("ClientID", clientId)
                .FillDataTable("sselScheduler.dbo.procReservationInviteeSelect");

            var result = dt.AsEnumerable().Select(x => new AvailableInvitee
            {
                ClientID = x.Field<int>("ClientID"),
                LName = x.Field<string>("LName"),
                FName = x.Field<string>("FName")
            }).ToList();

            return result;
        }

        public IEnumerable<IReservationInviteeItem> SelectInviteesByResource(int resourceId, DateTime sd, DateTime ed, bool includeDeleted)
        {
            object active = DBNull.Value;
            if (!includeDeleted)
                active = true;

            var dt = DataCommand()
                .Param("Action", "SelectInviteesByResource")
                .Param("ResourceID", resourceId)
                .Param("StartDate", sd)
                .Param("EndDate", ed)
                .Param("IsActive", active)
                .FillDataTable("sselScheduler.dbo.procReservationInviteeItemSelect");

            var result = CreateReservationInviteeModels(dt);

            return result;
        }

        public IEnumerable<IReservationInviteeItem> SelectInviteesByProcessTech(int procTechId, DateTime sd, DateTime ed, bool includeDeleted)
        {
            object active = DBNull.Value;
            if (!includeDeleted)
                active = true;

            var dt = DataCommand()
                .Param("Action", "SelectInviteesByProcessTech")
                .Param("ProcessTechID", procTechId)
                .Param("StartDate", sd)
                .Param("EndDate", ed)
                .Param("IsActive", active)
                .FillDataTable("sselScheduler.dbo.procReservationInviteeItemSelect");

            var result = CreateReservationInviteeModels(dt);

            return result;
        }

        public IEnumerable<IReservationInviteeItem> SelectInviteesByLabLocation(int labLocationId, DateTime sd, DateTime ed, bool includeDeleted)
        {
            object active = DBNull.Value;
            if (!includeDeleted)
                active = true;

            var dt = DataCommand()
                .Param("Action", "SelectInviteesByLabLocation")
                .Param("LabLocationID", labLocationId)
                .Param("StartDate", sd)
                .Param("EndDate", ed)
                .Param("IsActive", active)
                .FillDataTable("sselScheduler.dbo.procReservationInviteeItemSelect");

            var result = CreateReservationInviteeModels(dt);

            return result;
        }

        public IEnumerable<IReservationInviteeItem> SelectInviteesByClient(int clientId, DateTime sd, DateTime ed, bool includeDeleted)
        {
            object active = DBNull.Value;
            if (!includeDeleted)
                active = true;

            var dt = DataCommand()
                .Param("Action", "SelectInviteesByClient")
                .Param("ClientID", clientId)
                .Param("StartDate", sd)
                .Param("EndDate", ed)
                .Param("IsActive", active)
                .FillDataTable("sselScheduler.dbo.procReservationInviteeItemSelect");

            var result = CreateReservationInviteeModels(dt);

            return result;
        }

        public IEnumerable<IReservationInviteeItem> SelectInviteesByDateRange(DateTime sd, DateTime ed, bool includeDeleted)
        {
            object active = DBNull.Value;
            if (!includeDeleted)
                active = true;

            var dt = DataCommand()
                .Param("Action", "SelectInviteesByDateRange")
                .Param("StartDate", sd)
                .Param("EndDate", ed)
                .Param("IsActive", active)
                .FillDataTable("sselScheduler.dbo.procReservationInviteeItemSelect");

            var result = CreateReservationInviteeModels(dt);

            return result;
        }

        public int[] FilterInvitedReservations(int[] reservationIds, int clientId)
        {
            var query = Session.Query<ReservationInviteeItem>().Where(x => reservationIds.Contains(x.ReservationID) && x.InviteeID == clientId);
            var result = query.Select(x => x.ReservationID).ToArray();
            return result;
        }

        public void ExtendReservation(int reservationId, int totalMinutes, int? modifiedByClientId)
        {
            var rsv = Require<Reservation>(reservationId);
            DateTime beginDateTime = rsv.BeginDateTime;
            DateTime endDateTime = rsv.EndDateTime.AddMinutes(totalMinutes);
            UpdateDuration(rsv, beginDateTime, endDateTime, DateTime.Now);
            Session.SaveOrUpdate(rsv);
            InsertReservationHistory("ExtendReservation", "LNF.Impl.Scheduler.ReservationManager", rsv, modifiedByClientId);
        }

        public IEnumerable<RecentReservation> SelectRecentReservations(int resourceId)
        {
            // procReservationSelect @Action = 'SelectRecent'

            //SELECT TOP 6 Rv.ReservationID, Rv.BeginDateTime, Rv.EndDateTime,
            //  Rv.ClientID, sselData.dbo.udf_GetDisplayName(RV.ClientID) AS DisplayName
            //FROM dbo.Reservation Rv
            //WHERE Rv.ResourceID = @ResourceID
            //ORDER BY ABS (datediff (second, Rv.BeginDateTime, getdate())) ASC

            // Need to use criterion because of the complicated order by clause.
            var dt = DataCommand()
                .Param("Action", "SelectRecent")
                .Param("ResourceID", resourceId)
                .FillDataTable("sselScheduler.dbo.procReservationSelect");

            return CreateRecentReservations(dt);
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

            Session.Save(result);

            return result;
        }

        private Reservation PrivateUpdate(UpdateReservationArgs args)
        {
            var r = Session.Get<Reservation>(args.ReservationID);

            if (r == null)
                throw new Exception($"Cannot find Reservation with ReservationID = {args.ReservationID}");

            r.Account = Session.Get<Account>(args.AccountID);
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
            var res = Session.Get<Resource>(args.ResourceID);
            var client = Session.Get<Client>(args.ClientID);
            var account = Session.Get<Account>(args.AccountID);
            var activity = Session.Get<Activity>(args.ActivityID);

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

            Session.Save(r);

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
                    DateTime granStartTime = res.GetNextGranularity(args.Now, GranularityDirection.Previous);

                    if (args.BeginDateTime < granStartTime)
                    {
                        string body = $"Unable to create a reservation. BeginDateTime is in the past.\n--------------------\nClient: {client.DisplayName} [{client.ClientID}]\nResource: {res.ResourceName} [{res.ResourceID}]\nBeginDateTime: {args.BeginDateTime:yyyy-MM-dd HH:mm:ss}\nEndDateTime: {args.EndDateTime:yyyy-MM-dd HH:mm:ss}\nModifiedByClientID: {args.ModifiedByClientID}";
                        SendEmail.SendDeveloperEmail("LNF.Impl.Repository.Scheduler.Reservation.CanCreateCheck", "Create reservation failed", body);
                        throw new Exception("Your reservation was not created. Cannot create a reservation in the past.");
                    }
                }
            }

            if (args.EndDateTime <= args.BeginDateTime)
            {
                string body = $"Unable to create a reservation. EndDateTime is before BeginDateTime.\n--------------------\nClient: {client.DisplayName} [{client.ClientID}]\nResource: {res.ResourceName} [{res.ResourceID}]\nBeginDateTime: {args.BeginDateTime:yyyy-MM-dd HH:mm:ss}\nEndDateTime: {args.EndDateTime:yyyy-MM-dd HH:mm:ss}\nModifiedByClientID: {args.ModifiedByClientID}";
                SendEmail.SendDeveloperEmail("LNF.Impl.Repository.Scheduler.Reservation.CanCreateCheck", "Create reservation failed", body);
                throw new Exception("Your reservation was not created. Cannot create a reservation that ends before it starts.");
            }

            // conflicting reservations must be:
            //      1) same resource
            //      2) not canceled
            //      3) date ranges overlap

            // [2020-08-12 jg]
            // We must use Reservation, not ReservationInfo, here for possible conflicts that were previously overwritten when a tool engineer
            // is extending a scheduled maintenance (for example). If we query Reservation IsActive will be false, but ReservationInfo will still
            // be true until the transaction is committed.
            var conflict = Session.Query<Reservation>()
                .Where(x => x.Resource == res && x.IsActive && (x.BeginDateTime < args.EndDateTime && x.EndDateTime > args.BeginDateTime))
                .FirstOrDefault(x => !x.ActualEndDateTime.HasValue && x.ReservationID != args.LinkedReservationID);

            if (conflict != null)
            {
                string body = $"Unable to create a reservation. There is a conflict with an existing reservation.\n--------------------\nClient: {client.DisplayName} {client.ClientID}\nResource: {res.ResourceName} [{res.ResourceID}]\nBeginDateTime: {args.BeginDateTime:yyyy-MM-dd HH:mm:ss}\nEndDateTime: {args.EndDateTime:yyyy-MM-dd HH:mm:ss}\nConflictingReservationID: {conflict.ReservationID}\nModifiedByClientID: {args.ModifiedByClientID}";
                SendEmail.SendDeveloperEmail("LNF.Impl.Repository.Scheduler.Reservation.CanCreateCheck", "Create reservation failed", body);
                throw new Exception(string.Format("Your reservation was not created. There is a conflict with an existing reservation [#{0}].", conflict.ReservationID));
            }
        }

        private void UpdateDuration(Reservation rsv, DateTime beginDateTime, DateTime endDateTime, DateTime now)
        {
            if (rsv.OriginalBeginDateTime == null)
                rsv.OriginalBeginDateTime = rsv.BeginDateTime;

            if (rsv.OriginalEndDateTime == null)
                rsv.OriginalEndDateTime = rsv.EndDateTime;

            if (rsv.OriginalModifiedOn == null)
                rsv.OriginalModifiedOn = rsv.LastModifiedOn;

            rsv.LastModifiedOn = now;
            rsv.BeginDateTime = beginDateTime;
            rsv.EndDateTime = endDateTime;
            rsv.Duration = (endDateTime - beginDateTime).TotalMinutes;
            rsv.MaxReservedDuration = Math.Max(rsv.Duration, rsv.MaxReservedDuration);
        }

        private IQueryable<ReservationInfo> ReservationInfoQuery(DateTime sd, DateTime ed, int clientId = 0, int resourceId = 0, int activityId = 0, bool? started = null, bool? active = null)
        {
            var query = Session.Query<ReservationInfo>().Where(x =>
                    (x.BeginDateTime < ed && x.EndDateTime > sd)
                    || ((x.ActualBeginDateTime.HasValue && x.ActualBeginDateTime.Value < ed) && (x.ActualEndDateTime.HasValue && x.ActualEndDateTime.Value > sd)));

            if (clientId != 0)
                query = query.Where(x => x.ClientID == clientId);

            if (resourceId != 0)
                query = query.Where(x => x.ResourceID == resourceId);

            if (activityId != 0)
                query = query.Where(x => x.ActivityID == activityId);

            if (started.HasValue)
                query = query.Where(x => x.IsStarted == started.Value);

            if (active.HasValue)
                query = query.Where(x => x.IsActive == active.Value);

            return query;
        }

        private ReservationClient GetReservationClient(IReservationWithInvitees rsv, IClient client, bool inLab)
        {
            var item = (IReservationItem)rsv;
            var resourceClients = GetResourceClients(item.ResourceID);
            var userAuth = Reservations.GetAuthLevel(resourceClients, client);

            var result = new ReservationClient
            {
                ClientID = client.ClientID,
                ReservationID = item.ReservationID,
                ResourceID = item.ResourceID,
                IsReserver = item.ClientID == client.ClientID,
                IsInvited = rsv.Invitees.Any(x => x.InviteeID == client.ClientID),
                InLab = inLab,
                UserAuth = userAuth
            };

            return result;
        }

        private ReservationInvitee FindReservationInvitee(int reservationId, int inviteeId)
        {
            var r = Session.Get<Reservation>(reservationId);
            var i = Session.Get<Client>(inviteeId);
            var key = new ReservationInvitee { Reservation = r, Invitee = i };
            return FindReservationInvitee(key);
        }

        private ReservationInvitee FindReservationInvitee(ReservationInvitee key)
        {
            var result = Session.Get<ReservationInvitee>(key);
            return result;
        }

        private IEnumerable<IReservationItem> ByClientQuery(int clientId, DateTime sd, DateTime ed, bool includeDeleted)
        {
            object active = DBNull.Value;
            if (!includeDeleted)
                active = true;

            var dt = DataCommand()
                .Param("Action", "SelectByClient")
                .Param("ClientID", clientId)
                .Param("StartDate", sd)
                .Param("EndDate", ed)
                .Param("IsActive", active)
                .FillDataTable("sselScheduler.dbo.procReservationItemSelect");

            var result = CreateReservationItems(dt);

            return result;
        }

        private IEnumerable<IReservationItem> ByInviteeQuery(int inviteeId, DateTime sd, DateTime ed, bool includeDeleted)
        {
            object active = DBNull.Value;
            if (!includeDeleted)
                active = true;

            var dt = DataCommand()
                .Param("Action", "SelectByInvitee")
                .Param("InviteeID", inviteeId)
                .Param("StartDate", sd)
                .Param("EndDate", ed)
                .Param("IsActive", active)
                .FillDataTable("sselScheduler.dbo.procReservationItemSelect");

            var result = CreateReservationItems(dt);

            return result;
        }

        public IReservationGroup UpdateReservationGroup(int groupId, DateTime beginDateTime, DateTime endDateTime)
        {
            // This is all that happens in procReservationGroupUpdate @Action = 'ByGroupID'

            //UPDATE [sselScheduler].[dbo].[ReservationGroup]
            //SET [BeginDateTime] = @BeginDateTime
            //    ,[EndDateTime] = @EndDateTime
            //WHERE GroupID = @GroupID

            ReservationGroup rg = Require<ReservationGroup>(groupId);
            rg.BeginDateTime = beginDateTime;
            rg.EndDateTime = endDateTime;
            Session.Update(rg);

            return CreateReservationGroupModel(rg);
        }

        public IAutoEndLog AddAutoEndLog(int reservationId, string action)
        {
            IReservation rsv = GetReservation(reservationId);

            var entry = new AutoEndLog()
            {
                ReservationID = rsv.ReservationID,
                ResourceID = rsv.ResourceID,
                ResourceName = rsv.ResourceName,
                ClientID = rsv.ClientID,
                DisplayName = rsv.DisplayName,
                Timestamp = DateTime.Now,
                Action = action
            };

            Session.Save(entry);

            return CreateAutoEndLogModel(entry);
        }

        private IReservationItem CreateReservationItem(Reservation rsv)
        {
            var result = new ReservationItem
            {
                ReservationID = rsv.ReservationID,
                ResourceID = rsv.Resource.ResourceID,
                ResourceName = rsv.Resource.ResourceName,
                Granularity = rsv.Resource.Granularity,
                Offset = rsv.Resource.Offset,
                MinReservTime = rsv.Resource.MinReservTime,
                MaxReservTime = rsv.Resource.MaxReservTime,
                MinCancelTime = rsv.Resource.MinCancelTime,
                GracePeriod = rsv.Resource.GracePeriod,
                ReservFence = rsv.Resource.ReservFence,
                AuthDuration = rsv.Resource.AuthDuration,
                AuthState = rsv.Resource.AuthState,
                ResourceAutoEnd = rsv.Resource.AutoEnd,
                ProcessTechID = rsv.Resource.ProcessTech.ProcessTechID,
                LabID = rsv.Resource.ProcessTech.Lab.LabID,
                BuildingID = rsv.Resource.ProcessTech.Lab.Building.BuildingID,
                ActivityID = rsv.Activity.ActivityID,
                ActivityName = rsv.Activity.ActivityName,
                Editable = rsv.Activity.Editable,
                IsFacilityDownTime = rsv.Activity.IsFacilityDownTime,
                ActivityAccountType = rsv.Activity.AccountType,
                StartEndAuth = rsv.Activity.StartEndAuth,
                ClientID = rsv.Client.ClientID,
                UserName = rsv.Client.UserName,
                Privs = rsv.Client.Privs,
                Email = rsv.GetEmail(Session),
                Phone = rsv.GetPhone(Session),
                LName = rsv.Client.LName,
                MName = rsv.Client.MName,
                FName = rsv.Client.FName,
                RecurrenceID = rsv.RecurrenceID,
                GroupID = rsv.GroupID,
                ClientIDBegin = rsv.ClientIDBegin,
                ClientIDEnd = rsv.ClientIDEnd,
                AccountID = rsv.Account.AccountID,
                AccountName = rsv.Account.Name,
                ShortCode = rsv.Account.ShortCode,
                ChargeTypeID = rsv.Account.Org.OrgType.ChargeType.ChargeTypeID,
                BeginDateTime = rsv.BeginDateTime,
                EndDateTime = rsv.EndDateTime,
                ActualBeginDateTime = rsv.ActualBeginDateTime,
                ActualEndDateTime = rsv.ActualEndDateTime,
                Duration = rsv.Duration,
                ChargeMultiplier = rsv.ChargeMultiplier,
                ApplyLateChargePenalty = rsv.ApplyLateChargePenalty,
                ReservationAutoEnd = rsv.AutoEnd,
                IsActive = rsv.IsActive,
                IsStarted = rsv.IsStarted,
                KeepAlive = rsv.KeepAlive,
                Notes = rsv.Notes,
                CreatedOn = rsv.CreatedOn,
                LastModifiedOn = rsv.LastModifiedOn,
                CancelledDateTime = rsv.CancelledDateTime
            };

            return result;
        }

        private IEnumerable<RecentReservation> CreateRecentReservations(DataTable dt)
        {
            var result = new List<RecentReservation>();
            foreach (DataRow dr in dt.Rows)
            {
                result.Add(CreateRecentReservation(dr));
            }
            return result;
        }

        private RecentReservation CreateRecentReservation(DataRow dr)
        {
            if (dr == null) return null;

            return new RecentReservation
            {
                ReservationID = dr.Field<int>("ReservationID"),
                BeginDateTime = dr.Field<DateTime>("BeginDateTime"),
                EndDateTime = dr.Field<DateTime>("EndDateTime"),
                ClientID = dr.Field<int>("ClientID"),
                DisplayName = dr.Field<string>("DisplayName")
            };
        }

        private IEnumerable<IReservationItem> CreateReservationItems(DataTable dt)
        {
            IList<IReservationItem> result = new List<IReservationItem>();
            foreach (DataRow dr in dt.Rows)
            {
                var item = CreateReservationItem(dr);
                result.Add(item);
            }
            return result;
        }

        private IReservationItem CreateReservationItem(DataRow dr)
        {
            var result = new ReservationItem
            {
                ReservationID = dr.Field<int>("ReservationID"),
                ResourceID = dr.Field<int>("ResourceID"),
                ResourceName = dr.Field<string>("ResourceName"),
                Granularity = dr.Field<int>("Granularity"),
                Offset = dr.Field<int>("Offset"),
                MinReservTime = dr.Field<int>("MinReservTime"),
                MaxReservTime = dr.Field<int>("MaxReservTime"),
                MinCancelTime = dr.Field<int>("MinCancelTime"),
                GracePeriod = dr.Field<int>("GracePeriod"),
                ReservFence = dr.Field<int>("ReservFence"),
                AuthDuration = dr.Field<int>("AuthDuration"),
                AuthState = dr.Field<bool>("AuthState"),
                ResourceAutoEnd = dr.Field<int>("ResourceAutoEnd"),
                ProcessTechID = dr.Field<int>("ProcessTechID"),
                LabID = dr.Field<int>("LabID"),
                BuildingID = dr.Field<int>("BuildingID"),
                ActivityID = dr.Field<int>("ActivityID"),
                ActivityName = dr.Field<string>("ActivityName"),
                Editable = dr.Field<bool>("Editable"),
                IsFacilityDownTime = dr.Field<bool>("IsFacilityDownTime"),
                ActivityAccountType = dr.Field<ActivityAccountType>("ActivityAccountType"),
                StartEndAuth = dr.Field<ClientAuthLevel>("StartEndAuth"),
                ClientID = dr.Field<int>("ClientID"),
                UserName = dr.Field<string>("UserName"),
                Privs = dr.Field<ClientPrivilege>("Privs"),
                Email = dr.Field<string>("Email"),
                Phone = dr.Field<string>("Phone"),
                LName = dr.Field<string>("LName"),
                MName = dr.Field<string>("MName"),
                FName = dr.Field<string>("FName"),
                AccountID = dr.Field<int>("AccountID"),
                AccountName = dr.Field<string>("AccountName"),
                ShortCode = dr.Field<string>("ShortCode"),
                ChargeTypeID = dr.Field<int>("ChargeTypeID"),
                RecurrenceID = dr.Field<int?>("RecurrenceID"),
                GroupID = dr.Field<int?>("GroupID"),
                ClientIDBegin = dr.Field<int?>("ClientIDBegin"),
                ClientIDEnd = dr.Field<int?>("ClientIDEnd"),
                BeginDateTime = dr.Field<DateTime>("BeginDateTime"),
                EndDateTime = dr.Field<DateTime>("EndDateTime"),
                ActualBeginDateTime = dr.Field<DateTime?>("ActualBeginDateTime"),
                ActualEndDateTime = dr.Field<DateTime?>("ActualEndDateTime"),
                Duration = dr.Field<double>("Duration"),
                ChargeMultiplier = dr.Field<double>("ChargeMultiplier"),
                ApplyLateChargePenalty = dr.Field<bool>("ApplyLateChargePenalty"),
                ReservationAutoEnd = dr.Field<bool>("ReservationAutoEnd"),
                HasProcessInfo = dr.Field<bool>("HasProcessInfo"),
                HasInvitees = dr.Field<bool>("HasInvitees"),
                IsActive = dr.Field<bool>("IsActive"),
                IsStarted = dr.Field<bool>("IsStarted"),
                KeepAlive = dr.Field<bool>("KeepAlive"),
                Notes = dr.Field<string>("Notes"),
                CreatedOn = dr.Field<DateTime>("CreatedOn"),
                LastModifiedOn = dr.Field<DateTime>("LastModifiedOn"),
                CancelledDateTime = dr.Field<DateTime?>("CancelledDateTime")
            };

            return result;
        }

        private T CreateReservationModel<T>(Reservation rsv) where T : ReservationInfoBase, new()
        {
            if (rsv == null) return null;

            // This is weird because ReservationInfo is also a persisted entity in NHiberante, but it is read-only.
            // Hopefully this won't cause any issues.

            ClientOrgInfo co = Session.Query<ClientOrgInfo>().FirstOrDefault(x => x.ClientID == rsv.Client.ClientID && x.OrgID == rsv.Account.Org.OrgID);
            if (co == null)
            {
                // This happens when there is a remote reservation because the client and account don't match.
                // In this case use the "primary" ClientOrg.
                co = Session.Query<ClientOrgInfo>().FirstOrDefault(x => x.ClientID == rsv.Client.ClientID && x.EmailRank == 1);
            }

            return new T()
            {
                ReservationID = rsv.ReservationID,

                ResourceID = rsv.Resource.ResourceID,
                ResourceName = rsv.Resource.ResourceName,
                ResourceDescription = rsv.Resource.Description,
                AuthDuration = rsv.Resource.AuthDuration,
                AuthState = rsv.Resource.AuthState,
                GracePeriod = rsv.Resource.GracePeriod,
                Granularity = rsv.Resource.Granularity,
                HelpdeskEmail = rsv.Resource.HelpdeskEmail,
                WikiPageUrl = rsv.Resource.WikiPageUrl,
                IsReady = rsv.Resource.IsReady,
                IsSchedulable = rsv.Resource.IsSchedulable,
                MaxAlloc = rsv.Resource.MaxAlloc,
                MinReservTime = rsv.Resource.MinReservTime,
                MaxReservTime = rsv.Resource.MaxReservTime,
                MinCancelTime = rsv.Resource.MinCancelTime,
                Offset = rsv.Resource.Offset,
                OTFSchedTime = rsv.Resource.OTFSchedTime,
                ReservFence = rsv.Resource.ReservFence,
                ResourceAutoEnd = rsv.Resource.AutoEnd,
                ResourceIsActive = rsv.Resource.IsActive,
                State = rsv.Resource.State,
                StateNotes = rsv.Resource.StateNotes,
                UnloadTime = rsv.Resource.UnloadTime,

                ProcessTechID = rsv.Resource.ProcessTech.ProcessTechID,
                ProcessTechName = rsv.Resource.ProcessTech.ProcessTechName,
                ProcessTechDescription = rsv.Resource.ProcessTech.Description,
                ProcessTechIsActive = rsv.Resource.ProcessTech.IsActive,

                ProcessTechGroupID = rsv.Resource.ProcessTech.Group.ProcessTechGroupID,
                ProcessTechGroupName = rsv.Resource.ProcessTech.Group.GroupName,

                LabID = rsv.Resource.ProcessTech.Lab.LabID,
                LabName = rsv.Resource.ProcessTech.Lab.LabName,
                LabDisplayName = rsv.Resource.ProcessTech.Lab.DisplayName,
                LabDescription = rsv.Resource.ProcessTech.Lab.Description,
                LabIsActive = rsv.Resource.ProcessTech.Lab.IsActive,

                BuildingID = rsv.Resource.ProcessTech.Lab.Building.BuildingID,
                BuildingName = rsv.Resource.ProcessTech.Lab.Building.BuildingName,
                BuildingDescription = rsv.Resource.ProcessTech.Lab.Building.BuildingDescription,
                BuildingIsActive = rsv.Resource.ProcessTech.Lab.Building.BuildingIsActive,

                AccountID = rsv.Account.AccountID,
                AccountName = rsv.Account.Name,
                ShortCode = rsv.Account.ShortCode,

                ClientID = rsv.Client.ClientID,
                UserName = rsv.Client.UserName,
                FName = rsv.Client.FName,
                MName = rsv.Client.MName,
                LName = rsv.Client.LName,
                Privs = rsv.Client.Privs,

                ClientOrgID = co.ClientOrgID,
                ClientOrgActive = co.ClientOrgActive,
                ClientAddressID = co.ClientAddressID,
                Email = co.Email,
                Phone = co.Phone,
                IsManager = co.IsManager,
                IsFinManager = co.IsFinManager,
                SubsidyStartDate = co.SubsidyStartDate,
                NewFacultyStartDate = co.NewFacultyStartDate,
                ChargeTypeID = rsv.Account.Org.OrgType.ChargeType.ChargeTypeID,

                ActivityID = rsv.Activity.ActivityID,
                ActivityName = rsv.Activity.ActivityName,
                ActivityAccountType = rsv.Activity.AccountType,
                StartEndAuth = rsv.Activity.StartEndAuth,
                Editable = rsv.Activity.Editable,
                IsFacilityDownTime = rsv.Activity.IsFacilityDownTime,

                BeginDateTime = rsv.BeginDateTime,
                EndDateTime = rsv.EndDateTime,
                ActualBeginDateTime = rsv.ActualBeginDateTime,
                ActualEndDateTime = rsv.ActualEndDateTime,
                CancelledDateTime = rsv.CancelledDateTime,
                ChargeMultiplier = rsv.ChargeMultiplier,
                ClientIDBegin = rsv.ClientIDBegin,
                ClientIDEnd = rsv.ClientIDEnd,
                CreatedOn = rsv.CreatedOn,
                LastModifiedOn = rsv.LastModifiedOn,
                Duration = rsv.Duration,
                HasInvitees = rsv.HasInvitees,
                HasProcessInfo = rsv.HasProcessInfo,
                IsActive = rsv.IsActive,
                IsStarted = rsv.IsStarted,
                GroupID = rsv.GroupID,
                RecurrenceID = rsv.RecurrenceID,
                IsUnloaded = rsv.IsUnloaded,
                KeepAlive = rsv.KeepAlive,
                ApplyLateChargePenalty = rsv.ApplyLateChargePenalty,
                ReservationAutoEnd = rsv.AutoEnd,
                MaxReservedDuration = rsv.MaxReservedDuration,
                Notes = rsv.Notes,
                OriginalBeginDateTime = rsv.OriginalBeginDateTime,
                OriginalEndDateTime = rsv.OriginalEndDateTime,
                OriginalModifiedOn = rsv.OriginalModifiedOn
            };
        }

        private IEnumerable<IReservationInviteeItem> CreateReservationInviteeModels(DataTable dt)
        {
            List<IReservationInviteeItem> result = new List<IReservationInviteeItem>();

            foreach (DataRow dr in dt.Rows)
            {
                result.Add(CreateReservationInviteeModel(dr));
            }

            return result;
        }

        private IReservationInviteeItem CreateReservationInviteeModel(DataRow dr)
        {
            if (dr == null) return null;

            return new ReservationInviteeItem
            {
                InviteeID = dr.Field<int>("InviteeID"),
                ReservationID = dr.Field<int>("ReservationID"),
                BeginDateTime = dr.Field<DateTime>("BeginDateTime"),
                EndDateTime = dr.Field<DateTime>("EndDateTime"),
                ActualBeginDateTime = dr.Field<DateTime?>("ActualBeginDateTime"),
                ActualEndDateTime = dr.Field<DateTime?>("ActualEndDateTime"),
                IsStarted = dr.Field<bool>("IsStarted"),
                IsActive = dr.Field<bool>("IsActive"),
                ResourceID = dr.Field<int>("ResourceID"),
                ProcessTechID = dr.Field<int>("ProcessTechID"),
                InviteeActive = dr.Field<bool>("InviteeActive"),
                InviteeFName = dr.Field<string>("InviteeFName"),
                InviteeLName = dr.Field<string>("InviteeLName"),
                InviteePrivs = dr.Field<ClientPrivilege>("InviteePrivs")
            };
        }

        private IReservationInviteeItem CreateReservationInviteeModel(ReservationInvitee inv)
        {
            if (inv == null) return null;

            return new ReservationInviteeItem
            {
                InviteeID = inv.Invitee.ClientID,
                ReservationID = inv.Reservation.ReservationID,
                BeginDateTime = inv.Reservation.BeginDateTime,
                EndDateTime = inv.Reservation.EndDateTime,
                ActualBeginDateTime = inv.Reservation.ActualBeginDateTime,
                ActualEndDateTime = inv.Reservation.ActualEndDateTime,
                IsStarted = inv.Reservation.IsStarted,
                IsActive = inv.Reservation.IsActive,
                ResourceID = inv.Reservation.Resource.ResourceID,
                ProcessTechID = inv.Reservation.Resource.ProcessTech.ProcessTechID,
                InviteeActive = inv.Invitee.Active,
                InviteeFName = inv.Invitee.FName,
                InviteeLName = inv.Invitee.LName,
                InviteePrivs = inv.Invitee.Privs
            };
        }

        private IReservationWithInvitees CreateReservationWithInviteesModel(ReservationInfo rsv)
        {
            if (rsv == null) return null;

            return new ReservationWithInvitees
            {
                ReservationID = rsv.ReservationID,

                ResourceID = rsv.ResourceID,
                ResourceName = rsv.ResourceName,
                ResourceDescription = rsv.ResourceDescription,
                AuthDuration = rsv.AuthDuration,
                AuthState = rsv.AuthState,
                GracePeriod = rsv.GracePeriod,
                Granularity = rsv.Granularity,
                HelpdeskEmail = rsv.HelpdeskEmail,
                WikiPageUrl = rsv.WikiPageUrl,
                IsReady = rsv.IsReady,
                IsSchedulable = rsv.IsSchedulable,
                MaxAlloc = rsv.MaxAlloc,
                MinReservTime = rsv.MinReservTime,
                MaxReservTime = rsv.MaxReservTime,
                MinCancelTime = rsv.MinCancelTime,
                Offset = rsv.Offset,
                OTFSchedTime = rsv.OTFSchedTime,
                ReservFence = rsv.ReservFence,
                ResourceAutoEnd = rsv.ResourceAutoEnd,
                ResourceIsActive = rsv.IsActive,
                State = rsv.State,
                StateNotes = rsv.StateNotes,
                UnloadTime = rsv.UnloadTime,

                ProcessTechID = rsv.ProcessTechID,
                ProcessTechName = rsv.ProcessTechName,
                ProcessTechDescription = rsv.ProcessTechDescription,
                ProcessTechIsActive = rsv.ProcessTechIsActive,

                ProcessTechGroupID = rsv.ProcessTechGroupID,
                ProcessTechGroupName = rsv.ProcessTechGroupName,

                LabID = rsv.LabID,
                LabName = rsv.LabName,
                LabDisplayName = rsv.LabDisplayName,
                LabDescription = rsv.LabDescription,
                LabIsActive = rsv.IsActive,

                BuildingID = rsv.BuildingID,
                BuildingName = rsv.BuildingName,
                BuildingDescription = rsv.BuildingDescription,
                BuildingIsActive = rsv.BuildingIsActive,

                AccountID = rsv.AccountID,
                AccountName = rsv.AccountName,
                ShortCode = rsv.ShortCode,

                ClientID = rsv.ClientID,
                UserName = rsv.UserName,
                FName = rsv.FName,
                MName = rsv.MName,
                LName = rsv.LName,
                Privs = rsv.Privs,

                ClientOrgID = rsv.ClientOrgID,
                ClientOrgActive = rsv.ClientOrgActive,
                ClientAddressID = rsv.ClientAddressID,
                Email = rsv.Email,
                Phone = rsv.Phone,
                IsManager = rsv.IsManager,
                IsFinManager = rsv.IsFinManager,
                SubsidyStartDate = rsv.SubsidyStartDate,
                NewFacultyStartDate = rsv.NewFacultyStartDate,
                ChargeTypeID = rsv.ChargeTypeID,

                ActivityID = rsv.ActivityID,
                ActivityName = rsv.ActivityName,
                ActivityAccountType = rsv.ActivityAccountType,
                StartEndAuth = rsv.StartEndAuth,
                Editable = rsv.Editable,
                IsFacilityDownTime = rsv.IsFacilityDownTime,

                BeginDateTime = rsv.BeginDateTime,
                EndDateTime = rsv.EndDateTime,
                ActualBeginDateTime = rsv.ActualBeginDateTime,
                ActualEndDateTime = rsv.ActualEndDateTime,
                CancelledDateTime = rsv.CancelledDateTime,
                ChargeMultiplier = rsv.ChargeMultiplier,
                ClientIDBegin = rsv.ClientIDBegin,
                ClientIDEnd = rsv.ClientIDEnd,
                CreatedOn = rsv.CreatedOn,
                LastModifiedOn = rsv.LastModifiedOn,
                Duration = rsv.Duration,
                HasInvitees = rsv.HasInvitees,
                HasProcessInfo = rsv.HasProcessInfo,
                IsActive = rsv.IsActive,
                IsStarted = rsv.IsStarted,
                GroupID = rsv.GroupID,
                RecurrenceID = rsv.RecurrenceID,
                IsUnloaded = rsv.IsUnloaded,
                KeepAlive = rsv.KeepAlive,
                ApplyLateChargePenalty = rsv.ApplyLateChargePenalty,
                ReservationAutoEnd = rsv.ReservationAutoEnd,
                MaxReservedDuration = rsv.MaxReservedDuration,
                Notes = rsv.Notes,
                OriginalBeginDateTime = rsv.OriginalBeginDateTime,
                OriginalEndDateTime = rsv.OriginalEndDateTime,
                OriginalModifiedOn = rsv.OriginalModifiedOn,

                Invitees = GetInvitees(rsv.ReservationID)
            };
        }

        private IReservationGroup CreateReservationGroupModel(ReservationGroup rg)
        {
            if (rg == null) return null;

            return new ReservationGroupItem
            {
                GroupID = rg.GroupID,
                ClientID = rg.Client.ClientID,
                AccountID = rg.Account.AccountID,
                ActivityID = rg.Activity.ActivityID,
                BeginDateTime = rg.BeginDateTime,
                EndDateTime = rg.EndDateTime,
                CreatedOn = rg.CreatedOn,
                IsActive = rg.IsActive
            };
        }

        private IAutoEndLog CreateAutoEndLogModel(AutoEndLog log)
        {
            if (log == null) return null;

            return new AutoEndLogItem
            {
                Action = log.Action,
                AutoEndLogID = log.AutoEndLogID,
                ClientID = log.ClientID,
                DisplayName = log.DisplayName,
                ReservationID = log.ReservationID,
                ResourceID = log.ResourceID,
                ResourceName = log.ResourceName,
                Timestamp = log.Timestamp
            };
        }

        private IReservationHistory CreateReservationHistoryModel(ReservationHistory rh)
        {
            if (rh == null) return null;

            return new ReservationHistoryItem
            {
                ReservationHistoryID = rh.ReservationHistoryID,
                ReservationID = rh.Reservation.ReservationID,
                AccountID = rh.Account.AccountID,
                UserAction = rh.UserAction,
                ActionSource = rh.ActionSource,
                BeginDateTime = rh.BeginDateTime,
                EndDateTime = rh.EndDateTime,
                ChargeMultiplier = rh.ChargeMultiplier,
                LinkedReservationID = rh.LinkedReservationID,
                ModifiedByClientID = rh.ModifiedByClientID,
                ModifiedDateTime = rh.ModifiedDateTime
            };
        }
    }
}
