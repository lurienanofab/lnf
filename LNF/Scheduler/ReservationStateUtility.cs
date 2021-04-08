using System;

namespace LNF.Scheduler
{
    public class ReservationStateUtility
    {
        public static readonly ReservationState[] TruthTable = new[]
        {
            ReservationState.Other,         //[00]
            ReservationState.Other,         //[01]
            ReservationState.Other,         //[02]
            ReservationState.Other,         //[03]
            ReservationState.Other,         //[04]
            ReservationState.Other,         //[05]
            ReservationState.Other,         //[06]
            ReservationState.Other,         //[07]
            ReservationState.Invited,       //[08]
            ReservationState.Invited,       //[09]
            ReservationState.Invited,       //[10]
            ReservationState.Invited,       //[11]
            ReservationState.Invited,       //[12]
            ReservationState.NotInLab,      //[13]
            ReservationState.Invited,       //[14]
            ReservationState.NotInLab,      //[15]
            ReservationState.Undefined,     //[16]
            ReservationState.UnAuthToStart, //[17]
            ReservationState.Editable,      //[18]
            ReservationState.Editable,      //[19]
            ReservationState.Undefined,     //[20]
            ReservationState.NotInLab,      //[21]
            ReservationState.Editable,      //[22]
            ReservationState.Editable,      //[23]
            ReservationState.Undefined,     //[24]
            ReservationState.Undefined,     //[25]
            ReservationState.Undefined,     //[26]
            ReservationState.Undefined,     //[27]
            ReservationState.Undefined,     //[28]
            ReservationState.Undefined,     //[29]
            ReservationState.Undefined,     //[30]
            ReservationState.Undefined,     //[31]
            ReservationState.Other,         //[32]
            ReservationState.Other,         //[33]
            ReservationState.Other,         //[34]
            ReservationState.Other,         //[35]
            ReservationState.Other,         //[36]
            ReservationState.Other,         //[37]
            ReservationState.Other,         //[38]
            ReservationState.Other,         //[39]
            ReservationState.Invited,       //[40]
            ReservationState.Invited,       //[41]
            ReservationState.Invited,       //[42]
            ReservationState.Invited,       //[43]
            ReservationState.Invited,       //[44]
            ReservationState.StartOnly,     //[45]
            ReservationState.Invited,       //[46]
            ReservationState.StartOrDelete, //[47] [2020-10-08 jg] changed from StartOnly to StartOrDelete so invitee can cancel reservation
            ReservationState.Undefined,     //[48]
            ReservationState.UnAuthToStart, //[49]
            ReservationState.Editable,      //[50]
            ReservationState.Editable,      //[51]
            ReservationState.Undefined,     //[52]
            ReservationState.StartOnly,     //[53]
            ReservationState.Editable,      //[54]
            ReservationState.StartOrDelete, //[55]
            ReservationState.Undefined,     //[56]
            ReservationState.Undefined,     //[57]
            ReservationState.Undefined,     //[58]
            ReservationState.Undefined,     //[59]
            ReservationState.Undefined,     //[60]
            ReservationState.Undefined,     //[61]
            ReservationState.Undefined,     //[62]
            ReservationState.Undefined      //[63]
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
        // 0  0  0  0  0  0  ReservationState.Other         [00]
        // 0  0  0  0  0  1  ReservationState.Other         [01]
        // 0  0  0  0  1  0  ReservationState.Other         [02]
        // 0  0  0  0  1  1  ReservationState.Other         [03]
        // 0  0  0  1  0  0  ReservationState.Other         [04]
        // 0  0  0  1  0  1  ReservationState.Other         [05]
        // 0  0  0  1  1  0  ReservationState.Other         [06]
        // 0  0  0  1  1  1  ReservationState.Other         [07]
        // 0  0  1  0  0  0  ReservationState.Invited       [08]
        // 0  0  1  0  0  1  ReservationState.Invited       [09]
        // 0  0  1  0  1  0  ReservationState.Invited       [10]
        // 0  0  1  0  1  1  ReservationState.Invited       [11]
        // 0  0  1  1  0  0  ReservationState.Invited       [12]
        // 0  0  1  1  0  1  ReservationState.NotInLab      [13]
        // 0  0  1  1  1  0  ReservationState.Invited       [14]
        // 0  0  1  1  1  1  ReservationState.NotInLab      [15]
        // 0  1  0  0  0  0  ReservationState.Undefined     [16]
        // 0  1  0  0  0  1  ReservationState.UnAuthToStart [17]
        // 0  1  0  0  1  0  ReservationState.Editable      [18]
        // 0  1  0  0  1  1  ReservationState.Editable      [19]
        // 0  1  0  1  0  0  ReservationState.Undefined     [20]
        // 0  1  0  1  0  1  ReservationState.NotInLab      [21]
        // 0  1  0  1  1  0  ReservationState.Editable      [22]
        // 0  1  0  1  1  1  ReservationState.Editable      [23]
        // 0  1  1  0  0  0  ReservationState.Undefined     [24]
        // 0  1  1  0  0  1  ReservationState.Undefined     [25]
        // 0  1  1  0  1  0  ReservationState.Undefined     [26]
        // 0  1  1  0  1  1  ReservationState.Undefined     [27]
        // 0  1  1  1  0  0  ReservationState.Undefined     [28]
        // 0  1  1  1  0  1  ReservationState.Undefined     [29]
        // 0  1  1  1  1  0  ReservationState.Undefined     [30]
        // 0  1  1  1  1  1  ReservationState.Undefined     [31]
        // 1  0  0  0  0  0  ReservationState.Other         [32]
        // 1  0  0  0  0  1  ReservationState.Other         [33]
        // 1  0  0  0  1  0  ReservationState.Other         [34]
        // 1  0  0  0  1  1  ReservationState.Other         [35]
        // 1  0  0  1  0  0  ReservationState.Other         [36]
        // 1  0  0  1  0  1  ReservationState.Other         [37]
        // 1  0  0  1  1  0  ReservationState.Other         [38]
        // 1  0  0  1  1  1  ReservationState.Other         [39]
        // 1  0  1  0  0  0  ReservationState.Invited       [40]
        // 1  0  1  0  0  1  ReservationState.Invited       [41]
        // 1  0  1  0  1  0  ReservationState.Invited       [42]
        // 1  0  1  0  1  1  ReservationState.Invited       [43]
        // 1  0  1  1  0  0  ReservationState.Invited       [44]
        // 1  0  1  1  0  1  ReservationState.StartOnly     [45]
        // 1  0  1  1  1  0  ReservationState.Invited       [46]
        // 1  0  1  1  1  1  ReservationState.StartOrDelete [47] [2020-10-08 jg] changed from StartOnly to StartOrDelete so invitee can cancel reservation
        // 1  1  0  0  0  0  ReservationState.Undefined     [48]
        // 1  1  0  0  0  1  ReservationState.UnAuthToStart [49]
        // 1  1  0  0  1  0  ReservationState.Editable      [50]
        // 1  1  0  0  1  1  ReservationState.Editable      [51]
        // 1  1  0  1  0  0  ReservationState.Undefined     [52]
        // 1  1  0  1  0  1  ReservationState.StartOnly     [53]
        // 1  1  0  1  1  0  ReservationState.Editable      [54]
        // 1  1  0  1  1  1  ReservationState.StartOrDelete [55]
        // 1  1  1  0  0  0  ReservationState.Undefined     [56]
        // 1  1  1  0  0  1  ReservationState.Undefined     [57]
        // 1  1  1  0  1  0  ReservationState.Undefined     [58]
        // 1  1  1  0  1  1  ReservationState.Undefined     [59]
        // 1  1  1  1  0  0  ReservationState.Undefined     [60]
        // 1  1  1  1  0  1  ReservationState.Undefined     [61]
        // 1  1  1  1  1  0  ReservationState.Undefined     [62]
        // 1  1  1  1  1  1  ReservationState.Undefined     [63]
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

            bool throwErrorOnUndefined = false;

            if (throwErrorOnUndefined && result == ReservationState.Undefined)
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
