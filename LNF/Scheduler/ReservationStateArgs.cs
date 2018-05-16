using LNF.Models.Data;
using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public class ReservationStateArgs
    {
        public bool IsInLab { get; set; }
        public bool IsReserver { get; set; }
        public bool IsInvited { get; set; }
        public bool IsAuthorized { get; set; }
        public bool IsRepair { get; set; }
        public bool IsFacilityDownTime { get; set; }
        public bool IsBeforeMinCancelTime { get; set; }
        public int MinReservTime { get; set; }
        public DateTime BeginDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime? ActualBeginDateTime { get; set; }
        public DateTime? ActualEndDateTime { get; set; }
        public ClientAuthLevel UserAuth { get; set; }
        public bool IsToolEngineer { get { return (UserAuth & ClientAuthLevel.ToolEngineer) > 0; } }
        public ReservationManager Manager { get; }

        public ReservationStateArgs(ReservationManager mgr)
        {
            Manager = mgr;
        }

        public static ReservationStateArgs Create(Reservation rsv, Client client, bool inlab, ReservationManager mgr)
        {
            // Determine Ownership
            bool isReserver = rsv.Client.ClientID == client.ClientID;

            // Determine Invition
            bool isInvited = mgr.Session.Query<ReservationInvitee>().Any(x => x.Reservation.ReservationID == rsv.ReservationID && x.Invitee.ClientID == client.ClientID);

            // Determine Authorization
            var resourceClients = mgr.Session.Query<ResourceClient>().Where(x => x.Resource == rsv.Resource).ToList();
            var userAuth = mgr.GetAuthLevel(resourceClients, client);
            bool isAuthorized = (userAuth & (ClientAuthLevel)rsv.Activity.StartEndAuth) > 0;

            bool isBeforeMinCancelTime = (DateTime.Now <= rsv.BeginDateTime.AddMinutes(-1 * rsv.Resource.MinCancelTime));

            bool isRepair = rsv.Activity.Editable;
            bool isFacilityDownTime = rsv.Activity.IsFacilityDownTime;
            int minReservTime = rsv.Resource.MinReservTime;

            return new ReservationStateArgs(mgr)
            {
                IsInLab = inlab,
                IsReserver = isReserver,
                IsInvited = isInvited,
                IsAuthorized = isAuthorized,
                IsRepair = isRepair,
                IsFacilityDownTime = isFacilityDownTime,
                IsBeforeMinCancelTime = isBeforeMinCancelTime,
                MinReservTime = minReservTime,
                BeginDateTime = rsv.BeginDateTime,
                EndDateTime = rsv.EndDateTime,
                ActualBeginDateTime = rsv.ActualBeginDateTime,
                ActualEndDateTime = rsv.ActualEndDateTime,
                UserAuth = userAuth
            };
        }

        public static ReservationStateArgs Create(ReservationItem rsv, ClientItem client, bool inlab, IEnumerable<Models.Scheduler.ReservationInviteeItem> invitees, IEnumerable<ResourceClientInfo> resourceClients, ReservationManager mgr)
        {
            // Determine Ownership
            bool isReserver = rsv.ClientID == client.ClientID;

            // Determine Invition
            bool isInvited = invitees.Any(x => x.ReservationID == rsv.ReservationID && x.ClientID == client.ClientID);

            // Determine Authorization
            var userAuth = mgr.GetAuthLevel(resourceClients, client);
            bool isAuthorized = (userAuth & rsv.StartEndAuth) > 0;

            bool isBeforeMinCancelTime = (DateTime.Now <= rsv.BeginDateTime.AddMinutes(-1 * rsv.MinCancelTime));

            bool isRepair = rsv.Editable;
            bool isFacilityDownTime = rsv.IsFacilityDownTime;
            int minReservTime = rsv.MinReservTime;

            return new ReservationStateArgs(mgr)
            {
                IsInLab = inlab,
                IsReserver = isReserver,
                IsInvited = isInvited,
                IsAuthorized = isAuthorized,
                IsRepair = isRepair,
                IsFacilityDownTime = isFacilityDownTime,
                IsBeforeMinCancelTime = isBeforeMinCancelTime,
                MinReservTime = minReservTime,
                BeginDateTime = rsv.BeginDateTime,
                EndDateTime = rsv.EndDateTime,
                ActualBeginDateTime = rsv.ActualBeginDateTime,
                ActualEndDateTime = rsv.ActualEndDateTime,
                UserAuth = userAuth
            };
        }
    }
}
