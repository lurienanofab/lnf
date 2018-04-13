using LNF.Cache;
using LNF.CommonTools;
using LNF.Data;
using LNF.Logging;
using LNF.Models.Data;
using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;
using LNF.Scheduler.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace LNF.Scheduler
{
    public class ReservationManager : ManagerBase
    {
        public EmailManager EmailManager { get; }

        public ReservationManager(ISession session) : base(session)
        {
            EmailManager = new EmailManager(session);
        }

        public readonly ReservationState[] TruthTable = new[]
        {
            ReservationState.Other, ReservationState.Other, ReservationState.Other, ReservationState.Other,
            ReservationState.Other, ReservationState.Other, ReservationState.Other, ReservationState.Other,
            ReservationState.Invited, ReservationState.Invited, ReservationState.Invited, ReservationState.Invited,
            ReservationState.Invited, ReservationState.NotInLab, ReservationState.Invited, ReservationState.NotInLab,
            ReservationState.Undefined, ReservationState.UnAuthToStart, ReservationState.Editable, ReservationState.Editable,
            ReservationState.Undefined, ReservationState.NotInLab, ReservationState.Editable, ReservationState.Editable,
            ReservationState.Undefined, ReservationState.Undefined, ReservationState.Undefined, ReservationState.Undefined,
            ReservationState.Undefined, ReservationState.Undefined, ReservationState.Undefined, ReservationState.Undefined,
            ReservationState.Other, ReservationState.Other, ReservationState.Other, ReservationState.Other,
            ReservationState.Other, ReservationState.Other, ReservationState.Other, ReservationState.Other,
            ReservationState.Invited, ReservationState.Invited, ReservationState.Invited, ReservationState.Invited,
            ReservationState.Invited, ReservationState.StartOnly, ReservationState.Invited, ReservationState.StartOnly,
            ReservationState.Undefined, ReservationState.UnAuthToStart, ReservationState.Editable, ReservationState.Editable,
            ReservationState.Undefined, ReservationState.StartOnly, ReservationState.Editable, ReservationState.StartOrDelete,
            ReservationState.Undefined, ReservationState.Undefined, ReservationState.Undefined, ReservationState.Undefined,
            ReservationState.Undefined, ReservationState.Undefined, ReservationState.Undefined, ReservationState.Undefined
        };

        public readonly ReservationState[] TruthTableTE = new[]
        {
            ReservationState.Undefined, ReservationState.StartOnly, ReservationState.Editable, ReservationState.StartOrDelete
        };

        public int GetSubStateVal(bool isInLab, bool isReserver, bool isInvited, bool isAuthorized, bool isBeforeMinCancelTime, bool isStartable)
        {
            return (isInLab ? 32 : 0)
                + (isReserver ? 16 : 0)
                + (isInvited ? 8 : 0)
                + (isAuthorized ? 4 : 0)
                + (isBeforeMinCancelTime ? 2 : 0)
                + (isStartable ? 1 : 0);
        }

        public void InsertForModification(Reservation rsv, int linkedReservationId, int? modifiedByClientId)
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

            bool isInLab = CacheManager.Current.ClientInLab(rsv.Resource.ProcessTech.Lab.LabID);

            CanCreateCheck(rsv);

            Session.Insert(rsv);

            InsertReservationHistory("InsertForModification", "procReservationInsert", rsv, modifiedByClientId, linkedReservationId);
        }

        public Reservation InsertRepair(int resourceId, int clientId, DateTime beginDateTime, DateTime endDateTime, DateTime actualBeginDateTime, string notes, int? modifiedByClientId)
        {
            // procReservationInsert @Action = 'InsertRepair'

            //SELECT @ActivityID = ActivityID
            //FROM dbo.Activity
            //WHERE Editable = 0

            Activity repairActivity = Session.Query<Activity>().FirstOrDefault(x => !x.Editable);

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
                Resource = Session.Single<Resource>(resourceId),
                Client = Session.Single<Client>(clientId),
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
                IsStarted = true, // was false - fixed on 2017-08-21 [jg]
                IsUnloaded = true, // was false - fixed on 2017-08-21 [jg]
                MaxReservedDuration = duration,
                KeepAlive = false
            };

            Session.SaveOrUpdate(result);

            InsertReservationHistory("InsertRepair", "procReservationInsert", result, modifiedByClientId);

            return result;
        }

        public void CanCreateCheck(Reservation rsv)
        {
            // ignore recurring reservations
            if (!rsv.RecurrenceID.HasValue)
            {
                // These two activites allow for creating a reservation in the past
                if (rsv.Activity != Properties.Current.Activities.FacilityDownTime && rsv.Activity != Properties.Current.Activities.Repair)
                {
                    // Granularity			: stored in minutes and entered in minutes
                    // Offset				: stored in hours and entered in hours 
                    DateTime granStartTime = ResourceUtility.GetNextGranularity(TimeSpan.FromMinutes(rsv.Resource.Granularity), TimeSpan.FromHours(rsv.Resource.Offset), DateTime.Now, NextGranDir.Previous);

                    if (rsv.BeginDateTime < granStartTime)
                    {
                        string body = string.Format("Unable to create a reservation. BeginDateTime is in the past.\n--------------------\nClient: {0} [{1}]\nResource: {2} [{3}]\nBeginDateTime: {4:yyyy-MM-dd HH:mm:ss}\nEndDateTime: {5:yyyy-MM-dd HH:mm:ss}", rsv.Client.DisplayName, rsv.Client.ClientID, rsv.Resource.ResourceName, rsv.Resource.ResourceID, rsv.BeginDateTime, rsv.EndDateTime);
                        ServiceProvider.Current.Email.SendMessage(CacheManager.Current.CurrentUser.ClientID, "LNF.Repository.Scheduler.Reservation.CanCreateCheck()", "Create reservation failed", body, SendEmail.SystemEmail, SendEmail.DeveloperEmails);
                        throw new Exception("Your reservation was not created. Cannot create a reservation in the past.");
                    }
                }
            }

            if (rsv.EndDateTime <= rsv.BeginDateTime)
            {
                string body = string.Format("Unable to create a reservation. EndDateTime is before BeginDateTime.\n--------------------\nClient: {0} [{1}]\nResource: {2} [{3}]\nBeginDateTime: {4:yyyy-MM-dd HH:mm:ss}\nEndDateTime: {5:yyyy-MM-dd HH:mm:ss}", rsv.Client.DisplayName, rsv.Client.ClientID, rsv.Resource.ResourceName, rsv.Resource.ResourceID, rsv.BeginDateTime, rsv.EndDateTime);
                ServiceProvider.Current.Email.SendMessage(CacheManager.Current.CurrentUser.ClientID, "LNF.Repository.Scheduler.Reservation.CanCreateCheck()", "Create reservation failed", body, SendEmail.SystemEmail, SendEmail.DeveloperEmails);
                throw new Exception("Your reservation was not created. Cannot create a reservation that ends before it starts.");
            }

            // conflicting reservations must be:
            //      1) same resource
            //      2) not canceled
            //      3) date ranges overlap

            var conflict = Session.Query<Reservation>().Where(x =>
                x.Resource == rsv.Resource // same resource
                && x.IsActive // not canceled
                && !x.ActualEndDateTime.HasValue // not ended
                && (x.BeginDateTime < rsv.EndDateTime && x.EndDateTime > rsv.BeginDateTime) // date ranges overlap
            ).FirstOrDefault();

            if (conflict != null)
            {
                string body = string.Format("Unable to create a reservation. There is a conflict with an existing reservation.\n--------------------\nClient: {0} [{1}]\nResource: {2} [{3}]\nBeginDateTime: {4:yyyy-MM-dd HH:mm:ss}\nEndDateTime: {5:yyyy-MM-dd HH:mm:ss}\nConflicting ReservationID: {6}", rsv.Client.DisplayName, rsv.Client.ClientID, rsv.Resource.ResourceName, rsv.Resource.ResourceID, rsv.BeginDateTime, rsv.EndDateTime, conflict.ReservationID);
                ServiceProvider.Current.Email.SendMessage(CacheManager.Current.CurrentUser.ClientID, "LNF.Repository.Scheduler.Reservation.CanCreateCheck()", "Create reservation failed", body, SendEmail.SystemEmail, SendEmail.DeveloperEmails);
                throw new Exception(string.Format("Your reservation was not created. There is a conflict with an existing reservation [#{0}].", conflict.ReservationID));
            }
        }

        /// <summary>
        /// Ends a reservation, and turn off Interlock
        /// </summary>
        public async Task EndReservation(int reservationId)
        {
            var rsv = Session.Single<Reservation>(reservationId);

            // Make sure this reservation hasn't already been ended some how
            // [2016-01-21 jg] Allow FacilityDownTime because they already have ActualEndDateTime set
            if (rsv.ActualEndDateTime != null && !rsv.Activity.IsFacilityDownTime)
                return;

            // End Reservation
            int clientId = CacheManager.Current.CurrentUser.ClientID;
            End(rsv, clientId, clientId);

            // Turn Interlock Off
            if (CacheManager.Current.WagoEnabled)
                await WagoInterlock.ToggleInterlock(rsv.Resource.ResourceID, false, 0);

            // Check for other open reservation slots between now and the reservation fence
            DateTime? nextBeginDateTime = OpenResSlot(rsv.Resource.ResourceID, TimeSpan.FromMinutes(rsv.Resource.ReservFence), TimeSpan.FromMinutes(rsv.Resource.MinReservTime), DateTime.Now, rsv.EndDateTime);

            if (nextBeginDateTime == null)
                return;

            // Get the next reservation start time
            DateTime currentEndDateTime = ResourceUtility.GetNextGranularity(TimeSpan.FromMinutes(rsv.Resource.Granularity), TimeSpan.FromHours(rsv.Resource.Offset), rsv.ActualEndDateTime.Value, NextGranDir.Previous);

            // Send email notifications to all clients who want to be notified of open reservation slots
            EmailManager.EmailOnOpenSlot(rsv.Resource.ResourceID, currentEndDateTime, nextBeginDateTime.Value, EmailNotify.Always, reservationId);

            if (nextBeginDateTime.Value.Subtract(currentEndDateTime).TotalMinutes >= rsv.Resource.MinReservTime)
                EmailManager.EmailOnOpenSlot(rsv.Resource.ResourceID, currentEndDateTime, nextBeginDateTime.Value, EmailNotify.OnOpening, reservationId);
        }

        public ReservationState GetReservationState(int reservationId, int clientId, bool isInLab)
        {
            // Get Reservation Info
            var rsv = Session.Single<Reservation>(reservationId);
            var client = Session.Single<Client>(clientId);

            return GetReservationState(rsv, client, isInLab);
        }

        public ReservationState GetReservationState(Reservation rsv, Client client, bool isInLab)
        {
            var args = ReservationStateArgs.Create(rsv, client, isInLab, this);

            try
            {
                return GetReservationState(args);
            }
            catch (Exception ex)
            {
                string errmsg = string.Format(string.Format("Unable to determine reservation state for ReservationID: {0}, BeginDateTime: {1:yyyy-MM-dd HH:mm:ss}, ResourceID: {2}, isReserver: {3}, isInvited: {4}, isAuthorized: {5}, beforeMinCancelTime: {6}", rsv.ReservationID, rsv.BeginDateTime, rsv.Resource.ResourceID, args.IsReserver, args.IsInvited, args.IsAuthorized, args.IsBeforeMinCancelTime));
                throw new Exception(errmsg, ex);
            }
        }

        public ReservationState GetReservationState(ReservationStateArgs args)
        {
            // This is the truth table for non tool engineer 
            // note that having both R and I true is meaningless
            // L - IsInLab
            // R - IsReserver
            // I - IsInvited
            // A - IsAuth
            // M - Before MCT
            // S - In Start Per

            // L  R  I  A  M  S  
            // 0  0  0  0  0  0  ReservationState.Other
            // 0  0  0  0  0  1  ReservationState.Other
            // 0  0  0  0  1  0  ReservationState.Other
            // 0  0  0  0  1  1  ReservationState.Other
            // 0  0  0  1  0  0  ReservationState.Other
            // 0  0  0  1  0  1  ReservationState.Other
            // 0  0  0  1  1  0  ReservationState.Other
            // 0  0  0  1  1  1  ReservationState.Other
            // 0  0  1  0  0  0  ReservationState.Invited
            // 0  0  1  0  0  1  ReservationState.Invited
            // 0  0  1  0  1  0  ReservationState.Invited
            // 0  0  1  0  1  1  ReservationState.Invited
            // 0  0  1  1  0  0  ReservationState.Invited
            // 0  0  1  1  0  1  ReservationState.NotInLab
            // 0  0  1  1  1  0  ReservationState.Invited
            // 0  0  1  1  1  1  ReservationState.NotInLab
            // 0  1  0  0  0  0  ReservationState.Undefined
            // 0  1  0  0  0  1  ReservationState.UnAuthToStart
            // 0  1  0  0  1  0  ReservationState.Editable
            // 0  1  0  0  1  1  ReservationState.Editable
            // 0  1  0  1  0  0  ReservationState.Undefined
            // 0  1  0  1  0  1  ReservationState.NotInLab
            // 0  1  0  1  1  0  ReservationState.Editable
            // 0  1  0  1  1  1  ReservationState.Editable
            // 0  1  1  0  0  0  ReservationState.Undefined
            // 0  1  1  0  0  1  ReservationState.Undefined
            // 0  1  1  0  1  0  ReservationState.Undefined
            // 0  1  1  0  1  1  ReservationState.Undefined
            // 0  1  1  1  0  0  ReservationState.Undefined
            // 0  1  1  1  0  1  ReservationState.Undefined
            // 0  1  1  1  1  0  ReservationState.Undefined
            // 0  1  1  1  1  1  ReservationState.Undefined
            // 1  0  0  0  0  0  ReservationState.Other
            // 1  0  0  0  0  1  ReservationState.Other
            // 1  0  0  0  1  0  ReservationState.Other
            // 1  0  0  0  1  1  ReservationState.Other
            // 1  0  0  1  0  0  ReservationState.Other
            // 1  0  0  1  0  1  ReservationState.Other
            // 1  0  0  1  1  0  ReservationState.Other
            // 1  0  0  1  1  1  ReservationState.Other
            // 1  0  1  0  0  0  ReservationState.Invited
            // 1  0  1  0  0  1  ReservationState.Invited
            // 1  0  1  0  1  0  ReservationState.Invited
            // 1  0  1  0  1  1  ReservationState.Invited
            // 1  0  1  1  0  0  ReservationState.Invited
            // 1  0  1  1  0  1  ReservationState.StartOnly
            // 1  0  1  1  1  0  ReservationState.Invited
            // 1  0  1  1  1  1  ReservationState.StartOnly
            // 1  1  0  0  0  0  ReservationState.Undefined
            // 1  1  0  0  0  1  ReservationState.UnAuthToStart
            // 1  1  0  0  1  0  ReservationState.Editable
            // 1  1  0  0  1  1  ReservationState.Editable
            // 1  1  0  1  0  0  ReservationState.Undefined
            // 1  1  0  1  0  1  ReservationState.StartOnly
            // 1  1  0  1  1  0  ReservationState.Editable
            // 1  1  0  1  1  1  ReservationState.StartOrDelete
            // 1  1  1  0  0  0  ReservationState.Undefined
            // 1  1  1  0  0  1  ReservationState.Undefined
            // 1  1  1  0  1  0  ReservationState.Undefined
            // 1  1  1  0  1  1  ReservationState.Undefined
            // 1  1  1  1  0  0  ReservationState.Undefined
            // 1  1  1  1  0  1  ReservationState.Undefined
            // 1  1  1  1  1  0  ReservationState.Undefined
            // 1  1  1  1  1  1  ReservationState.Undefined
            // L  R  I  A  M  S 

            // The truth table for tool engineers is simply this
            // for TE, MCT=0, so both M and S cannot both be false
            // L  R  I  A  M  S  
            // x  x  x  x  0  0  ReservationState.Undefined
            // x  x  x  x  0  1  ReservationState.StartOnly
            // x  x  x  x  1  0  ReservationState.Editable
            // x  x  x  x  1  1  ReservationState.StartOrDelete

            // Note that the four cases in which the res can be started are modified by IsInLab. 
            // if this is false, the state changes as shown above 

            // Repair Reservations, returns immediately
            if (!args.IsRepair) return ReservationState.Repair;

            if (args.ActualBeginDateTime == null && args.ActualEndDateTime == null)
            {
                // reservations that have not yet been started
                if (args.EndDateTime <= DateTime.Now) // should never occur - if in the past, the actuals should exist
                {
                    if (args.IsReserver)
                        return ReservationState.PastSelf;
                    else
                        return ReservationState.PastOther;
                }

                // redefine min cancel time (MCT) for tool engineers - can edit up until scheduled start time
                return GetUnstartedReservationState(args.BeginDateTime, args.MinReservTime, args.IsInLab, args.IsToolEngineer, args.IsReserver, args.IsInvited, args.IsAuthorized, args.IsBeforeMinCancelTime);
            }
            else if (args.ActualBeginDateTime != null && args.ActualEndDateTime == null)
            {
                // reservations that have been started
                if (args.IsReserver || args.IsToolEngineer)
                    return ReservationState.Endable;
                else if (args.IsInvited)
                {
                    if (args.UserAuth != ClientAuthLevel.UnauthorizedUser && args.UserAuth != ClientAuthLevel.RemoteUser)
                        //2008-06-26 Sandrine requested that invitee should be able to end the reservation
                        return ReservationState.Endable;
                    else
                        return ReservationState.Invited;
                }
                else
                    return ReservationState.ActiveNotEndable;
            }
            else if (args.ActualBeginDateTime != null && args.ActualEndDateTime != null)
            {
                // at this point actualEndDateTime must not be null, and
                // we don't care if actualBeginDateTime is null or not

                // reservations in the past OR it's Facility Down Time reservation
                if (args.IsFacilityDownTime)
                {
                    // Facility Down Time, it must be editable if it's not started yet
                    if (args.ActualEndDateTime.HasValue && args.ActualEndDateTime.Value < DateTime.Now && args.IsToolEngineer)
                        return ReservationState.PastSelf; // FDT reservation that has already ended
                    else if (args.BeginDateTime > DateTime.Now && args.IsToolEngineer)
                        return ReservationState.Editable; // FDT reservation that has not started yet
                    else if (args.EndDateTime > DateTime.Now && args.IsToolEngineer)
                        return ReservationState.Endable; //it's endable only if it's not ended yet
                    else
                        return ReservationState.Other;
                }

                if (args.IsReserver)
                    return ReservationState.PastSelf;
                else
                    return ReservationState.PastOther;
            }
            else //if (actualBeginDateTime == null && actualEndDateTime != null)
            {
                // a reservation cannot have ended if it never began
                throw new InvalidOperationException("ActualBeginDateTime cannot be null if ActualEndDateTime is not null.");
            }
        }

        public ReservationState GetUnstartedReservationState(DateTime beginDateTime, int minReservTime, bool isInLab, bool isEngineer, bool isReserver, bool isInvited, bool isAuthorized, bool isBeforeMinCancelTime)
        {
            var isStartable = IsStartable(beginDateTime, minReservTime);

            var actual = (isEngineer)
                 ? new { isInLab = false, isReserver = false, isInvited = false, isAuthorized = false, isBeforeMinCancelTime = DateTime.Now <= beginDateTime, isStartable }
                 : new { isInLab, isReserver, isInvited, isAuthorized, isBeforeMinCancelTime, isStartable };

            var subStateValue = GetSubStateVal(actual.isInLab, actual.isReserver, actual.isInvited, actual.isAuthorized, actual.isBeforeMinCancelTime, actual.isStartable);

            var result = (isEngineer)
                ? TruthTableTE[subStateValue]
                : TruthTable[subStateValue];

            if (result == ReservationState.Undefined)
            {
                var errmsg = "Unstarted reservation state is undefined."
                    + " IsEngineer: {0}, IsInLab: {1}, IsReserver: {2}, IsInvited: {3}, IsAuthorized: {4}, IsBeforeMinCancelTime: {5}, IsStartable: {6}, SubStateValue: {7}";

                throw new Exception(string.Format(errmsg,
                    isEngineer ? "Yes" : "No",
                    actual.isInLab ? "Yes" : "No",
                    actual.isReserver ? "Yes" : "No",
                    actual.isInvited ? "Yes" : "No",
                    actual.isAuthorized ? "Yes" : "No",
                    actual.isBeforeMinCancelTime ? "Yes" : "No",
                    actual.isStartable ? "Yes" : "No",
                    subStateValue));
            }

            return result;
        }

        public bool IsStartable(ReservationState state)
        {
            switch (state)
            {
                case ReservationState.StartOrDelete:
                case ReservationState.StartOnly:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsStartable(DateTime beginDateTime, int minReservTime)
        {
            return IsStartable(DateTime.Now, beginDateTime, minReservTime);
        }

        public bool IsStartable(DateTime now, DateTime beginDateTime, int minReservTime)
        {
            return (now > beginDateTime.AddMinutes(-1 * minReservTime));
        }

        public DateTime? OpenResSlot(int resourceId, TimeSpan reservFence, TimeSpan minReservTime, DateTime now, DateTime sd)
        {
            var query = SelectByResource(resourceId, now, now.Add(reservFence), false).OrderBy(x => x.BeginDateTime).ToList();

            for (int j = 1; j < query.Count - 1; j++)
            {
                // If there are other open reservation slots, then don't email reserver
                var curBeginDateTime = query[j].BeginDateTime;
                var lastEndDateTime = query[j - 1].EndDateTime;
                if (curBeginDateTime.Subtract(lastEndDateTime) >= minReservTime)
                    return null;
            }

            var followingReservations = query.Where(x => x.BeginDateTime >= sd).OrderBy(x => x.BeginDateTime);

            if (followingReservations.Count() == 0)
                // There are no other reservations behind it
                return null;
            else
                return followingReservations.First().BeginDateTime;
        }

        public Reservation Create(Resource resource, Client client, Account account, Activity activity, DateTime beginDateTime, DateTime endDateTime, double duration, string notes, bool autoEnd, bool hasProcessInfo, bool hasInvitees, ReservationRecurrence recurrence, bool isActive, bool keepAlive, double maxReservedDuration, Client modifiedBy)
        {
            int? recurId = (recurrence == null) ? default(int?) : recurrence.RecurrenceID;

            return Session.NamedQuery("CreateReservation").SetParameters(new
            {
                resource.ResourceID,
                client.ClientID,
                account.AccountID,
                activity.ActivityID,
                BeginDateTime = beginDateTime,
                EndDateTime = endDateTime,
                Duration = duration,
                Notes = notes,
                AutoEnd = autoEnd,
                HasProcessInfo = hasProcessInfo,
                HasInvitees = hasInvitees,
                RecurrenceID = recurId,
                IsActive = isActive,
                CreatedOn = DateTime.Now,
                KeepAlive = keepAlive,
                MaxReservedDuration = maxReservedDuration,
                ModifiedByClientID = modifiedBy.ClientID
            }).List<Reservation>().FirstOrDefault();
        }

        public Reservation FromDataRow(DataRow dr)
        {
            if (dr.Table.Columns.Contains("ReservationID") && int.TryParse(dr["ReservationID"].ToString(), out int id))
                return Session.Single<Reservation>(id);
            return null;
        }

        public IList<Reservation> SelectByResource(int resourceId, DateTime startDate, DateTime endDate, bool includeDeleted)
        {
            if (includeDeleted)
                return Session.Query<Reservation>().Where(x => x.Resource.ResourceID == resourceId &&
                    ((x.BeginDateTime < endDate && x.EndDateTime > startDate) || (x.ActualBeginDateTime < endDate && x.ActualEndDateTime > startDate))).ToList();
            else
                return Session.Query<Reservation>().Where(x => x.IsActive && x.Resource.ResourceID == resourceId &&
                    ((x.BeginDateTime < endDate && x.EndDateTime > startDate) || (x.ActualBeginDateTime < endDate && x.ActualEndDateTime > startDate))).ToList();
        }

        public IList<Reservation> SelectByProcessTech(int processTechId, DateTime startDate, DateTime endDate, bool includeDeleted)
        {
            if (includeDeleted)
                return Session.Query<Reservation>().Where(x => x.Resource.ProcessTech.ProcessTechID == processTechId && x.BeginDateTime < endDate && x.EndDateTime > startDate).ToList();
            else
                return Session.Query<Reservation>().Where(x => x.Resource.ProcessTech.ProcessTechID == processTechId && x.BeginDateTime < endDate && x.EndDateTime > startDate && x.IsActive).ToList();
        }

        public IList<Reservation> SelectByClient(int clientId, DateTime startDate, DateTime endDate, bool includeDeleted)
        {
            List<Reservation> result = new List<Reservation>();

            if (includeDeleted)
            {
                result.AddRange(Session.Query<Reservation>().Where(x => x.Client.ClientID == clientId && x.BeginDateTime < endDate && x.EndDateTime > startDate));
                result.AddRange(Session.Query<ReservationInvitee>().Where(x => x.Invitee.ClientID == clientId && x.Reservation.BeginDateTime < endDate && x.Reservation.EndDateTime > startDate).Select(x => x.Reservation));
            }
            else
            {
                result.AddRange(Session.Query<Reservation>().Where(x => x.Client.ClientID == clientId && x.BeginDateTime < endDate && x.EndDateTime > startDate && x.IsActive));
                result.AddRange(Session.Query<ReservationInvitee>().Where(x => x.Invitee.ClientID == clientId && x.Reservation.BeginDateTime < endDate && x.Reservation.EndDateTime > startDate && x.Reservation.IsActive).Select(x => x.Reservation));
            }

            return result;
        }

        ///<summary>
        ///Ends any reservations that needs to be auto-ended. This includes both types of auto-ending: resource-centric and reservation-centric.
        ///</summary>
        public async Task EndAutoEndReservations(IEnumerable<Reservation> reservations)
        {
            //End auto-end reservations, and turn off interlocks

            int count = reservations.Count();

            using (var timer = LogTaskTimer.Start("ReservationUtility.EndAutoEndReservations", "count = {0}", () => new object[] { count }))
            {
                foreach (Reservation rsv in reservations)
                {
                    try
                    {
                        End(rsv, -1, -1);
                        await WagoInterlock.ToggleInterlock(rsv.Resource.ResourceID, false, 0); //always pass 0 when ending auto-end reservations
                        AutoEndLog.AddEntry(rsv, "autoend");
                        timer.AddData("Ended auto-end reservation {0} for resource {1}", rsv.ReservationID, rsv.Resource.ResourceID);
                    }
                    catch (Exception ex)
                    {
                        timer.AddData("***ERROR*** Failed to auto-end reservation {0} for resource {1}: {2}", rsv.ReservationID, rsv.Resource.ResourceID, ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Deletes a reservation
        /// </summary>
        public void DeleteReservation(int reservationId)
        {
            // Get Reservation Info and Resource Info
            var rsv = Session.Single<Reservation>(reservationId);
            var res = rsv.Resource;

            // Delete reservation
            Delete(rsv, CacheManager.Current.CurrentUser.ClientID);

            // Send email to reserver and invitees
            EmailManager.EmailOnUserDelete(rsv);
            EmailManager.EmailOnUninvited(rsv, ReservationInviteeItem.Create(GetInvitees(rsv)));

            // Send email notifications to all clients want to be notified of open reservation slots
            EmailManager.EmailOnOpenSlot(res.ResourceID, rsv.BeginDateTime, rsv.EndDateTime, EmailNotify.Always, reservationId);

            // Check for other open reservation slots between now and the reservation fence and Get the next reservation start time
            DateTime? nextBeginDateTime = OpenResSlot(res.ResourceID, TimeSpan.FromMinutes(res.ReservFence), TimeSpan.FromMinutes(res.MinReservTime), DateTime.Now, rsv.EndDateTime);

            if (nextBeginDateTime.HasValue)
                EmailManager.EmailOnOpenSlot(res.ResourceID, rsv.BeginDateTime, nextBeginDateTime.Value, EmailNotify.OnOpening, reservationId);
        }

        public IQueryable<ReservationInvitee> GetInvitees(Reservation rsv)
        {
            return Session.Query<ReservationInvitee>().Where(x => x.Reservation.ReservationID == rsv.ReservationID);
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

            IList<Reservation> result = Session.Query<Reservation>()
                .Where(x =>
                    x.Resource.ResourceID == resourceId
                    && x.IsActive
                    && x.ActualBeginDateTime != null
                    && x.ActualEndDateTime == null)
                .OrderByDescending(x => x.EndDateTime)
                .ToList();

            return result;
        }

        public IList<Reservation> SelectAutoEnd()
        {
            return Session.NamedQuery("SelectAutoEndReservations").List<Reservation>();
        }

        public IList<Reservation> SelectPastEndableRepair()
        {
            return Session.NamedQuery("SelectPastEndableRepairReservations").List<Reservation>();
        }

        public IList<Reservation> SelectPastUnstarted()
        {
            return Session.NamedQuery("SelectPastUnstartedReservations").List<Reservation>();
        }

        //Ends a past unstarted reservation
        public int EndPastUnstarted(Reservation rsv, DateTime endDate, int clientId)
        {
            //ClientID might be -1
            return Session.NamedQuery("EndPastUnstartedReservations").SetParameters(new
            {
                rsv.ReservationID,
                EndDateTime = endDate,
                ClientID = clientId
            }).Result<int>();
        }

        public IList<Reservation> SelectExisting(Resource resource)
        {
            DateTime dtn = DateTime.Now;
            IList<Reservation> reservationsWithFutureEndDateTime = Session.Query<Reservation>().Where(x => x.EndDateTime > DateTime.Now && x.ActualEndDateTime == null && x.Resource == resource).ToList();
            IList<Reservation> reservationsWithFutureEndDateTimeWhereChargeBeginDateTimeLessThanNow = reservationsWithFutureEndDateTime.Where(x => x.ChargeBeginDateTime() < dtn).ToList();
            return reservationsWithFutureEndDateTimeWhereChargeBeginDateTimeLessThanNow;
        }

        public IList<Reservation> ReservationsInWindow(Resource resource, int minutes)
        {
            DateTime edate = DateTime.Now.AddMinutes(minutes);
            IList<Reservation> result = Session.Query<Reservation>().Where(x => x.BeginDateTime >= DateTime.Now && x.BeginDateTime < edate && x.Resource == resource).ToList();
            return result;
        }

        public int GetAvailableSchedMin(int resourceId, int clientId)
        {
            string sql = "EXEC sselScheduler.dbo.procReservationSelect @Action = 'GetAvailableSchedMin', @ResourceID = :resourceId, @ClientID = :clientId";
            return Session.SqlQuery(sql).SetParameters(new { resourceId, clientId }).Result<int>();
        }

        ///<summary>
        /// Ends any reservations that the reserver fails to start before the grace period had ended.
        ///</summary>
        public void EndUnstartedReservations(IEnumerable<Reservation> reservations)
        {
            //End unstarted reservations

            int count = reservations.Count();

            using (var timer = LogTaskTimer.Start("ReservationUtility.EndUnstartedReservations", "count = {0}", () => new object[] { count }))
            {
                foreach (Reservation rsv in reservations)
                {
                    DateTime oldEndDateTime = rsv.EndDateTime;
                    DateTime newEndDateTime = rsv.BeginDateTime;

                    bool endReservation = false;
                    DateTime ed;

                    if (rsv.KeepAlive)
                    {
                        //KeepAlive: we don't care about GracePeriod, only AutoEnd
                        if (rsv.Resource.AutoEnd <= 0 || rsv.AutoEnd)
                            ed = rsv.EndDateTime;
                        else
                            ed = rsv.EndDateTime.AddMinutes(rsv.Resource.AutoEnd);

                        if (ed <= DateTime.Now)
                        {
                            endReservation = true;
                            newEndDateTime = ed;
                        }
                    }
                    else
                    {
                        //The end datetime will be the scheduled begin datetime plus the grace period
                        ed = newEndDateTime.AddMinutes(rsv.Resource.GracePeriod);
                        endReservation = true;
                        newEndDateTime = ed;
                    }

                    if (endReservation)
                    {
                        EndPastUnstarted(rsv, newEndDateTime, -1);
                        AutoEndLog.AddEntry(rsv, "unstarted");
                        timer.AddData("Unstarted reservation {0} was ended, KeepAlive = {1}, Reservation.AutoEnd = {2}, Resource.AutoEnd = {3}, eDate = '{4}'", rsv.ReservationID, rsv.KeepAlive, rsv.AutoEnd, rsv.Resource.AutoEnd, ed);

                        DateTime? NextBeginDateTime = OpenResSlot(rsv.Resource.ResourceID, TimeSpan.FromMinutes(rsv.Resource.ReservFence), TimeSpan.FromMinutes(rsv.Resource.MinReservTime), DateTime.Now, oldEndDateTime);

                        //Check if reservation slot becomes big enough
                        if (NextBeginDateTime.HasValue)
                        {
                            if (NextBeginDateTime.Value.Subtract(oldEndDateTime).TotalMinutes < rsv.Resource.MinReservTime
                                && NextBeginDateTime.Value.Subtract(newEndDateTime).TotalMinutes >= rsv.Resource.MinReservTime)
                            {
                                EmailManager.EmailOnOpenReservations(rsv.Resource.ResourceID, newEndDateTime, NextBeginDateTime.Value);
                            }
                        }
                    }
                    else
                        timer.AddData("Unstarted reservation {0} was not ended, KeepAlive = {1}, Reservation.AutoEnd = {2}, Resource.AutoEnd = {3}, eDate = '{4}'", rsv.ReservationID, rsv.KeepAlive, rsv.AutoEnd, rsv.Resource.AutoEnd, ed);
                }
            }
        }

        ///<summary>
        ///Ends any repair reservations that are in the past.
        ///</summary>
        public void EndRepairReservations(IEnumerable<Reservation> reservations)
        {
            //End past repair reservations
            int count = reservations.Count();

            using (var timer = LogTaskTimer.Start("ReservationUtility.EndRepairReservations", "count = {0}", () => new object[] { count }))
            {
                foreach (Reservation rsv in reservations)
                {
                    End(rsv, -1, -1);
                    timer.AddData("Ended repair reservation {0}", rsv.ReservationID);

                    //Reset resource state
                    ResourceUtility.UpdateState(rsv.Resource.ResourceID, ResourceState.Online, string.Empty);
                    AutoEndLog.AddEntry(rsv, "repair");
                    timer.AddData("Set ResourceID {0} online", rsv.Resource.ResourceID);
                }
            }
        }

        public Reservation ModifyExistingReservation(Reservation rsv, ReservationDuration rd)
        {
            var currentUser = CacheManager.Current.CurrentUser;

            var result = GetReservationForModification(rsv, rd, out bool insert);

            UpdateReservation(result, rd);

            if (HandleFacilityDowntimeResrvation(result))
                UpdateFacilityDownTime(result, currentUser.ClientID);
            else
            {
                if (insert)
                {
                    InsertForModification(rsv, rsv.ReservationID, currentUser.ClientID);
                    rsv.AppendNotes(string.Format("Canceled for modification. New ReservationID: {0}", rsv.ReservationID));
                }
                else
                    Update(result, currentUser.ClientID);

                HandlePracticeReservation(result);
            }

            ReservationInviteeData.Update(CopyReservationInviteesTable(), result.ReservationID);
            ReservationInviteeData.Update(CopyReservationProcessInfoTable(), result.ReservationID);

            EmailManager.EmailOnUserUpdate(result);
            EmailManager.EmailOnInvited(result, CacheManager.Current.ReservationInvitees(), EmailManager.ReservationModificationType.Modified);
            EmailManager.EmailOnUninvited(rsv, CacheManager.Current.RemovedInvitees());

            return result;
        }

        public void UpdateNotes(Reservation rsv, string notes)
        {
            rsv.Notes = notes;

            // there is no reason to add reservation history
            // because notes are not tracked in the ReservationHistory table
        }

        public void UpdateCharges(Reservation rsv, double chargeMultiplier)
        {
            rsv.ChargeMultiplier = chargeMultiplier;
            AddReservationHistory("ReservationUtility", "UpdateCharges", rsv);
        }

        public void UpdateAccount(Reservation rsv, int accountId)
        {
            rsv.Account = Session.Single<Account>(accountId);
            AddReservationHistory("ReservationUtility", "UpdateAccount", rsv);
        }

        public DateTime GetBeginDateTime(Reservation rsv)
        {
            return (rsv.ActualBeginDateTime == null) ? rsv.BeginDateTime : rsv.ActualBeginDateTime.Value;
        }

        public DateTime GetEndDateTime(Reservation rsv)
        {
            return (rsv.ActualEndDateTime == null) ? rsv.EndDateTime : rsv.ActualEndDateTime.Value;
        }

        public ReservationInProgress GetRepairReservationInProgress(int resourceId)
        {
            var res = CacheManager.Current.ResourceTree().First(x => x.ResourceID == resourceId);

            if (res.CurrentReservationID > 0 && !res.CurrentActivityEditable)
            {
                return new ReservationInProgress()
                {
                    ReservationID = res.CurrentReservationID,
                    ClientID = res.CurrentClientID,
                    ActivityID = res.CurrentActivityID,
                    ActivityName = res.CurrentActivityName,
                    BeginDateTime = res.CurrentBeginDateTime.Value,
                    EndDateTime = res.CurrentEndDateTime.Value,
                    DisplayName = ClientItem.GetDisplayName(res.CurrentLastName, res.CurrentFirstName),
                    Editable = res.CurrentActivityEditable,
                    Notes = res.CurrentNotes,
                    ResourceID = res.ResourceID,
                    ResourceName = res.ResourceName
                };
            }

            return null;
        }

        public IList<Reservation> SelectReservationsByPeriod(DateTime period)
        {
            DateTime sdate = period;
            DateTime edate = sdate.AddMonths(1);

            IList<Reservation> result = Session.Query<Reservation>()
                .Where(x =>
                    x.ActualBeginDateTime != null &&
                    x.ActualEndDateTime != null &&
                    ((x.BeginDateTime < edate && x.EndDateTime > sdate) || (x.ActualBeginDateTime < edate && x.ActualEndDateTime > sdate))
                ).OrderBy(x => x.Client.ClientID)
                .ThenBy(x => x.Resource.ResourceID)
                .ThenBy(x => x.Account.AccountID)
                .ToList();

            return result;
        }

        public IList<Reservation> SelectByDateRange(DateTime startDate, DateTime endDate, int clientId = 0)
        {
            IList<Reservation> result = Session.Query<Reservation>().Where(x =>
                ((x.BeginDateTime < endDate && x.EndDateTime > startDate) ||
                (x.ActualBeginDateTime < endDate && x.ActualEndDateTime > startDate)) &&
                x.Client.ClientID == (clientId > 0 ? clientId : x.Client.ClientID)
            ).ToList();

            return result;
        }

        public ReservationHistory AddReservationHistory(string actionSource, string userAction, Reservation rsv, Client modifiedBy = null)
        {
            if (modifiedBy == null)
                modifiedBy = Session.Single<Client>(CacheManager.Current.CurrentUser.ClientID);

            ReservationHistory hist = new ReservationHistory()
            {
                ActionSource = actionSource,
                UserAction = userAction,
                Account = rsv.Account,
                BeginDateTime = rsv.BeginDateTime,
                ChargeMultiplier = rsv.ChargeMultiplier,
                EndDateTime = rsv.EndDateTime,
                ModifiedByClientID = modifiedBy.ClientID,
                ModifiedDateTime = DateTime.Now,
                Reservation = rsv
            };

            Session.Insert(hist);

            return hist;
        }

        /// <summary>
        /// Compose the tooltip text for the specified reservation
        /// </summary>
        public string GetReservationToolTip(Reservation rsv, ReservationState state)
        {
            // Display Reservation info
            string toolTip = string.Empty;

            if (state == ReservationState.PastOther || state == ReservationState.PastSelf)
                toolTip += string.Format("<div><b>Used by {0}</b></div>", rsv.Client.DisplayName);
            else if (state == ReservationState.Repair)
                toolTip += string.Format("<div><b>Repaired by {0}</b></div>", rsv.Client.DisplayName);
            else
                toolTip += string.Format("<div><b>Reserved by {0}</b></div>", rsv.Client.DisplayName);

            if (state == ReservationState.Other || state == ReservationState.PastOther)
            {
                int clientId = rsv.Client.ClientID;
                int accountId = rsv.Account.AccountID;
                var ca = CacheManager.Current.GetClientAccount(clientId, accountId);

                // ca is null for Remote Processing reservations because the acct does not actually belong to the client
                if (ca != null)
                {
                    if (!string.IsNullOrEmpty(ca.Phone))
                        toolTip += string.Format("<div><b>Phone: {0}</b></div>", ca.Phone);

                    if (!string.IsNullOrEmpty(ca.Email))
                        toolTip += string.Format("<div><b>Email: {0}</b></div>", ca.Email);
                }
            }

            // reservations in the past should have actual times, but may not - need to check for this
            if (rsv.ActualBeginDateTime != null && rsv.ActualEndDateTime != null)
                toolTip += string.Format("<div><b>{0} - {1}</b></div>", rsv.ActualBeginDateTime.Value.ToShortTimeString(), rsv.ActualEndDateTime.Value.ToShortTimeString());
            else
                toolTip += string.Format("<div><b>{0} - {1}</b></div>", rsv.BeginDateTime.ToShortTimeString(), rsv.EndDateTime.ToShortTimeString());

            // Reservation Notes
            if (!string.IsNullOrEmpty(rsv.Notes))
            {
                if (rsv.Notes.Length > 255)
                    toolTip += string.Format("<hr><div><b>Notes:</b></div><div>{0}...</div>", rsv.Notes.Substring(0, 255));
                else
                    toolTip += string.Format("<hr><div><b>Notes:</b></div><div>{0}</div>", rsv.Notes);
            }

            // Reservation Process Info
            using (var reader = ReservationProcessInfoData.SelectAllDataReader(rsv.ReservationID))
            {
                if (reader.Read())
                {
                    toolTip += "<hr><div><b>Process Info:</b></div>";
                    toolTip += string.Format("<div>{0}: {1} {2}</div>", reader["ProcessInfoName"], reader["Param"], reader["Value"]);
                    while (reader.Read())
                        toolTip += string.Format("<div>{0}: {1} {2}</div>", reader["ProcessInfoName"], reader["Param"], reader["Value"]);
                }

                reader.Close();
            }

            // Reservation Invitees
            using (var reader = ReservationInviteeData.SelectReservationInviteesDataReader(rsv.ReservationID))
            {
                if (reader.Read())
                {
                    toolTip += "<hr><div><b>Invitees:</b></div>";
                    toolTip += string.Format("<div>{0}</div>", reader["InviteeName"]);
                    while (reader.Read())
                        toolTip += string.Format("<div>{0}</div>", reader["InviteeName"]);
                }

                reader.Close();
            }

            toolTip += string.Format("<hr><div><em class=\"text-muted\">ReservationID: {0}</em></div>", rsv.ReservationID);

            return toolTip;
        }

        /// <summary>
        /// Compose the tooltip caption text for the specified reservation
        /// </summary>
        public string GetReservationCaption(ReservationState state)
        {
            switch (state)
            {
                case ReservationState.Undefined:
                    return "Reservation will be startable shortly";
                case ReservationState.Editable:
                    return "Reservation Summary (click icon to modify)";
                case ReservationState.StartOnly:
                case ReservationState.StartOrDelete:
                    return "Click to start reservation";
                case ReservationState.Endable:
                    return "Click to end reservation";
                case ReservationState.PastSelf:
                    return "Click to edit reservation run notes";
                case ReservationState.Other:
                case ReservationState.Invited:
                case ReservationState.PastOther:
                    return "Click to send email to reservation reserver";
                case ReservationState.Repair:
                    return "Resource offline for repair";
                case ReservationState.NotInLab:
                    return "You must be in the lab to start the reservation";
                case ReservationState.UnAuthToStart:
                    return "You are not authorized to start this reservation";
                case ReservationState.Meeting:
                    return "Regular meeting time";
                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// Starts a reservation, and turn on Interlock
        /// </summary>
        /// <param name="rsv">The reservation to start</param>
        /// <param name="clientId">The ClientID of the current user starting the reservation</param>
        public async Task StartReservation(Reservation rsv, int clientId, bool isInLab)
        {
            if (rsv == null)
                throw new ArgumentNullException("rsv", "A null Reservation object is not allowed.");

            if (rsv.Resource == null)
                throw new ArgumentNullException("rsv", "A null Resource object is not allowed.");

            if (rsv.IsStarted) return;

            ReservationState state = GetReservationState(rsv.ReservationID, clientId, isInLab);

            if (state != ReservationState.StartOnly && state != ReservationState.StartOrDelete)
                throw new Exception(string.Format("Reservation #{0} is not startable at this time. [State = {1}]", rsv.ReservationID, state));

            bool enableInterlock = CacheManager.Current.WagoEnabled;

            // End Previous un-ended reservations
            var endableRsvQuery = SelectEndableReservations(rsv.Resource.ResourceID);
            foreach (var endableRsv in endableRsvQuery)
                End(endableRsv, clientId, clientId);

            // Start Reservation
            Start(rsv, clientId, clientId);

            // If Resource authorization type is rolling and the reserver is a regular user for the resource then reset reserver's expiration date
            int authLevel = 0, resourceClientId = 0;

            using (var reader = ResourceClientData.SelectResourceClient(rsv.Resource.ResourceID, clientId))
            {
                if (reader.Read())
                {
                    authLevel = Convert.ToInt32(reader["AuthLevel"]);
                    resourceClientId = Convert.ToInt32(reader["ResourceClientID"]);
                }

                reader.Close();
            }

            var res = rsv.Resource;
            if (res.AuthState && (authLevel == (int)ClientAuthLevel.AuthorizedUser))
            {
                DateTime expiration = DateTime.Now.AddMonths(res.AuthDuration);
                ResourceClientData.UpdateExpiration(resourceClientId, expiration);
            }

            // Turn Interlock On
            if (enableInterlock)
            {
                uint duration = OnTheFlyUtility.GetStateDuration(res.ResourceID);
                await WagoInterlock.ToggleInterlock(res.ResourceID, true, duration);
                bool interlockState = await WagoInterlock.GetPointState(res.ResourceID);
                if (!interlockState)
                    throw new InvalidOperationException(string.Format("Failed to start interlock for ResourceID {0}", res.ResourceID));
            }
        }

        public Reservation GetReservationForModification(Reservation rsv, ReservationDuration rd, out bool insert)
        {
            throw new NotImplementedException();
        }

        public void UpdateReservation(Reservation rsv, ReservationDuration rd)
        {
            throw new NotImplementedException();
        }

        public bool HandleFacilityDowntimeResrvation(Reservation rsv)
        {
            throw new NotImplementedException();
        }

        public bool HandlePracticeReservation(Reservation rsv)
        {
            throw new NotImplementedException();
        }

        public DataTable CopyReservationInviteesTable()
        {
            throw new NotImplementedException();
        }

        public DataTable CopyReservationProcessInfoTable()
        {
            throw new NotImplementedException();
        }

        public IList<Reservation> SelectByGroup(int groupId)
        {
            // procReservationSelect @Action = 'ByGroupID'

            //SELECT res.ResourceID, res.ResourceName
            //FROM Reservation r
            //INNER JOIN [Resource] res ON res.ResourceID = r.ResourceID
            //WHERE r.GroupID = @GroupID

            IList<Reservation> result = Session.Query<Reservation>().Where(x => x.GroupID == groupId).ToList();

            return result;
        }

        public IList<Reservation> SelectHistory(int clientId, DateTime sd, DateTime ed)
        {
            var result = Session.Query<Reservation>()
                .Where(x => x.Client.ClientID == clientId && (x.BeginDateTime < ed) && (x.EndDateTime > sd)).ToList();

            return result;
        }

        public IList<Reservation> SelectHistoryToForgiveForRepair(int resourceId, DateTime sd, DateTime ed)
        {
            //[2013-05-20 jg] We no longer care if the reservation was canceled or not, all need to be forgiven
            //      because of the booking fee on uncancelled reservations.

            int repairActivityId = 14;

            var result = Session.Query<Reservation>().Where(x =>
                x.Resource.ResourceID == resourceId
                && ((x.BeginDateTime < ed && x.EndDateTime > sd) || (x.ActualBeginDateTime < ed && x.ActualEndDateTime > sd)) //will include all overlapping reservations
                && x.Activity.ActivityID != repairActivityId).OrderBy(x => x.BeginDateTime).ToList();

            return result;
        }

        public IList<Reservation> SelectOverwrittable(int resourceId, DateTime sd, DateTime ed)
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

            IList<Reservation> result = Session.Query<Reservation>().Where(x =>
                x.Resource.ResourceID == resourceId
                && x.BeginDateTime < ed
                && x.EndDateTime > sd
                && x.IsActive
                && x.ActualEndDateTime == null).ToList();

            return result;
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

                // also an entry into history is made for each reservation
                InsertReservationHistory("ByGroupID", "procReservationUpdate", rsv, modifiedByClientId);
            }

            return query.Count;
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

            Reservation rsv = Session.Query<Reservation>().Where(x =>
                !x.Activity.Editable
                && x.Resource.ResourceID == resourceId
                && (x.ActualEndDateTime != null && x.ActualEndDateTime < DateTime.Now)
            ).OrderByDescending(x => x.ActualEndDateTime).FirstOrDefault();

            if (rsv != null)
                return rsv.ActualEndDateTime;
            else
                return null;
        }

        public IList<ReservationHistoryFilterItem> FilterCancelledReservations(IList<Reservation> reservations, bool includeCanceledForModification)
        {
            if (reservations == null || reservations.Count == 0)
                return new List<ReservationHistoryFilterItem>();

            int minReservationId = reservations.Min(x => x.ReservationID);

            IList<ReservationHistory> hist = Session.Query<ReservationHistory>().Where(x => x.LinkedReservationID.HasValue && x.LinkedReservationID.Value >= minReservationId && x.UserAction == ReservationHistory.INSERT_FOR_MODIFICATION).ToList();
            IList<ReservationHistoryFilterItem> result = new List<ReservationHistoryFilterItem>();

            foreach (Reservation rsv in reservations)
            {
                bool isCanceledForModification = hist.Any(x => x.LinkedReservationID.HasValue && x.LinkedReservationID.Value == rsv.ReservationID);

                if (!isCanceledForModification || includeCanceledForModification)
                {
                    result.Add(new ReservationHistoryFilterItem()
                    {
                        Reservation = rsv,
                        IsCanceledForModification = hist.Any(x => x.LinkedReservationID.HasValue && x.LinkedReservationID.Value == rsv.ReservationID)
                    });
                }
            }

            return result;
        }

        public IList<Reservation> GetCurrentReservations()
        {
            return Session.Query<Reservation>()
                .Where(x => x.IsActive && x.IsStarted && x.ActualBeginDateTime != null && x.ActualEndDateTime == null)
                .OrderBy(x => x.ActualBeginDateTime)
                .ToList();
        }

        public IList<Reservation> GetConflictingReservations(IEnumerable<Reservation> reservations, DateTime sd, DateTime ed)
        {
            var result = reservations.Where(x => x.BeginDateTime < ed && x.EndDateTime > sd).ToList();
            return result;
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

        public ClientAuthLevel GetAuthLevel(IEnumerable<IAuthorized> resourceClients, IPrivileged client, int resourceId)
        {
            if (client.HasPriv(ClientPrivilege.Administrator | ClientPrivilege.Developer))
                return ClientAuthLevel.ToolEngineer;

            var rc = resourceClients.FirstOrDefault(x => x.ClientID == client.ClientID || x.ClientID == -1);

            if (rc == null)
                return ClientAuthLevel.UnauthorizedUser;

            if (resourceId == 0)
                return ClientAuthLevel.UnauthorizedUser;

            return rc.AuthLevel;
        }

        public ReservationHistory InsertReservationHistory(string action, string actionSource, Reservation rsv, int? modifiedByClientId = null, int? linkedReservationId = null)
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

        public IQueryable<ReservationHistory> GetHistory(Reservation rsv)
        {
            return Session.Query<ReservationHistory>().Where(x => x.Reservation.ReservationID == rsv.ReservationID);
        }

        public void Insert(Reservation rsv, int? modifiedByClientId)
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

            CanCreateCheck(rsv);

            rsv.LastModifiedOn = DateTime.Now;
            rsv.ChargeMultiplier = 1;
            rsv.ApplyLateChargePenalty = true;
            rsv.IsStarted = false;
            rsv.IsUnloaded = false;

            Session.Insert(rsv);

            // also an entry into history is made
            InsertReservationHistory("Insert", "procReservationInsert", rsv, modifiedByClientId);
        }

        public void DeleteAndForgive(Reservation rsv, int? modifiedByClientId)
        {
            // This is all that happens in procReservationDelete @Action = 'WithForgive'

            //-- Set Reservation to inactive
            //UPDATE dbo.Reservation
            //SET IsActive = 0, LastModifiedOn = GETDATE(), CancelledDateTime = GETDATE(), ChargeMultiplier = 0
            //WHERE ReservationID = @ReservationID

            rsv.IsActive = false;
            rsv.LastModifiedOn = DateTime.Now;
            rsv.CancelledDateTime = DateTime.Now;
            rsv.ChargeMultiplier = 0;

            //-- Delete Reservation ProcessInfos
            //DELETE FROM dbo.ReservationProcessInfo
            //WHERE ReservationID = @ReservationID

            Session.Delete(Session.Query<ReservationProcessInfo>().Where(x => x.Reservation.ReservationID == rsv.ReservationID));

            //-- Delete Reservation Invitees
            //DELETE FROM dbo.ReservationInvitee
            //WHERE ReservationID = @ReservationID

            Session.Delete(Session.Query<ReservationInvitee>().Where(x => x.Reservation.ReservationID == rsv.ReservationID));

            // also an entry into history is made
            InsertReservationHistory("WithForgive", "procReservationDelete", rsv, modifiedByClientId);
        }

        public IList<ClientAccount> AvailableAccounts(Reservation rsv)
        {
            IList<ClientAccount> result = null;

            DateTime sd = rsv.CreatedOn.Date;
            DateTime ed = sd.AddDays(1);

            if (rsv.Activity.AccountType == ActivityAccountType.Reserver || rsv.Activity.AccountType == ActivityAccountType.Both)
            {
                //Load reserver's accounts
                result = Session.ClientManager().ActiveClientAccounts(rsv.Client, sd, ed).ToList();
            }

            if (rsv.Activity.AccountType == ActivityAccountType.Invitee || rsv.Activity.AccountType == ActivityAccountType.Both)
            {
                //Loads each of the invitee's accounts
                foreach (ReservationInvitee ri in GetInvitees(rsv))
                {
                    IQueryable<ClientAccount> temp = Session.ClientManager().ActiveClientAccounts(ri.Invitee, sd, ed);

                    if (result == null)
                        result = temp.ToList();
                    else
                    {
                        foreach (ClientAccount t in temp)
                        {
                            //Check if account already exists
                            if (!result.Any(i => i.Account == t.Account))
                            {
                                result.Add(t);
                            }
                        }
                    }
                }
            }

            return result;
        }

        public void InsertFacilityDownTime(Reservation rsv, int? modifiedByClientId)
        {
            // This is all that happens in procReservationInsert @Action = 'InsertFacilityDownTime'

            //INSERT INTO dbo.Reservation(ResourceID, ClientID, AccountID, ActivityID,
            //  BeginDateTime, EndDateTime, ActualBeginDateTime, ActualEndDateTime, ClientIDBegin, ClientIDEnd, CreatedOn, LastModifiedOn,
            //  Duration, Notes, AutoEnd, ChargeMultiplier, RecurrenceID, GroupID, ApplyLateChargePenalty, 
            //  HasProcessInfo, HasInvitees, IsActive, IsStarted, IsUnloaded, MaxReservedDuration, KeepAlive)
            //VALUES(@ResourceID, @ClientID, @AccountID, @ActivityID,
            //  @BeginDateTime, @EndDateTime, @ActualBeginDateTime, @ActualEndDateTime, @ClientID, @ClientID, @CreatedOn, GETDATE(),
            //  @Duration, @Notes, @AutoEnd, 1.00, @RecurrenceID, @GroupID, 1,
            //  @HasProcessInfo, @HasInvitees, @IsActive, 0, 0, @Duration, 0)

            CanCreateCheck(rsv);

            rsv.LastModifiedOn = DateTime.Now;
            rsv.ChargeMultiplier = 1;
            rsv.ApplyLateChargePenalty = true;
            rsv.IsStarted = false;
            rsv.IsUnloaded = false;
            rsv.KeepAlive = false;

            Session.Insert(rsv);

            InsertReservationHistory("InsertFacilityDownTime", "procReservationInsert", rsv, modifiedByClientId);
        }

        public bool IsInvited(Reservation rsv, Client c)
        {
            return Session.Query<ReservationInvitee>().Any(x => x.Reservation.ReservationID == rsv.ReservationID && x.Invitee == c);
        }

        public void Start(Reservation rsv, int? startedByClientId, int? modifiedByClientId)
        {
            // This is all that happens in procReservationUpdate @Action = 'Start'

            //UPDATE dbo.Reservation
            //SET ActualBeginDateTime = GETDATE(),
            //    ClientIDBegin = @ClientID,
            //    IsStarted = 1		
            //WHERE ReservationID = @ReservationID

            rsv.ActualBeginDateTime = DateTime.Now;
            rsv.ClientIDBegin = startedByClientId;
            rsv.IsStarted = true;

            // also an entry into history is made
            InsertReservationHistory("Start", "procReservationUpdate", rsv, modifiedByClientId);
        }

        public void End(Reservation rsv, int? endedByClientId, int? modifiedByClientId)
        {
            rsv.ActualEndDateTime = DateTime.Now;
            rsv.ClientIDEnd = endedByClientId;

            if (rsv.ActualBeginDateTime.Value > rsv.ActualEndDateTime.Value)
                throw new Exception(string.Format("ActualDateTime is greater than ActualEndDateTime [ReservationID: {0}, ResourceID: {1}, ActualBeginDateTime: {2:yyyy-MM-dd HH:mm:ss}, ActualEndDateTime: {3:yyyy-MM-dd HH:mm:ss}", rsv.ReservationID, rsv.Resource.ResourceID, rsv.ActualBeginDateTime, rsv.ActualEndDateTime));

            if (!rsv.Activity.Editable)
                rsv.Resource.State = ResourceState.Online;

            InsertReservationHistory("End", "procReservationUpdate", rsv, modifiedByClientId);
        }

        public void EndForRepair(Reservation rsv, int? endedByClientId, int? modifiedByClientId)
        {
            // This is all that happens in procReservationUpdate @Action = 'EndForRepair'

            //UPDATE dbo.Reservation
            //SET ActualEndDateTime = GETDATE(),
            //    ChargeMultiplier = 0,
            //    ApplyLateChargePenalty = 0,
            //    ClientIDEnd = @ClientID
            //WHERE ReservationID = @ReservationID

            rsv.ActualEndDateTime = DateTime.Now;
            rsv.ChargeMultiplier = 0;
            rsv.ApplyLateChargePenalty = false;
            rsv.ClientIDEnd = endedByClientId;

            // also an entry into history is made
            InsertReservationHistory("EndForRepair", "procReservationUpdate", rsv, modifiedByClientId);
        }

        public void Update(Reservation rsv, int? modifiedByClientId)
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

            if (rsv.OriginalBeginDateTime.HasValue)
            {
                if (!rsv.OriginalModifiedOn.HasValue)
                    rsv.OriginalModifiedOn = DateTime.Now;
            }

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

            rsv.Duration = rsv.EndDateTime.Subtract(rsv.BeginDateTime).TotalMinutes;
            rsv.MaxReservedDuration = Math.Max(rsv.Duration, rsv.MaxReservedDuration);
            rsv.LastModifiedOn = DateTime.Now;

            // also an entry into history is made
            InsertReservationHistory("Update", "procReservationUpdate", rsv, modifiedByClientId);
        }

        public void Delete(Reservation rsv, int? modifiedByClientId)
        {
            // This is all that happens in procReservationDelete @Action = 'ByReservationID'

            //-- Set Reservation to inactive
            //UPDATE dbo.Reservation
            //SET IsActive = 0, LastModifiedOn = GETDATE(), CancelledDateTime = GETDATE()
            //WHERE ReservationID = @ReservationID

            rsv.IsActive = false;
            rsv.LastModifiedOn = DateTime.Now;
            rsv.CancelledDateTime = DateTime.Now;

            // also an entry into history is made
            InsertReservationHistory("ByReservationID", "procReservationDelete", rsv, modifiedByClientId);
        }

        public void UpdateCharges(Reservation rsv, double chargeMultiplier, bool applyLateChargePenalty, int? modifiedByClientId)
        {
            // procReservationUpdate @Action = 'UpdateCharges'

            //UPDATE dbo.Reservation
            //SET ChargeMultiplier = @ChargeMultiplier,
            //    ApplyLateChargePenalty = @ApplyLateChargePenalty
            //WHERE ReservationID = @ReservationID

            rsv.ChargeMultiplier = chargeMultiplier;
            rsv.ApplyLateChargePenalty = applyLateChargePenalty;

            // also an entry into history is made
            InsertReservationHistory("UpdateCharges", "procReservationUpdate", rsv, modifiedByClientId);
        }

        public void UpdateFacilityDownTime(Reservation rsv, int? modifiedByClientId)
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

            rsv.LastModifiedOn = DateTime.Now;

            // also an entry into history is made
            InsertReservationHistory("UpdateFacilityDownTime", "procReservationUpdate", rsv, modifiedByClientId);
        }
    }
}
