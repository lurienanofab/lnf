using LNF.Repository.Scheduler;

namespace LNF.Scheduler
{
    public class ReservationHistoryFilterItem
    {
        public Reservation Reservation { get; set; }
        public bool IsCanceledForModification { get; set; }
    }
}
