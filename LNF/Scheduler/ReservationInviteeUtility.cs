using LNF.Scheduler.Data;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Scheduler
{
    public static class ReservationInviteeUtility
    {
        public static IList<AvailableInviteeItem> SelectAvailable(int reservationId, int resourceId, int activityId, int clientId)
        {
            var dt = ReservationInviteeData.SelectAvailableInvitees(reservationId, resourceId, activityId, clientId);
            return dt.AsEnumerable().Select(x => AvailableInviteeItem.Create(x.Field<int>("ClientID"), x.Field<string>("LName"), x.Field<string>("FName"))).ToList();
        }
    }
}
