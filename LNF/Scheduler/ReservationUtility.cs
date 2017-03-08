using LNF.Cache;
using LNF.CommonTools;
using LNF.Data;
using LNF.Logging;
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
    public static class ReservationUtility
    {
        //public const int PREVIOUS_GRANULARITY = 0;
        //public const int FUTURE_GRANULARITY = 1;

        private static readonly ReservationState[] TruthTable = new[]
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

        private static readonly ReservationState[] TruthTableTE = new[]
        {
            ReservationState.Undefined, ReservationState.StartOnly, ReservationState.Editable, ReservationState.StartOrDelete
        };

        public static int GetSubStateVal(bool isInLab, bool isReserver, bool isInvited, bool isAuth, bool beforeMCT, bool isStartable)
        {
            return (isInLab ? 32 : 0) + (isReserver ? 16 : 0) + (isInvited ? 8 : 0) + (isAuth ? 4 : 0) + (beforeMCT ? 2 : 0) + (isStartable ? 1 : 0);
        }

        public static Reservation Create(Resource resource, Client client, Account account, Activity activity, DateTime beginDateTime, DateTime endDateTime, double duration, string notes, bool autoEnd, bool hasProcessInfo, bool hasInvitees, ReservationRecurrence recurrence, bool isActive, bool keepAlive, double maxReservedDuration, Client modifiedBy)
        {
            int? recurId = (recurrence == null) ? null : (int?)recurrence.RecurrenceID;
            return DA.Current.QueryBuilder()
                .AddParameter("ResourceID", resource.ResourceID)
                .AddParameter("ClientID", client.ClientID)
                .AddParameter("AccountID", account.AccountID)
                .AddParameter("ActivityID", activity.ActivityID)
                .AddParameter("BeginDateTime", beginDateTime)
                .AddParameter("EndDateTime", endDateTime)
                .AddParameter("Duration", duration)
                .AddParameter("Notes", notes)
                .AddParameter("AutoEnd", autoEnd)
                .AddParameter("HasProcessInfo", hasProcessInfo)
                .AddParameter("HasInvitees", hasInvitees)
                .AddParameter("RecurrenceID", recurId)
                .AddParameter("IsActive", isActive)
                .AddParameter("CreatedOn", DateTime.Now)
                .AddParameter("KeepAlive", keepAlive)
                .AddParameter("MaxReservedDuration", maxReservedDuration)
                .AddParameter("ModifiedByClientID", modifiedBy.ClientID)
                .NamedQuery("CreateReservation")
                .List<Reservation>()
                .FirstOrDefault();
        }

        public static Reservation FromDataRow(DataRow dr)
        {
            int id = 0;
            if (dr.Table.Columns.Contains("ReservationID") && int.TryParse(dr["ReservationID"].ToString(), out id))
                return DA.Scheduler.Reservation.Single(id);
            return null;
        }

        public static IList<Reservation> SelectAutoEnd()
        {
            return DA.Current.QueryBuilder().NamedQuery("SelectAutoEndReservations").List<Reservation>();
        }

        public static IList<Reservation> SelectPastEndableRepair()
        {
            return DA.Current.QueryBuilder().NamedQuery("SelectPastEndableRepairReservations").List<Reservation>();
        }

        public static IList<Reservation> SelectPastUnstarted()
        {
            return DA.Current.QueryBuilder().NamedQuery("SelectPastUnstartedReservations").List<Reservation>();
        }

        //Ends a past unstarted reservation
        public static int EndPastUnstarted(Reservation rsv, DateTime endDate, int clientId)
        {
            //ClientID might be -1
            return DA.Current
                .QueryBuilder()
                .ApplyParameters(new
                {
                    ReservationID = rsv.ReservationID,
                    EndDateTime = endDate,
                    ClientID = clientId
                })
                .NamedQuery("EndPastUnstartedReservations")
                .Update();
        }

        public static IList<Reservation> SelectExisting(Resource resource)
        {
            DateTime dtn = DateTime.Now;
            IList<Reservation> reservationsWithFutureEndDateTime = DA.Scheduler.Reservation.Query().Where(x => x.EndDateTime > DateTime.Now && x.ActualEndDateTime == null && x.Resource == resource).ToList();
            IList<Reservation> reservationsWithFutureEndDateTimeWhereChargeBeginDateTimeLessThanNow = reservationsWithFutureEndDateTime.Where(x => x.ChargeBeginDateTime() < dtn).ToList();
            return reservationsWithFutureEndDateTimeWhereChargeBeginDateTimeLessThanNow;
        }

        public static IList<Reservation> ReservationsInWindow(Resource resource, int minutes)
        {
            DateTime edate = DateTime.Now.AddMinutes(minutes);
            IList<Reservation> result = DA.Scheduler.Reservation.Query().Where(x => x.BeginDateTime >= DateTime.Now && x.BeginDateTime < edate && x.Resource == resource).ToList();
            return result;
        }

        public static int GetAvailableSchedMin(int resourceId, int clientId)
        {
            return DA.Current.QueryBuilder()
                .ApplyParameters(new { ResourceID = resourceId, ClientID = clientId })
                .SqlQuery("EXEC sselScheduler.dbo.procReservationSelect @Action = 'GetAvailableSchedMin', @ResourceID = :ResourceID, @ClientID = :ClientID")
                .Result<int>();
        }

        ///<summary>
        /// Ends any reservations that the reserver fails to start before the grace period had ended.
        ///</summary>
        public static void EndUnstartedReservations(IEnumerable<Reservation> reservations)
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
                        AutoEndLog.AddUnstartedEntry(rsv);
                        timer.AddData("Unstarted reservation {0} was ended, KeepAlive = {1}, Reservation.AutoEnd = {2}, Resource.AutoEnd = {3}, eDate = '{4}'", rsv.ReservationID, rsv.KeepAlive, rsv.AutoEnd, rsv.Resource.AutoEnd, ed);

                        DateTime? NextBeginDateTime = OpenResSlot(rsv.Resource.ResourceID, TimeSpan.FromMinutes(rsv.Resource.ReservFence), TimeSpan.FromMinutes(rsv.Resource.MinReservTime), DateTime.Now, oldEndDateTime);

                        //Check if reservation slot becomes big enough
                        if (NextBeginDateTime.HasValue)
                        {
                            if (NextBeginDateTime.Value.Subtract(oldEndDateTime).TotalMinutes < rsv.Resource.MinReservTime
                                && NextBeginDateTime.Value.Subtract(newEndDateTime).TotalMinutes >= rsv.Resource.MinReservTime)
                            {
                                EmailUtility.EmailOnOpenReservations(rsv.Resource.ResourceID, newEndDateTime, NextBeginDateTime.Value);
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
        public static void EndRepairReservations(IEnumerable<Reservation> reservations)
        {
            //End past repair reservations
            int count = reservations.Count();

            using (var timer = LogTaskTimer.Start("ReservationUtility.EndRepairReservations", "count = {0}", () => new object[] { count }))
            {
                foreach (Reservation rsv in reservations)
                {
                    rsv.End(-1, -1);
                    timer.AddData("Ended repair reservation {0}", rsv.ReservationID);

                    //Reset resource state
                    ResourceUtility.UpdateState(rsv.Resource.ResourceID, ResourceState.Online, string.Empty);
                    AutoEndLog.AddRepairEntry(rsv);
                    timer.AddData("Set ResourceID {0} online", rsv.Resource.ResourceID);
                }
            }
        }

        ///<summary>
        ///Ends any reservations that needs to be auto-ended. This includes both types of auto-ending: resource-centric and reservation-centric.
        ///</summary>
        public static async Task EndAutoEndReservations(IEnumerable<Reservation> reservations)
        {
            //End auto-end reservations, and turn off interlocks

            int count = reservations.Count();

            using (var timer = LogTaskTimer.Start("ReservationUtility.EndAutoEndReservations", "count = {0}", () => new object[] { count }))
            {
                foreach (Reservation rsv in reservations)
                {
                    try
                    {
                        rsv.End(-1, -1);
                        await WagoInterlock.ToggleInterlock(rsv.Resource.ResourceID, false, 0); //always pass 0 when ending auto-end reservations
                        AutoEndLog.AddAutoEndEntry(rsv);
                        timer.AddData("Ended auto-end reservation {0} for resource {1}", rsv.ReservationID, rsv.Resource.ResourceID);
                    }
                    catch (Exception ex)
                    {
                        timer.AddData("***ERROR*** Failed to auto-end reservation {0} for resource {1}: {2}", rsv.ReservationID, rsv.Resource.ResourceID, ex.Message);
                    }
                }
            }
        }

        public static DateTime? OpenResSlot(int resourceId, TimeSpan reservFence, TimeSpan minReservTime, DateTime now, DateTime sd)
        {
            var query = DA.Scheduler.Reservation.SelectByResource(resourceId, now, now.Add(reservFence), false).OrderBy(x => x.BeginDateTime).ToList();

            for (int j = 1; j < query.Count - 1; j++)
            {
                // If there are other open reservation slots, then don't email reserver
                var curBeginDateTime = query[j].BeginDateTime;
                var lastEndDateTime = query[j - 1].EndDateTime;
                if (curBeginDateTime.Subtract(lastEndDateTime) >= minReservTime)
                    return null;
            }

            var followingReservations = from x in query
                                        where x.BeginDateTime >= sd
                                        orderby x.BeginDateTime
                                        select x;

            if (followingReservations.Count() == 0)
                // There are no other reservations behind it
                return null;
            else
                return followingReservations.First().BeginDateTime;
        }

        /// <summary>
        /// Ends a reservation, and turn off Interlock
        /// </summary>
        public static async Task EndReservation(int reservationId)
        {
            var rsv = DA.Current.Single<Reservation>(reservationId);

            // Make sure this reservation hasn't already been ended some how
            // [2016-01-21 jg] Allow FacilityDownTime because they already have ActualEndDateTime set
            if (rsv.ActualEndDateTime != null && !rsv.Activity.IsFacilityDownTime)
                return;

            // End Reservation
            int clientId = CacheManager.Current.CurrentUser.ClientID;
            rsv.End(clientId, clientId);

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
            EmailUtility.EmailOnOpenSlot(rsv.Resource.ResourceID, currentEndDateTime, nextBeginDateTime.Value, EmailNotify.Always, reservationId);

            if (nextBeginDateTime.Value.Subtract(currentEndDateTime).TotalMinutes >= rsv.Resource.MinReservTime)
                EmailUtility.EmailOnOpenSlot(rsv.Resource.ResourceID, currentEndDateTime, nextBeginDateTime.Value, EmailNotify.OnOpening, reservationId);
        }

        public static ReservationState GetReservationState(int reservationId, int clientId)
        {
            // Get Reservation Info
            var rsv = DA.Current.Single<Reservation>(reservationId);

            // Repair Reservations, returns immediately
            if (!rsv.Activity.Editable) return ReservationState.Repair;

            // Determine Ownership
            bool isReserver = rsv.Client.ClientID == clientId;

            // Determine Invition
            bool isInvited = DA.Current.Query<ReservationInvitee>().Any(x => x.Reservation.ReservationID == reservationId && x.Invitee.ClientID == clientId);

            // Determine Authorization
            ClientAuthLevel userAuth = CacheManager.Current.GetAuthLevel(rsv.Resource.ResourceID, clientId);
            bool isAuthorized = (userAuth & (ClientAuthLevel)rsv.Activity.StartEndAuth) > 0;
            bool isEngineer = (userAuth & ClientAuthLevel.ToolEngineer) > 0;

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

            if (rsv.ActualBeginDateTime == null && rsv.ActualEndDateTime == null)
            {
                // reservations that have not yet been started
                if (rsv.EndDateTime <= DateTime.Now) // should never occur - if in the past, the actuals should exist
                {
                    if (isReserver)
                        return ReservationState.PastSelf;
                    else
                        return ReservationState.PastOther;
                }

                // Get Resource Info
                var res = rsv.Resource;

                CacheManager.Current.CheckSession();

                bool isInLab = ClientInLab(reservationId);

                bool beforeMinCancelTime = (DateTime.Now <= rsv.BeginDateTime.AddMinutes(-1 * res.MinCancelTime));
                int graceSeconds = 60 * res.GracePeriod + 10;
                bool isStartable = (DateTime.Now > rsv.BeginDateTime.AddMinutes(-1 * res.MinReservTime));

                // redefine MCT for tool engineers - can edit up until scheduled start time                
                ReservationState result = GetUnstartedReservationState(rsv.BeginDateTime, res.MinReservTime, isInLab, isEngineer, isReserver, isInvited, isAuthorized, beforeMinCancelTime);

                if (result == ReservationState.Undefined)
                    throw new Exception(string.Format("Unable to determine reservation state for ReservationID: {0}, BeginDateTime: {1:yyyy-MM-dd HH:mm:ss}, ResourceID: {2}, isReserver: {3}, isInvited: {4}, isAuthorized: {5}, beforeMinCancelTime: {6}", reservationId, rsv.BeginDateTime, res.ResourceID, isReserver, isInvited, isAuthorized, beforeMinCancelTime));

                return result;
            }
            else if (rsv.ActualBeginDateTime != null && rsv.ActualEndDateTime == null)
            {
                // reservations that have been started
                if (isReserver || isEngineer)
                    return ReservationState.Endable;
                else if (isInvited)
                {
                    if (userAuth != ClientAuthLevel.UnauthorizedUser && userAuth != ClientAuthLevel.RemoteUser)
                        //2008-06-26 Sandrine requested that invitee should be able to end the reservation
                        return ReservationState.Endable;
                    else
                        return ReservationState.Invited;
                }
                else
                    return ReservationState.ActiveNotEndable;
            }
            else if (rsv.ActualEndDateTime != null)
            {
                // reservations in the past OR it's Facility Down Time reservation
                if (rsv.Activity.IsFacilityDownTime)
                {
                    // Facility Down Time, it must be editable if it's not started yet
                    if (rsv.ActualEndDateTime.HasValue && rsv.ActualEndDateTime.Value < DateTime.Now && isEngineer)
                        return ReservationState.PastSelf; // FDT reservation that has already ended
                    else if (rsv.BeginDateTime > DateTime.Now && isEngineer)
                        return ReservationState.Editable; // FDT reservation that has not started yet
                    else if (rsv.EndDateTime > DateTime.Now && isEngineer)
                        return ReservationState.Endable; //it's endable only if it's not ended yet
                    else
                        return ReservationState.Other;
                }

                if (isReserver)
                    return ReservationState.PastSelf;
                else
                    return ReservationState.PastOther;
            }

            throw new Exception(string.Format("Unable to determine reservation state for ReservationID: {0}", reservationId));
        }

        public static ReservationState GetUnstartedReservationState(DateTime beginDateTime, int minReservTime, bool isInLab, bool isEngineer, bool isReserver, bool isInvited, bool isAuthorized, bool beforeMinCancelTime)
        {
            bool isStartable = IsStartable(beginDateTime, minReservTime);

            if (isEngineer)
                return TruthTableTE[GetSubStateVal(false, false, false, false, DateTime.Now <= beginDateTime, isStartable)];
            else
                return TruthTable[GetSubStateVal(isInLab, isReserver, isInvited, isAuthorized, beforeMinCancelTime, isStartable)];
        }

        public static bool ClientInLab(int reservationId)
        {
            var rsv = DA.Current.Single<Reservation>(reservationId);
            int labId = rsv.Resource.ProcessTech.Lab.LabID;
            return CacheManager.Current.ClientInLab(labId);
        }

        /// <summary>
        /// Deletes a reservation
        /// </summary>
        public static void DeleteReservation(int reservationId)
        {
            // Get Reservation Info and Resource Info
            var rsv = DA.Scheduler.Reservation.Single(reservationId);
            var res = rsv.Resource;

            // Delete reservation
            rsv.Delete(CacheManager.Current.CurrentUser.ClientID);

            // Send email to reserver and invitees
            EmailUtility.EmailOnUserDelete(rsv);
            EmailUtility.EmailOnUninvited(rsv, rsv.GetInvitees().Select(ReservationInviteeItem.Create));

            // Send email notifications to all clients want to be notified of open reservation slots
            EmailUtility.EmailOnOpenSlot(res.ResourceID, rsv.BeginDateTime, rsv.EndDateTime, EmailNotify.Always, reservationId);

            // Check for other open reservation slots between now and the reservation fence and Get the next reservation start time
            DateTime? nextBeginDateTime = OpenResSlot(res.ResourceID, TimeSpan.FromMinutes(res.ReservFence), TimeSpan.FromMinutes(res.MinReservTime), DateTime.Now, rsv.EndDateTime);

            if (nextBeginDateTime.HasValue)
                EmailUtility.EmailOnOpenSlot(res.ResourceID, rsv.BeginDateTime, nextBeginDateTime.Value, EmailNotify.OnOpening, reservationId);
        }

        public static Reservation ModifyExistingReservation(Reservation rsv, ReservationDuration rd)
        {
            var CurrentUser = CacheManager.Current.CurrentUser;

            bool insert;
            var result = GetReservationForModification(rsv, rd, out insert);

            UpdateReservation(result, rd);

            if (HandleFacilityDowntimeResrvation(result))
                result.UpdateFacilityDownTime(CurrentUser.ClientID);
            else
            {
                if (insert)
                {
                    result.InsertForModification(rsv.ReservationID, CurrentUser.ClientID);
                    rsv.AppendNotes(string.Format("Canceled for modification. New ReservationID: {0}", rsv.ReservationID));
                }
                else
                    result.Update(CurrentUser.ClientID);

                HandlePracticeReservation(result);
            }

            ReservationInviteeData.Update(CopyReservationInviteesTable(), result.ReservationID);
            ReservationInviteeData.Update(CopyReservationProcessInfoTable(), result.ReservationID);

            EmailUtility.EmailOnUserUpdate(result);
            EmailUtility.EmailOnInvited(result, CacheManager.Current.ReservationInvitees(), EmailUtility.ReservationModificationType.Modified);
            EmailUtility.EmailOnUninvited(rsv, CacheManager.Current.RemovedInvitees());

            return result;
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

        public static bool IsStartable(DateTime beginDateTime, int minReservTime)
        {
            return (DateTime.Now > beginDateTime.AddMinutes(-1 * minReservTime));
        }

        public static void UpdateNotes(Reservation rsv, string notes)
        {
            rsv.Notes = notes;
            //there is no reason to add reservation history because notes are not tracked in the ReservationHistory table
        }

        public static void UpdateCharges(Reservation rsv, double chargeMultiplier)
        {
            rsv.ChargeMultiplier = chargeMultiplier;
            AddReservationHistory("ReservationUtility", "UpdateCharges", rsv);
        }

        public static void UpdateAccount(Reservation rsv, int accountId)
        {
            rsv.Account = DA.Current.Single<Account>(accountId);
            AddReservationHistory("ReservationUtility", "UpdateAccount", rsv);
        }

        public static DateTime GetBeginDateTime(Reservation rsv)
        {
            return (rsv.ActualBeginDateTime == null) ? rsv.BeginDateTime : rsv.ActualBeginDateTime.Value;
        }

        public static DateTime GetEndDateTime(Reservation rsv)
        {
            return (rsv.ActualEndDateTime == null) ? rsv.EndDateTime : rsv.ActualEndDateTime.Value;
        }

        public static ReservationInProgress GetRepairReservationInProgress(int resourceId)
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
                    DisplayName = Client.GetDisplayName(res.CurrentLastName, res.CurrentFirstName),
                    Editable = res.CurrentActivityEditable,
                    Notes = res.CurrentNotes,
                    ResourceID = res.ResourceID,
                    ResourceName = res.ResourceName
                };
            }

            return null;
        }

        public static IList<Reservation> SelectReservationsByPeriod(DateTime period)
        {
            DateTime sdate = period;
            DateTime edate = sdate.AddMonths(1);

            IList<Reservation> result = DA.Scheduler.Reservation.Query()
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

        public static IList<Reservation> SelectByDateRange(DateTime startDate, DateTime endDate, int clientId = 0)
        {
            IList<Reservation> result = DA.Scheduler.Reservation.Query().Where(x =>
                ((x.BeginDateTime < endDate && x.EndDateTime > startDate) ||
                (x.ActualBeginDateTime < endDate && x.ActualEndDateTime > startDate)) &&
                x.Client.ClientID == (clientId > 0 ? clientId : x.Client.ClientID)
            ).ToList();

            return result;
        }

        public static ReservationHistory AddReservationHistory(string actionSource, string userAction, Reservation rsv, Client modifiedBy = null)
        {
            if (modifiedBy == null)
                modifiedBy = DA.Current.Single<Client>(CacheManager.Current.ClientID);

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

            DA.Current.Insert(hist);

            return hist;
        }

        /// <summary>
        /// Compose the tooltip text for the specified reservation
        /// </summary>
        public static string GetReservationToolTip(Reservation rsv, ReservationState state)
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

        //public static string GetReservationClass(ReservationState state)
        //{
        //    switch (state)
        //    {
        //        case ReservationState.Undefined:
        //            return "ReservUndefined";
        //        case ReservationState.Editable:
        //            return "ReservEditable";
        //        case ReservationState.StartOrDelete:
        //            return "ReservStartOrDelete";
        //        case ReservationState.StartOnly:
        //            return "ReservStartOnly";
        //        case ReservationState.Endable:
        //            return "ReservEndable";
        //        case ReservationState.PastSelf:
        //            return "ReservPastSelf";
        //        case ReservationState.Other:
        //            return "ReservOther";
        //        case ReservationState.Invited:
        //            return "ReservInvited";
        //        case ReservationState.PastOther:
        //            return "ReservPastOther";
        //        case ReservationState.Repair:
        //        case ReservationState.Meeting:
        //            return "ReservRepair";
        //        case ReservationState.NotInLab:
        //            return "ReservNotInLab";
        //        case ReservationState.UnAuthToStart:
        //            return "ReservUnAuth";
        //        case ReservationState.ActiveNotEndable:
        //            return "ReservNotEndable";
        //        default:
        //            return string.Empty;
        //    }
        //}

        /// <summary>
        /// Starts a reservation, and turn on Interlock
        /// </summary>
        /// <param name="rsv">The reservation to start</param>
        /// <param name="clientId">The ClientID of the current user starting the reservation</param>
        public static async Task StartReservation(Reservation rsv, int clientId)
        {
            IRepository repo = DA.Current;

            if (rsv == null)
                throw new ArgumentNullException("rsv", "A null Reservation object is not allowed.");

            if (rsv.Resource == null)
                throw new ArgumentNullException("rsv", "A null Resource object is not allowed.");

            if (rsv.IsStarted) return;

            ReservationState state = GetReservationState(rsv.ReservationID, clientId);

            if (state != ReservationState.StartOnly && state != ReservationState.StartOrDelete)
                throw new Exception(string.Format("Reservation #{0} is not startable at this time. [State = {1}]", rsv.ReservationID, state));

            bool enableInterlock = CacheManager.Current.WagoEnabled;

            // End Previous un-ended reservations
            var endableRsvQuery = DA.Scheduler.Reservation.SelectEndableReservations(rsv.Resource.ResourceID);
            foreach (var endableRsv in endableRsvQuery)
                endableRsv.End(clientId, clientId);

            // Start Reservation
            rsv.Start(clientId, clientId);

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

        public static Reservation GetReservationForModification(Reservation rsv, ReservationDuration rd, out bool insert)
        {
            throw new NotImplementedException();
        }

        public static void UpdateReservation(Reservation rsv, ReservationDuration rd)
        {
            throw new NotImplementedException();
        }

        public static bool HandleFacilityDowntimeResrvation(Reservation rsv)
        {
            throw new NotImplementedException();
        }

        public static bool HandlePracticeReservation(Reservation rsv)
        {
            throw new NotImplementedException();
        }

        public static DataTable CopyReservationInviteesTable()
        {
            throw new NotImplementedException();
        }

        public static DataTable CopyReservationProcessInfoTable()
        {
            throw new NotImplementedException();
        }

        public static IList<Reservation> SelectByGroup(int groupId)
        {
            // procReservationSelect @Action = 'ByGroupID'

            //SELECT res.ResourceID, res.ResourceName
            //FROM Reservation r
            //INNER JOIN [Resource] res ON res.ResourceID = r.ResourceID
            //WHERE r.GroupID = @GroupID

            IList<Reservation> result = DA.Scheduler.Reservation.Query().Where(x => x.GroupID == groupId).ToList();

            return result;
        }

        public static IList<Reservation> SelectHistory(int clientId, DateTime startDate, DateTime endDate)
        {
            IList<Reservation> result = DA.Scheduler.Reservation.Query()
                .Where(x => x.Client.ClientID == clientId && (x.BeginDateTime < endDate) && (x.EndDateTime > startDate)).ToList();

            return result;
        }

        public static IList<Reservation> SelectHistoryToForgiveForRepair(int resourceId, DateTime startDate, DateTime endDate)
        {
            //[2013-05-20 jg] We no longer care if the reservation was cancelled or not, all need to be forgiven
            //because of the booking fee on uncancelled reservations.

            int repairActivityID = 14;

            IList<Reservation> result = DA.Scheduler.Reservation.Query().Where(x =>
                x.Resource.ResourceID == resourceId
                && (x.BeginDateTime < endDate && x.EndDateTime > startDate) //will include all overlapping reservations
                && x.Activity.ActivityID != repairActivityID).OrderBy(x => x.BeginDateTime).ToList();

            return result;
        }

        public static IList<Reservation> SelectOverwrittable(int resourceId, DateTime beginDateTime, DateTime endDateTime)
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

            IList<Reservation> result = DA.Scheduler.Reservation.Query().Where(x =>
                x.Resource.ResourceID == resourceId
                && x.BeginDateTime < endDateTime
                && x.EndDateTime > beginDateTime
                && x.IsActive
                && x.ActualEndDateTime == null).ToList();

            return result;
        }

        public static int UpdateByGroup(int groupId, DateTime beginDateTime, DateTime endDateTime, string notes, int? modifiedByClientId)
        {
            // This is all that happens in procReservationUpdate @Action = 'ByGroupID'

            //UPDATE dbo.Reservation
            //SET BeginDateTime = @BeginDateTime,
            //    EndDateTime = @EndDateTime,
            //    ActualBeginDateTime = @BeginDateTime,
            //    ActualEndDateTime = @EndDateTime
            //WHERE GroupID = @GroupID

            IList<Reservation> query = DA.Scheduler.Reservation.Query().Where(x => x.GroupID == groupId).ToList();

            foreach (Reservation rsv in query)
            {
                rsv.BeginDateTime = beginDateTime;
                rsv.EndDateTime = endDateTime;
                rsv.ActualBeginDateTime = beginDateTime;
                rsv.ActualEndDateTime = endDateTime;
                rsv.AppendNotes(notes);

                // also an entry into history is made for each reservation
                DA.Scheduler.ReservationHistory.Insert("ByGroupID", "procReservationUpdate", rsv, modifiedByClientId);
            }

            return query.Count;
        }

        public static DateTime? SelectLastRepairEndTime(int resourceId)
        {
            // This is all that happens in procReservationSelect @Action = 'SelectLastRepairReservEndTime'

            //SELECT TOP 1 Rv.ActualEndDateTime
            //FROM dbo.Reservation Rv, dbo.Activity A
            //WHERE Rv.ActivityID = A.ActivityID
            //    AND A.Editable = 0
            //    AND Rv.ResourceID = @ResourceID
            //    AND Rv.ActualEndDateTime < GETDATE()
            //ORDER BY Rv.ActualEndDateTime DESC

            Reservation rsv = DA.Scheduler.Reservation.Query().Where(x =>
                !x.Activity.Editable
                && x.Resource.ResourceID == resourceId
                && (x.ActualEndDateTime != null && x.ActualEndDateTime < DateTime.Now)
            ).OrderByDescending(x => x.ActualEndDateTime).FirstOrDefault();

            if (rsv != null)
                return rsv.ActualEndDateTime;
            else
                return null;
        }

        public static IList<ReservationHistoryFilterItem> FilterCancelledReservations(IList<Reservation> reservations, bool includeCanceledForModification)
        {
            if (reservations == null || reservations.Count == 0)
                return new List<ReservationHistoryFilterItem>();

            int minReservationId = reservations.Min(x => x.ReservationID);

            IList<ReservationHistory> hist = DA.Scheduler.ReservationHistory.Query().Where(x => x.LinkedReservationID.HasValue && x.LinkedReservationID.Value >= minReservationId && x.UserAction == ReservationHistory.INSERT_FOR_MODIFICATION).ToList();
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

        public static IList<Reservation> GetCurrentReservations()
        {
            return DA.Scheduler.Reservation.Query()
                .Where(x => x.IsActive && x.IsStarted && x.ActualBeginDateTime != null && x.ActualEndDateTime == null)
                .OrderBy(x => x.ActualBeginDateTime)
                .ToList();
        }

        public static IList<Reservation> GetConflictingReservations(IEnumerable<Reservation> reservations, DateTime sd, DateTime ed)
        {
            var result = reservations.Where(x => x.BeginDateTime < ed && x.EndDateTime > sd).ToList();
            return result;
        }

        public static double SelectReservableMinutes(int resourceId, int clientId, TimeSpan reservFence, TimeSpan maxAlloc, DateTime now)
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

            var query = DA.Current.Query<Reservation>().Where(x =>
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
    }
}
