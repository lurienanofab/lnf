using LNF.Cache;
using LNF.CommonTools;
using LNF.Models.Data;
using LNF.Models.Scheduler;
using LNF.Repository.Scheduler;
using LNF.Scheduler.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public class ReservationUtility
    {
        public ReservationUtility(DateTime now, IProvider provider)
        {
            Now = now;
            Provider = provider;
        }

        public DateTime Now { get; }
        public IProvider Provider { get; }

        public static readonly ReservationState[] TruthTable = new[]
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

        public static readonly ReservationState[] TruthTableTE = new[]
        {
            ReservationState.Undefined, ReservationState.StartOnly, ReservationState.Editable, ReservationState.StartOrDelete
        };

        public static int GetSubStateVal(bool isInLab, bool isReserver, bool isInvited, bool isAuthorized, bool isBeforeMinCancelTime, bool isStartable)
        {
            return (isInLab ? 32 : 0)
                + (isReserver ? 16 : 0)
                + (isInvited ? 8 : 0)
                + (isAuthorized ? 4 : 0)
                + (isBeforeMinCancelTime ? 2 : 0)
                + (isStartable ? 1 : 0);
        }

        /// <summary>
        /// Starts a reservation, and turns on Interlock.
        /// </summary>
        /// <param name="item">The reservation to start</param>
        /// <param name="client">The current user starting the reservation</param>
        public void Start(IReservation item, ReservationClient client, int? modifiedByClientId)
        {
            if (item == null)
                throw new ArgumentNullException("item", "A null ReservationItem object is not allowed.");

            if (item.IsStarted) return;

            var args = ReservationStateArgs.Create(item, client);
            ReservationState state = GetReservationState(args);

            if (state != ReservationState.StartOnly && state != ReservationState.StartOrDelete)
                throw new Exception($"Reservation #{item.ReservationID} is not startable at this time. [State = {state}]");

            bool enableInterlock = CacheManager.Current.WagoEnabled;

            // End Previous un-ended reservations
            var endableRsvQuery = Provider.Scheduler.Reservation.SelectEndableReservations(item.ResourceID);
            foreach (var endableRsv in endableRsvQuery)
                Provider.Scheduler.Reservation.EndReservation(endableRsv.ReservationID, modifiedByClientId, modifiedByClientId);

            // Start Reservation
            Provider.Scheduler.Reservation.StartReservation(item.ReservationID, modifiedByClientId);

            // If Resource authorization type is rolling and the reserver is a regular user for the resource then reset reserver's expiration date
            int authLevel = 0, resourceClientId = 0;

            using (var reader = ResourceClientData.SelectResourceClient(item.ResourceID, client.ClientID))
            {
                if (reader.Read())
                {
                    authLevel = (int)reader["AuthLevel"];
                    resourceClientId = (int)reader["ResourceClientID"];
                }
            }

            if (item.AuthState && (authLevel == (int)ClientAuthLevel.AuthorizedUser))
            {
                DateTime expiration = Now.AddMonths(item.AuthDuration);
                ResourceClientData.UpdateExpiration(resourceClientId, expiration);
            }

            // Turn Interlock On
            if (enableInterlock)
            {
                uint duration = OnTheFlyUtility.GetStateDuration(item.ResourceID);
                bool hasInterlock = WagoInterlock.ToggleInterlock(item.ResourceID, true, duration);
                if (hasInterlock)
                {
                    bool interlockState = WagoInterlock.GetPointState(item.ResourceID);
                    if (!interlockState)
                        throw new InvalidOperationException($"Failed to start interlock for ResourceID {item.ResourceID}.");
                }
            }
        }

        public void End(IReservation rsv, int? endedByClientId, int? modifiedByClientId)
        {
            // Make sure this reservation hasn't already been ended some how
            // [2016-01-21 jg] Allow FacilityDownTime because they already have ActualEndDateTime set
            if (rsv.ActualEndDateTime != null && !rsv.IsFacilityDownTime)
                return;

            // End Reservation
            Provider.Scheduler.Reservation.EndReservation(rsv.ReservationID, endedByClientId, modifiedByClientId);

            // Turn Interlock Off
            if (CacheManager.Current.WagoEnabled)
                WagoInterlock.ToggleInterlock(rsv.ResourceID, false, 0);

            // Check for other open reservation slots between now and the reservation fence
            DateTime? nextBeginDateTime = OpenResSlot(rsv.ResourceID, TimeSpan.FromMinutes(rsv.ReservFence), TimeSpan.FromMinutes(rsv.MinReservTime), rsv.EndDateTime);

            if (nextBeginDateTime == null)
                return;

            // Get the next reservation start time
            DateTime currentEndDateTime = ResourceUtility.GetNextGranularity(rsv, rsv.ActualEndDateTime.Value, GranularityDirection.Previous);

            // Send email notifications to all clients who want to be notified of open reservation slots
            Provider.EmailManager.EmailOnOpenSlot(rsv, currentEndDateTime, nextBeginDateTime.Value, EmailNotify.Always, endedByClientId.GetValueOrDefault());

            if (nextBeginDateTime.Value.Subtract(currentEndDateTime).TotalMinutes >= rsv.MinReservTime)
                Provider.EmailManager.EmailOnOpenSlot(rsv, currentEndDateTime, nextBeginDateTime.Value, EmailNotify.OnOpening, endedByClientId.GetValueOrDefault());
        }

        public IReservation Modify(IReservation rsv, ReservationData data)
        {
            IReservation result;
            bool insert = false;

            if (IsFacilityDownTimeActivity(rsv.ActivityID))
            {
                result = Provider.Scheduler.Reservation.UpdateFacilityDownTime(rsv.ReservationID, data.Duration.BeginDateTime, data.Duration.EndDateTime, data.ClientID);
                HandleFacilityDowntimeReservation(result, data.ClientID);
            }
            else
            {
                if (CreateForModification(rsv, data.Duration))
                {
                    var args = GetInsertReservationArgs(data);
                    result = Provider.Scheduler.Reservation.InsertForModification(args, rsv);
                    Provider.Scheduler.Reservation.AppendNotes(rsv.ReservationID, $"Canceled for modification. New ReservationID: {rsv.ReservationID}");
                    insert = true;
                }
                else
                {
                    var args = GetUpdateReservationArgs(data, rsv.ReservationID);
                    result = Provider.Scheduler.Reservation.UpdateReservation(args);
                }

                HandlePracticeReservation(result, data.Invitees, data.ClientID);
            }

            if (insert)
            {
                InsertReservationInvitees(result.ReservationID, data.Invitees);
                InsertReservationProcessInfos(result.ReservationID, data.ProcessInfos);
            }
            else
            {
                UpdateReservationInvitees(data.Invitees);
                UpdateReservationProcessInfos(data.ProcessInfos);
            }

            Provider.EmailManager.EmailOnUserUpdate(result, data.ClientID);
            Provider.EmailManager.EmailOnInvited(result, data.Invitees, data.ClientID, ReservationModificationType.Modified);
            Provider.EmailManager.EmailOnUninvited(rsv, data.Invitees, data.ClientID);

            return result;
        }

        public IReservation Create(ReservationData data)
        {
            IReservation result;

            // This method is for creating "normal" reservations - not FDT or repair or recurring.

            if (IsFacilityDownTimeActivity(data.ActivityID))
                throw new Exception("Use LNF.Web.Scheduler.FacilityDownTimeUtility.InsertFacilityDownTime");

            if (IsRepairActivity(data.ActivityID))
                throw new Exception("Use LNF.Web.Scheduler.RepairUtility.StartRepair");

            var args = GetInsertReservationArgs(data);
            result = Provider.Scheduler.Reservation.InsertReservation(args);
            HandlePracticeReservation(result, data.Invitees, args.ModifiedByClientID);

            InsertReservationInvitees(result.ReservationID, data.Invitees);
            InsertReservationProcessInfos(result.ReservationID, data.ProcessInfos);

            Provider.EmailManager.EmailOnUserCreate(result, data.ClientID);
            Provider.EmailManager.EmailOnInvited(result, data.Invitees, data.ClientID);

            return result;
        }

        public void Delete(IReservation rsv, int? modifiedByClientId)
        {
            Provider.Scheduler.Reservation.CancelReservation(rsv.ReservationID, modifiedByClientId);

            // Send email to reserver and invitees
            Provider.EmailManager.EmailOnUserDelete(rsv, modifiedByClientId.GetValueOrDefault());
            Provider.EmailManager.EmailOnUninvited(rsv, Provider.Scheduler.Reservation.GetInvitees(rsv.ReservationID), modifiedByClientId.GetValueOrDefault());

            // Send email notifications to all clients want to be notified of open reservation slots
            Provider.EmailManager.EmailOnOpenSlot(rsv, rsv.BeginDateTime, rsv.EndDateTime, EmailNotify.Always, modifiedByClientId.GetValueOrDefault());

            // Check for other open reservation slots between now and the reservation fence and Get the next reservation start time
            DateTime? nextBeginDateTime = OpenResSlot(rsv.ResourceID, TimeSpan.FromMinutes(rsv.ReservFence), TimeSpan.FromMinutes(rsv.MinReservTime), rsv.EndDateTime);

            if (nextBeginDateTime.HasValue)
                Provider.EmailManager.EmailOnOpenSlot(rsv, rsv.BeginDateTime, nextBeginDateTime.Value, EmailNotify.OnOpening, modifiedByClientId.GetValueOrDefault());
        }

        public static bool CreateForModification(IReservation rsv, ReservationDuration rd)
        {
            // if Time And Duration modified create new reservation else change existing
            if (rsv.BeginDateTime != rd.BeginDateTime || rsv.Duration != rd.Duration.TotalMinutes)
                return true;
            else
                return false;
        }

        public InsertReservationArgs GetInsertReservationArgs(ReservationData data) => data.CreateInsertArgs(Now);

        public UpdateReservationArgs GetUpdateReservationArgs(ReservationData data, int reservationId) => data.CreateUpdateArgs(Now, reservationId);

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
            if (args.IsRepair) return ReservationState.Repair;

            if (args.ActualBeginDateTime == null && args.ActualEndDateTime == null)
            {
                // reservations that have not yet been started
                if (args.EndDateTime <= Now) // should never occur - if in the past, the actuals should exist
                {
                    if (args.IsReserver)
                        return ReservationState.PastSelf;
                    else
                        return ReservationState.PastOther;
                }

                // redefine min cancel time (MCT) for tool engineers - can edit up until scheduled start time
                return GetUnstartedReservationState(args);
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
                    if (args.ActualEndDateTime.HasValue && args.ActualEndDateTime.Value < Now && args.IsToolEngineer)
                        return ReservationState.PastSelf; // FDT reservation that has already ended
                    else if (args.BeginDateTime > Now && args.IsToolEngineer)
                        return ReservationState.Editable; // FDT reservation that has not started yet
                    else if (args.EndDateTime > Now && args.IsToolEngineer)
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

        public bool IsStartable(DateTime beginDateTime, int minReservTime)
        {
            return (Now > beginDateTime.AddMinutes(-1 * minReservTime));
        }

        public static bool IsStartable(ReservationState state)
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

        public ReservationState GetUnstartedReservationState(ReservationStateArgs args)
        {
            var isStartable = IsStartable(args.BeginDateTime, args.MinReservTime);

            var actual = (args.IsToolEngineer)
                 ? new { IsInLab = false, IsReserver = false, IsInvited = false, IsAuthorized = false, IsBeforeMinCancelTime = Now <= args.BeginDateTime, IsStartable = isStartable }
                 : new { args.IsInLab, args.IsReserver, args.IsInvited, args.IsAuthorized, args.IsBeforeMinCancelTime, IsStartable = isStartable };

            var subStateValue = GetSubStateVal(actual.IsInLab, actual.IsReserver, actual.IsInvited, actual.IsAuthorized, actual.IsBeforeMinCancelTime, actual.IsStartable);

            var result = (args.IsToolEngineer)
                ? TruthTableTE[subStateValue]
                : TruthTable[subStateValue];

            if (result == ReservationState.Undefined)
            {
                var errmsg = "Unstarted reservation state is undefined."
                    + "  IsToolEngineer: {1}, IsInLab: {2}, IsReserver: {3}, IsInvited: {4}, IsAuthorized: {5}, IsBeforeMinCancelTime: {6}, IsStartable: {7}, SubStateValue: {8}";

                throw new Exception(string.Format(errmsg,
                    args.IsToolEngineer ? "Yes" : "No",
                    actual.IsInLab ? "Yes" : "No",
                    actual.IsReserver ? "Yes" : "No",
                    actual.IsInvited ? "Yes" : "No",
                    actual.IsAuthorized ? "Yes" : "No",
                    actual.IsBeforeMinCancelTime ? "Yes" : "No",
                    actual.IsStartable ? "Yes" : "No",
                    subStateValue));
            }

            return result;
        }

        ///<summary>
        ///Ends any reservations that needs to be auto-ended. This includes both types of auto-ending: resource-centric and reservation-centric.
        ///</summary>
        public HandleAutoEndReservationsProcessResult HandleAutoEndReservations(IEnumerable<IReservation> items)
        {
            //End auto-end reservations, and turn off interlocks

            var result = new HandleAutoEndReservationsProcessResult()
            {
                ReservationsCount = items.Count()
            };

            foreach (var rsv in items)
            {
                try
                {
                    End(rsv, -1, -1);
                    AutoEndLog.AddEntry(rsv, "autoend");
                    result.Data.Add($"Ended auto-end reservation {rsv.ReservationID} for resource {rsv.ResourceID}");
                }
                catch (Exception ex)
                {
                    result.Data.Add($"***ERROR*** Failed to auto-end reservation {rsv.ReservationID} for resource {rsv.ResourceID}: {ex.Message}");
                }
            }

            return result;
        }

        ///<summary>
        ///Ends any repair reservations that are in the past.
        ///</summary>
        public HandleRepairReservationsProcessResult HandleRepairReservations(IEnumerable<IReservation> items)
        {
            //End past repair reservations
            var result = new HandleRepairReservationsProcessResult()
            {
                ReservationsCount = items.Count()
            };

            foreach (var rsv in items)
            {
                End(rsv, -1, -1);
                result.Data.Add($"Ended repair reservation {rsv.ReservationID}");

                //Reset resource state
                ResourceUtility.UpdateState(rsv.ResourceID, ResourceState.Online, string.Empty);
                AutoEndLog.AddEntry(rsv, "repair");
                result.Data.Add($"Set ResourceID {rsv.ResourceID} online");
            }

            return result;
        }

        ///<summary>
        /// Ends any reservations that the reserver fails to start before the grace period had ended.
        ///</summary>
        public HandleUnstartedReservationsProcessResult HandleUnstartedReservations(IEnumerable<IReservation> items)
        {
            //End unstarted reservations
            var result = new HandleUnstartedReservationsProcessResult()
            {
                ReservationsCount = items.Count()
            };

            foreach (var rsv in items)
            {
                DateTime oldEndDateTime = rsv.EndDateTime;
                DateTime newEndDateTime = rsv.BeginDateTime;

                bool endReservation = false;
                DateTime ed;

                if (rsv.KeepAlive)
                {
                    //KeepAlive: we don't care about GracePeriod, only AutoEnd
                    if (rsv.ResourceAutoEnd <= 0 || rsv.AutoEnd)
                        ed = rsv.EndDateTime;
                    else
                        ed = rsv.EndDateTime.AddMinutes(rsv.ResourceAutoEnd);

                    if (ed <= Now)
                    {
                        endReservation = true;
                        newEndDateTime = ed;
                    }
                }
                else
                {
                    //The end datetime will be the scheduled begin datetime plus the grace period
                    ed = newEndDateTime.AddMinutes(rsv.GracePeriod);
                    endReservation = true;
                    newEndDateTime = ed;
                }

                if (endReservation)
                {
                    Provider.Scheduler.Reservation.EndPastUnstarted(rsv.ReservationID, newEndDateTime, -1);
                    AutoEndLog.AddEntry(rsv, "unstarted");
                    result.Data.Add($"Unstarted reservation {rsv.ReservationID} was ended, KeepAlive = {rsv.KeepAlive}, Reservation.AutoEnd = {rsv.AutoEnd}, Resource.AutoEnd = {rsv.ResourceAutoEnd}, ed = '{ed}'");

                    DateTime? NextBeginDateTime = OpenResSlot(rsv.ResourceID, TimeSpan.FromMinutes(rsv.ReservFence), TimeSpan.FromMinutes(rsv.MinReservTime), oldEndDateTime);

                    //Check if reservation slot becomes big enough
                    if (NextBeginDateTime.HasValue)
                    {
                        bool sendEmail =
                            NextBeginDateTime.Value.Subtract(oldEndDateTime).TotalMinutes < rsv.MinReservTime
                            && NextBeginDateTime.Value.Subtract(newEndDateTime).TotalMinutes >= rsv.MinReservTime;

                        if (sendEmail)
                        {
                            Provider.EmailManager.EmailOnOpenReservations(rsv, newEndDateTime, NextBeginDateTime.Value);
                        }
                    }
                }
                else
                    result.Data.Add($"Unstarted reservation {rsv.ReservationID} was not ended, KeepAlive = {rsv.KeepAlive}, Reservation.AutoEnd = {rsv.AutoEnd}, Resource.AutoEnd = {rsv.ResourceAutoEnd}, ed = '{ed}'");
            }

            return result;
        }

        public DateTime? OpenResSlot(int resourceId, TimeSpan reservFence, TimeSpan minReservTime, DateTime sd)
        {
            var query = Provider.Scheduler.Reservation.SelectByResource(resourceId, Now, Now.Add(reservFence), false).OrderBy(x => x.BeginDateTime).ToList();

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

        public static IEnumerable<IReservation> GetConflictingReservations(IEnumerable<IReservation> items, DateTime sd, DateTime ed)
        {
            return items.Where(x => x.BeginDateTime < ed && x.EndDateTime > sd).ToList();
        }

        public static ReservationInProgress GetRepairReservationInProgress(IResourceTree res)
        {
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

        public static ClientAuthLevel GetAuthLevel(IEnumerable<IAuthorized> resourceClients, IPrivileged client)
        {
            if (client.HasPriv(ClientPrivilege.Administrator | ClientPrivilege.Developer))
                return ClientAuthLevel.ToolEngineer;

            var rc = resourceClients.FirstOrDefault(x => x.ClientID == client.ClientID || x.ClientID == -1);

            if (rc == null)
                return ClientAuthLevel.UnauthorizedUser;

            return rc.AuthLevel;
        }

        /// <summary>
        /// Compose the tooltip caption text for the specified reservation
        /// </summary>
        public static string GetReservationCaption(ReservationState state)
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
        /// Compose the tooltip text for the specified reservation
        /// </summary>
        public static string GetReservationToolTip(IReservation item, ReservationState state)
        {
            // Display Reservation info
            string toolTip = string.Empty;

            string displayName = item.DisplayName;

            if (state == ReservationState.PastOther || state == ReservationState.PastSelf)
                toolTip += string.Format("<div><b>Used by {0}</b></div>", displayName);
            else if (state == ReservationState.Repair)
                toolTip += string.Format("<div><b>Repaired by {0}</b></div>", displayName);
            else
                toolTip += string.Format("<div><b>Reserved by {0}</b></div>", displayName);

            if (state == ReservationState.Other || state == ReservationState.PastOther)
            {
                int clientId = item.ClientID;
                int accountId = item.AccountID;

                if (!string.IsNullOrEmpty(item.Phone))
                    toolTip += string.Format("<div><b>Phone: {0}</b></div>", item.Phone);

                if (!string.IsNullOrEmpty(item.Email))
                    toolTip += string.Format("<div><b>Email: {0}</b></div>", item.Email);
            }

            // reservations in the past should have actual times, but may not - need to check for this
            if (item.ActualBeginDateTime != null && item.ActualEndDateTime != null)
                toolTip += string.Format("<div><b>{0} - {1}</b></div>", item.ActualBeginDateTime.Value.ToShortTimeString(), item.ActualEndDateTime.Value.ToShortTimeString());
            else
                toolTip += string.Format("<div><b>{0} - {1}</b></div>", item.BeginDateTime.ToShortTimeString(), item.EndDateTime.ToShortTimeString());

            // Reservation Notes
            if (!string.IsNullOrEmpty(item.Notes))
            {
                if (item.Notes.Length > 255)
                    toolTip += string.Format("<hr><div><b>Notes:</b></div><div>{0}...</div>", item.Notes.Substring(0, 255));
                else
                    toolTip += string.Format("<hr><div><b>Notes:</b></div><div>{0}</div>", item.Notes);
            }

            // Reservation Process Info
            var processInfos = ServiceProvider.Current.Scheduler.ProcessInfo.GetReservationProcessInfos(item.ReservationID);

            if (processInfos.Count() > 0)
            {
                toolTip += "<hr><div><b>Process Info:</b></div>";
                foreach (var rpi in processInfos)
                {
                    toolTip += string.Format("<div>{0}: {1}: {2}</div>", rpi.ProcessInfoName, rpi.Param, rpi.Value);
                }
            }

            // Reservation Invitees
            var invitees = ServiceProvider.Current.Scheduler.Reservation.GetInvitees(item.ReservationID);

            if (invitees.Count() > 0)
            {
                toolTip += "<hr><div><b>Invitees:</b></div>";
                foreach (var i in invitees)
                {
                    toolTip += string.Format("<div>{0}</div>", i.DisplayName);
                }
            }

            toolTip += string.Format("<hr><div><em class=\"text-muted\">ReservationID: {0}</em></div>", item.ReservationID);

            return toolTip;
        }

        public void UpdateReservationInvitees(IEnumerable<IReservationInvitee> invitees)
        {
            if (invitees == null) return;

            var removed = invitees.Where(x => x.Removed).ToArray();

            if (removed.Length > 0)
            {
                foreach (var item in removed)
                    Provider.Scheduler.Reservation.DeleteReservationInvitee(item.ReservationID, item.InviteeID);
            }

            var invited = invitees.Where(x => !x.Removed).ToArray();

            if (invited.Length > 0)
            {
                foreach (var item in invited)
                    Provider.Scheduler.Reservation.InsertReservationInvitee(item.ReservationID, item.InviteeID);
            }
        }

        public void InsertReservationInvitees(int reservationId, IEnumerable<IReservationInvitee> invitees)
        {
            if (invitees == null) return;

            var invited = invitees.Where(x => !x.Removed).ToArray();

            if (invited.Length > 0)
            {
                foreach (var item in invited)
                {
                    item.ReservationID = reservationId;
                    Provider.Scheduler.Reservation.InsertReservationInvitee(item.ReservationID, item.InviteeID);
                }
            }
        }

        /// <summary>
        /// Saves any changes to the current process info session items.
        /// </summary>
        public void UpdateReservationProcessInfos(IEnumerable<IReservationProcessInfo> processInfos)
        {
            foreach (var item in processInfos)
                Provider.Scheduler.ProcessInfo.UpdateReservationProcessInfo(item);
        }

        /// <summary>
        /// Creates new process info records for the specified reservation using the current process info session items.
        /// </summary>
        public void InsertReservationProcessInfos(int reservationId, IEnumerable<IReservationProcessInfo> processInfos)
        {
            foreach (var item in processInfos)
            {
                if (item.ReservationID == reservationId)
                    throw new Exception("Tried to copy a ReservationProcessInfo to the same reservation. This will cause a duplicate record.");

                item.ReservationID = reservationId;

                Provider.Scheduler.ProcessInfo.InsertReservationProcessInfo(item);
            }
        }

        public bool IsFacilityDownTimeActivity(int activityId)
        {
            return activityId == Properties.Current.Activities.FacilityDownTime.ActivityID;
        }

        public bool IsRepairActivity(int activityId)
        {
            return activityId == Properties.Current.Activities.Repair.ActivityID;
        }

        public bool IsPracticeActivity(int activityId)
        {
            return activityId == Properties.Current.Activities.Practice.ActivityID;
        }

        public bool HandleFacilityDowntimeReservation(IReservation rsv, int modifiedByClientId)
        {
            // 2009-06-21 If it's Facility downtime, we must delete the reservations that has been made during that period
            if (IsFacilityDownTimeActivity(rsv.ActivityID))
            {
                // Find and Remove any un-started reservations made during time of repair
                var query = Provider.Scheduler.Reservation.SelectByResource(rsv.ResourceID, rsv.BeginDateTime, rsv.EndDateTime, false);

                foreach (var existing in query)
                {
                    // Only if the reservation has not begun
                    if (existing.ActualBeginDateTime == null)
                    {
                        Provider.Scheduler.Reservation.CancelReservation(existing.ReservationID, modifiedByClientId);
                        Provider.EmailManager.EmailOnCanceledByRepair(existing, true, "LNF Facility Down", "Facility is down, thus we have to disable the tool.", rsv.EndDateTime, modifiedByClientId);
                    }
                    else
                    {
                        // We have to disable all those reservations that have been activated by setting IsActive to 0. 
                        // The catch here is that we must compare the "Actual" usage time with the repair time because if the user ends the reservation before the repair starts, we still 
                        // have to charge the user for that reservation
                    }
                }

                return true;
            }
            else
                return false;
        }

        public bool HandlePracticeReservation(IReservation rsv, IEnumerable<IReservationInvitee> invitees, int modifiedByClientId)
        {
            // 2009-09-16 Practice reservation : we must also check if tool engineers want to receive the notify email
            if (IsPracticeActivity(rsv.ActivityID))
            {
                IReservationInvitee invitee = null;

                if (invitees != null)
                    invitee = invitees.FirstOrDefault();

                if (invitee == null)
                    throw new InvalidOperationException("A practice reservation must have at least one invitee.");

                Provider.EmailManager.EmailOnPracticeRes(rsv, invitee.DisplayName, modifiedByClientId);

                return true;
            }
            else
                return false;
        }
    }
}
