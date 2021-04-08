using System;

namespace LNF.Scheduler
{
    public struct ReservationStateArgs
    {
        public int ReservationID { get; }
        public bool IsInLab { get; }
        public bool IsReserver { get; }
        public bool IsInvited { get; }
        public bool IsAuthorized { get; }
        public bool IsRepair { get; }
        public bool IsFacilityDownTime { get; }
        public int MinCancelTime { get; }
        public int MinReservTime { get; }
        public DateTime BeginDateTime { get; }
        public DateTime EndDateTime { get; }
        public DateTime? ActualBeginDateTime { get; }
        public DateTime? ActualEndDateTime { get; }
        public ClientAuthLevel UserAuth { get; }
        public bool IsToolEngineer => (UserAuth & ClientAuthLevel.ToolEngineer) > 0;

        public ReservationStateArgs(int reservationId, bool inlab, bool isReserver, bool isInvited, bool isAuthorized, bool isRepair, bool isFacilityDownTime, int minCancelTime, int minReservTime, DateTime beginDateTime, DateTime endDateTime, DateTime? actualBeginDateTime, DateTime? actualEndDateTime, ClientAuthLevel userAuth)
        {
            ReservationID = reservationId;
            IsInLab = inlab;
            IsReserver = isReserver;
            IsInvited = isInvited;
            IsAuthorized = isAuthorized;
            IsRepair = isRepair;
            IsFacilityDownTime = isFacilityDownTime;
            MinCancelTime = minCancelTime;
            MinReservTime = minReservTime;
            BeginDateTime = beginDateTime;
            EndDateTime = endDateTime;
            ActualBeginDateTime = actualBeginDateTime;
            ActualEndDateTime = actualEndDateTime;
            UserAuth = userAuth;
        }

        public static ReservationStateArgs Create(IReservationItem rsv, ReservationClient client, DateTime now)
        {
            var isAuthorized = client.IsAuthorized(rsv);
            var args = new ReservationStateArgs(rsv.ReservationID, client.InLab, client.IsReserver, client.IsInvited, isAuthorized, !rsv.Editable, rsv.IsFacilityDownTime, rsv.MinCancelTime, rsv.MinReservTime, rsv.BeginDateTime, rsv.EndDateTime, rsv.ActualBeginDateTime, rsv.ActualEndDateTime, client.UserAuth);
            return args;
        }

        public bool IsBeforeMinCancelTime() => IsBeforeMinCancelTime(DateTime.Now);

        public bool IsBeforeMinCancelTime(DateTime now)
        {
            // [2020-09-29 jg]
            // Per discussion in staff meeting. Tool engineers can cancel their own reservation even
            // after MinCancelTime (as long as the reservation is startable).

            if (IsToolEngineer)
                return true;

            if (MinCancelTime == 0)
            {
                if (now < BeginDateTime)
                    return true;
                else
                    return IsStartable(now);
            }

            return (now <= BeginDateTime.AddMinutes(-1 * MinCancelTime));
        }

        public bool IsStartable() => IsStartable(DateTime.Now);

        public bool IsStartable(DateTime now)
        {
            var startableDateTime = BeginDateTime.AddMinutes(-1 * MinReservTime);
            return now >= startableDateTime;
        }
    }
}
