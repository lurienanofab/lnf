using LNF.Models.Scheduler;
using LNF.Repository.Scheduler;

namespace LNF.Scheduler
{
    public class ReservationHistoryFilterItem
    {
        public ReservationItem Reservation { get; set; }
        public bool IsCanceledForModification { get; set; }
    }
}
