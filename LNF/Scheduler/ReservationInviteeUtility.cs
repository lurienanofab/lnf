using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;
using LNF.Scheduler.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Scheduler
{
    public static class ReservationInviteeUtility
    {
        public static ReservationInvitee Select(int reservationId, int inviteeId)
        {
            return Select(DA.Current.Single<Reservation>(reservationId), DA.Current.Single<Client>(inviteeId));
        }

        public static ReservationInvitee Select(Reservation rsv, Client invitee)
        {
            if (rsv == null || invitee == null)
                return null;

            ReservationInvitee key = new ReservationInvitee()
            {
                Reservation = rsv,
                Invitee = invitee
            };

            return DA.Current.Single<ReservationInvitee>(key);
        }

        public static bool IsInvited(int reservationId, int inviteeId)
        {
            return Select(reservationId, inviteeId) != null;
        }

        public static IList<AvailableInviteeItem> SelectAvailable(int reservationId, int resourceId, int activityId, int clientId)
        {
            var dt = ReservationInviteeData.SelectAvailableInvitees(reservationId, resourceId, activityId, clientId);
            return dt.AsEnumerable().Select(x => AvailableInviteeItem.Create(x.Field<int>("ClientID"), x.Field<string>("LName"), x.Field<string>("FName"))).ToList();
        }
    }
}
