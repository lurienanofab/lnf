namespace LNF.Scheduler
{
    public class ReservationHistoryFilterItem
    {
        public IReservationItem Reservation { get; set; }
        public bool IsCanceledForModification { get; set; }
    }
}
