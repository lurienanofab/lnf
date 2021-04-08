using LNF.Scheduler;
using System.Collections.Generic;

namespace LNF.Impl.Scheduler
{
    public static class ReservationExtensions
    {
        public static bool IsRunning(this IReservationItem item)
        {
            return item.ActualBeginDateTime != null && item.ActualEndDateTime == null;
        }
    }
}
