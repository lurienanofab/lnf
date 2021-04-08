using LNF.Data;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public class ReservationClient
    {
        public int ClientID { get; set; }
        public int ReservationID { get; set; }
        public int ResourceID { get; set; }
        public bool IsReserver { get; set; }
        public bool IsInvited { get; set; }
        public bool InLab { get; set; }
        public ClientAuthLevel UserAuth { get; set; }

        public bool IsAuthorized(IReservationItem rsv) => (UserAuth & rsv.StartEndAuth) > 0;

        public static ReservationClient Create(IReservationWithInviteesItem rsv, IClient client, IEnumerable<IResourceClient> resourceClients, bool inlab)
        {
            return Create(rsv, client, resourceClients, rsv.Invitees, inlab);
        }

        public static ReservationClient Create(IReservationItem rsv, IClient client, IEnumerable<IResourceClient> resourceClients, IEnumerable<IReservationInviteeItem> invitees, bool inlab)
        {
            var userAuth = Reservations.GetAuthLevel(resourceClients, client);
            var isReserver = rsv.ClientID == client.ClientID;
            var isInvited = invitees.Any(x => x.InviteeID == client.ClientID);

            var result = new ReservationClient
            {
                ClientID = client.ClientID,
                ReservationID = rsv.ReservationID,
                ResourceID = rsv.ResourceID,
                IsReserver = isReserver,
                IsInvited = isInvited,
                InLab = inlab,
                UserAuth = userAuth
            };

            return result;
        }
    }
}
