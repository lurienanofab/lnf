using System;

namespace LNF.Models.Scheduler
{
    public class ReservationHistoryItem
    {
        public const string INSERT_FOR_MODIFICATION = "InsertForModification";

        public int ReservationHistoryID { get; set; }
        public int ReservationID { get; set; }
        public string UserAction { get; set; }
        public int? LinkedReservationID { get; set; }
        public string ActionSource { get; set; }
        public int? ModifiedByClientID { get; set; }
        public DateTime ModifiedDateTime { get; set; }
        public int AccountID { get; set; }
        public DateTime BeginDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public double ChargeMultiplier { get; set; }
    }
}
