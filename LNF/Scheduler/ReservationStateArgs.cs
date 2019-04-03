using LNF.Models.Data;
using LNF.Models.Scheduler;
using LNF.PhysicalAccess;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public struct ReservationStateArgs
    {
        public bool IsInLab { get; }
        public bool IsReserver { get; }
        public bool IsInvited { get; }
        public bool IsAuthorized { get; }
        public bool IsRepair { get; }
        public bool IsFacilityDownTime { get; }
        public bool IsBeforeMinCancelTime { get; }
        public int MinReservTime { get; }
        public DateTime BeginDateTime { get; }
        public DateTime EndDateTime { get; }
        public DateTime? ActualBeginDateTime { get; }
        public DateTime? ActualEndDateTime { get; }
        public ClientAuthLevel UserAuth { get; }
        public bool IsToolEngineer => (UserAuth & ClientAuthLevel.ToolEngineer) > 0;

        public ReservationStateArgs(bool inlab, bool isReserver, bool isInvited, bool isAuthorized, bool isRepair, bool isFacilityDownTime, bool isBeforeMinCancelTime, int minReservTime, DateTime beginDateTime, DateTime endDateTime, DateTime? actualBeginDateTime, DateTime? actualEndDateTime, ClientAuthLevel userAuth)
        {
            IsInLab = inlab;
            IsReserver = isReserver;
            IsInvited = isInvited;
            IsAuthorized = isAuthorized;
            IsRepair = isRepair;
            IsFacilityDownTime = isFacilityDownTime;
            IsBeforeMinCancelTime = isBeforeMinCancelTime;
            MinReservTime = minReservTime;
            BeginDateTime = beginDateTime;
            EndDateTime = endDateTime;
            ActualBeginDateTime = actualBeginDateTime;
            ActualEndDateTime = actualEndDateTime;
            UserAuth = userAuth;
        }

        public static ReservationStateArgs Create(IReservation rsv, ReservationClient client)
        {
            var isAuthorized = (client.UserAuth & rsv.StartEndAuth) > 0;

            var isBeforeMinCancelTime = (DateTime.Now <= rsv.BeginDateTime.AddMinutes(-1 * rsv.MinCancelTime));

            var args = new ReservationStateArgs(client.InLab, client.IsReserver, client.IsInvited, isAuthorized, rsv.IsRepair, rsv.IsFacilityDownTime, isBeforeMinCancelTime, rsv.MinReservTime, rsv.BeginDateTime, rsv.EndDateTime, rsv.ActualBeginDateTime, rsv.ActualEndDateTime, client.UserAuth);

            return args;
        }
    }
}
