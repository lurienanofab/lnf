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
    public static class ReservationUtility
    {
        //public const int PREVIOUS_GRANULARITY = 0;
        //public const int FUTURE_GRANULARITY = 1;

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

        public static Reservation Create(Resource resource, Client client, Account account, Activity activity, DateTime beginDateTime, DateTime endDateTime, double duration, string notes, bool autoEnd, bool hasProcessInfo, bool hasInvitees, ReservationRecurrence recurrence, bool isActive, bool keepAlive, double maxReservedDuration, Client modifiedBy)
        {
            int? recurId = (recurrence == null) ? default(int?) : recurrence.RecurrenceID;

            return DA.Current.NamedQuery("CreateReservation", new
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

        public static Reservation FromDataRow(DataRow dr)
        {
            int id = 0;
            if (dr.Table.Columns.Contains("ReservationID") && int.TryParse(dr["ReservationID"].ToString(), out id))
                return DA.Scheduler.Reservation.Single(id);
            return null;
        }

        public static IList<Reservation> SelectAutoEnd()
        {
            return DA.Current.NamedQuery("SelectAutoEndReservations").List<Reservation>();
        }

        public static IList<Reservation> SelectPastEndableRepair()
        {
            return DA.Current.NamedQuery("SelectPastEndableRepairReservations").List<Reservation>();
        }

        public static IList<Reservation> SelectPastUnstarted()
        {
            return DA.Current.NamedQuery("SelectPastUnstartedReservations").List<Reservation>();
        }

        //Ends a past unstarted reservation
        public static int EndPastUnstarted(Reservation rsv, DateTime endDate, int clientId)
        {
            //ClientID might be -1
            return DA.Current.NamedQuery("EndPastUnstartedReservations", new
            {
                rsv.ReservationID,
                EndDateTime = endDate,
                ClientID = clientId
            }).Result<int>();
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
            string sql = "EXEC sselScheduler.dbo.procReservationSelect @Action = 'GetAvailableSchedMin', @ResourceID = :resourceId, @ClientID = :clientId";
            return DA.Current.SqlQuery(sql, new { resourceId, clientId }).Result<int>();
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
                        AutoEndLog.AddEntry(rsv, "unstarted");
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
                    AutoEndLog.AddEntry(rsv, "repair");
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

        public static ReservationState GetReservationState(int reservationId, int clientId, bool isInLab)
        {
            // Get Reservation Info
            var rsv = DA.Current.Single<Reservation>(reservationId);
            var client = DA.Current.Single<Client>(clientId);

            return GetReservationState(rsv, client, isInLab);
        }

        public static ReservationState GetReservationState(Reservation rsv, Client client, bool isInLab)
        {
            var args = ReservationStateArgs.Create(rsv, client, isInLab);

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

        public static ReservationState GetReservationState(ReservationStateArgs args)
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


        public static ReservationState GetUnstartedReservationState(DateTime beginDateTime, int minReservTime, bool isInLab, bool isEngineer, bool isReserver, bool isInvited, bool isAuthorized, bool isBeforeMinCancelTime)
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
            return IsStartable(DateTime.Now, beginDateTime, minReservTime);
        }

        public static bool IsStartable(DateTime now, DateTime beginDateTime, int minReservTime)
        {
            return (now > beginDateTime.AddMinutes(-1 * minReservTime));
        }

        public static void UpdateNotes(Reservation rsv, string notes)
        {
            rsv.Notes = notes;

            // there is no reason to add reservation history
            // because notes are not tracked in the ReservationHistory table
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
                    DisplayName = ClientItem.GetDisplayName(res.CurrentLastName, res.CurrentFirstName),
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
        public static async Task StartReservation(Reservation rsv, int clientId, bool isInLab)
        {
            IRepository repo = DA.Current;

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

        public static IList<Reservation> SelectHistory(int clientId, DateTime sd, DateTime ed)
        {
            var result = DA.Current.Query<Reservation>()
                .Where(x => x.Client.ClientID == clientId && (x.BeginDateTime < ed) && (x.EndDateTime > sd)).ToList();

            return result;
        }

        public static IList<Reservation> SelectHistoryToForgiveForRepair(int resourceId, DateTime sd, DateTime ed)
        {
            //[2013-05-20 jg] We no longer care if the reservation was canceled or not, all need to be forgiven
            //      because of the booking fee on uncancelled reservations.

            int repairActivityId = 14;

            var result = DA.Current.Query<Reservation>().Where(x =>
                x.Resource.ResourceID == resourceId
                && ((x.BeginDateTime < ed && x.EndDateTime > sd) || (x.ActualBeginDateTime < ed && x.ActualEndDateTime > sd)) //will include all overlapping reservations
                && x.Activity.ActivityID != repairActivityId).OrderBy(x => x.BeginDateTime).ToList();

            return result;
        }

        public static IList<Reservation> SelectOverwrittable(int resourceId, DateTime sd, DateTime ed)
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
                && x.BeginDateTime < ed
                && x.EndDateTime > sd
                && x.IsActive
                && x.ActualEndDateTime == null).ToList();

            return result;
        }

        public static int UpdateByGroup(int groupId, DateTime sd, DateTime ed, string notes, int? modifiedByClientId)
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
                rsv.BeginDateTime = sd;
                rsv.EndDateTime = ed;
                rsv.ActualBeginDateTime = sd;
                rsv.ActualEndDateTime = ed;
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

        public static ClientAuthLevel GetAuthLevel(IEnumerable<IAuthorized> resourceClients, IPrivileged client, int resourceId)
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
    }
}
