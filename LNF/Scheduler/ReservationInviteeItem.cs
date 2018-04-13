using LNF.Models.Data;
using LNF.Repository.Scheduler;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Scheduler
{
    public class ReservationInviteeItem
    {
        public int ReservationID { get; set; }
        public int InviteeID { get; set; }
        public string DisplayName { get; set; }

        public static IEnumerable<ReservationInviteeItem> Create(IQueryable<ReservationInvitee> query)
        {
            return query.Select(x => new ReservationInviteeItem()
            {
                ReservationID = x.Reservation.ReservationID,
                InviteeID = x.Invitee.ClientID,
                DisplayName = ClientItem.GetDisplayName(x.Invitee.LName, x.Invitee.FName)
            });
        }
    }
}
