namespace LNF.Scheduler
{
    public class ReservationHistoryUpdate
    {
        public int ClientID { get; set; }
        public int ReservationID { get; set; }
        public int? AccountID { get; set; }
        public double? ChargeMultiplier { get; set; }
        public string Notes { get; set; }
        public bool EmailClient { get; set; }
    }
}
