using System;

namespace LNF.Scheduler
{
    public static class ReservationGroups
    {
        public static IReservationGroup Update(int groupId, DateTime beginDateTime, DateTime endDateTime)
        {
            return ServiceProvider.Current.Scheduler.Reservation.UpdateReservationGroup(groupId, beginDateTime, endDateTime);
        }
    }
}
