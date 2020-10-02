using System;

namespace LNF.Scheduler
{
    public class ReservationStateUtility
    {
        public static readonly ReservationState[] TruthTable = new[]
        {
            ReservationState.Other, //0
            ReservationState.Other, //1
            ReservationState.Other, //2
            ReservationState.Other, //3
            ReservationState.Other, //4
            ReservationState.Other, //5
            ReservationState.Other, //6
            ReservationState.Other, //7
            ReservationState.Invited, //8
            ReservationState.Invited, //9
            ReservationState.Invited, //10
            ReservationState.Invited, //11
            ReservationState.Invited, //12
            ReservationState.NotInLab, //13
            ReservationState.Invited, //14
            ReservationState.Editable, //15
            ReservationState.Undefined,
            ReservationState.UnAuthToStart,
            ReservationState.Editable,
            ReservationState.Editable,
            ReservationState.Undefined,
            ReservationState.NotInLab,
            ReservationState.Editable,
            ReservationState.Editable,
            ReservationState.Undefined,
            ReservationState.Undefined,
            ReservationState.Undefined,
            ReservationState.Undefined,
            ReservationState.Undefined,
            ReservationState.Undefined,
            ReservationState.Undefined,
            ReservationState.Undefined,
            ReservationState.Other,
            ReservationState.Other,
            ReservationState.Other,
            ReservationState.Other,
            ReservationState.Other,
            ReservationState.Other,
            ReservationState.Other,
            ReservationState.Other,
            ReservationState.Invited,
            ReservationState.Invited,
            ReservationState.Invited,
            ReservationState.Invited,
            ReservationState.Invited,
            ReservationState.StartOnly,
            ReservationState.Invited,
            ReservationState.StartOnly,
            ReservationState.Undefined,
            ReservationState.UnAuthToStart,
            ReservationState.Editable,
            ReservationState.Editable,
            ReservationState.Undefined,
            ReservationState.StartOnly,
            ReservationState.Editable,
            ReservationState.StartOrDelete,
            ReservationState.Undefined,
            ReservationState.Undefined,
            ReservationState.Undefined,
            ReservationState.Undefined,
            ReservationState.Undefined,
            ReservationState.Undefined,
            ReservationState.Undefined,
            ReservationState.Undefined
        };

        public static readonly ReservationState[] TruthTableTE = new[]
        {
            ReservationState.Undefined,
            ReservationState.StartOnly,
            ReservationState.Editable,
            ReservationState.StartOrDelete
        };

        public DateTime Now { get; }

        private ReservationStateUtility(DateTime now)
        {
            Now = now;
        }

        public static ReservationStateUtility Create(DateTime now)
        {
            return new ReservationStateUtility(now);
        }

        #region Truth Tables
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
        // 0  0  1  1  0  1  ReservationState.NotInLab (not inlab, not reserver, is invited, is authorized, not before mct, is startable)
        // 0  0  1  1  1  0  ReservationState.Invited
        // 0  0  1  1  1  1  ReservationState.Editable (not inlab, not reserver, is invited, is authorized, is before mct, is startable) [2020-09-30 jg] Changed from NotInLab to Editable so invitee can cancel outside of lab if before MCT (same as reserver)
        // 0  1  0  0  0  0  ReservationState.Undefined
        // 0  1  0  0  0  1  ReservationState.UnAuthToStart
        // 0  1  0  0  1  0  ReservationState.Editable
        // 0  1  0  0  1  1  ReservationState.Editable
        // 0  1  0  1  0  0  ReservationState.Undefined
        // 0  1  0  1  0  1  ReservationState.NotInLab (not inlab, is reserver, not invited, is authorized, not before mct, is startable)
        // 0  1  0  1  1  0  ReservationState.Editable
        // 0  1  0  1  1  1  ReservationState.Editable (not inlab, is reserver, not invited, is authorized, is before mct, is startable)
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
        #endregion

        public ReservationState GetReservationState(ReservationStateArgs args)
        {
            // Repair Reservations, returns immediately
            // 2008-08-15 temp (return Meeting if ReservationID is -1)
            if (args.IsRepair)
                return args.ReservationID == -1 
                    ? ReservationState.Meeting 
                    : ReservationState.Repair;

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

        public ReservationState GetUnstartedReservationState(ReservationStateArgs args)
        {
            bool isInLab = false;
            bool isReserver = false;
            bool isInvited = false;
            bool isAuthorized = false;
            bool isBeforeMinCancelTime = args.IsBeforeMinCancelTime(Now);
            bool isStartable = args.IsStartable(Now);

            if (!args.IsToolEngineer)
            {
                isInLab = args.IsInLab;
                isReserver = args.IsReserver;
                isInvited = args.IsInvited;
                isAuthorized = args.IsAuthorized;
            }

            var subStateValue = GetSubStateVal(isInLab, isReserver, isInvited, isAuthorized, isBeforeMinCancelTime, isStartable);

            var result = (args.IsToolEngineer)
                ? TruthTableTE[subStateValue]
                : TruthTable[subStateValue];

            if (result == ReservationState.Undefined)
            {
                var errmsg = "Unstarted reservation state is undefined."
                    + " ReservationID: {0}, IsToolEngineer: {1}, IsInLab: {2}, IsReserver: {3}, IsInvited: {4}, IsAuthorized: {5}, IsBeforeMinCancelTime: {6}, IsStartable: {7}, SubStateValue: {8}";

                throw new Exception(string.Format(errmsg,
                    args.ReservationID,
                    args.IsToolEngineer ? "Yes" : "No",
                    isInLab ? "Yes" : "No",
                    isReserver ? "Yes" : "No",
                    isInvited ? "Yes" : "No",
                    isAuthorized ? "Yes" : "No",
                    isBeforeMinCancelTime ? "Yes" : "No",
                    isStartable ? "Yes" : "No",
                    subStateValue));
            }

            return result;
        }

        public int GetSubStateVal(bool isInLab, bool isReserver, bool isInvited, bool isAuthorized, bool isBeforeMinCancelTime, bool isStartable)
        {
            return (isInLab ? 32 : 0)
                + (isReserver ? 16 : 0)
                + (isInvited ? 8 : 0)
                + (isAuthorized ? 4 : 0)
                + (isBeforeMinCancelTime ? 2 : 0)
                + (isStartable ? 1 : 0);
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
    }

}
