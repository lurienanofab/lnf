namespace LNF.Scheduler
{
    public class ReservationHistoryFilterItem
    {
        public IReservation Reservation { get; set; }
        public bool IsCanceledForModification { get; set; }
    }
}
