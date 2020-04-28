using System;

namespace LNF.Billing
{
    public class ToolBillingReservation : IToolBillingReservation
    {
        public int ReservationID { get; set; }
        public int ResourceID { get; set; }
        public int ClientID { get; set; }
        public int ActivityID { get; set; }
        public int AccountID { get; set; }
        public string AccountName { get; set; }
        public string ShortCode { get; set; }
        public int ChargeTypeID { get; set; }
        public bool IsActive { get; set; }
        public bool IsStarted { get; set; }
        public DateTime BeginDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public DateTime? ActualBeginDateTime { get; set; }
        public DateTime? ActualEndDateTime { get; set; }
        public DateTime? CancelledDateTime { get; set; }
        public DateTime ChargeBeginDateTime { get; set; }
        public DateTime ChargeEndDateTime { get; set; }
        public DateTime LastModifiedOn { get; set; }
        public double ChargeMultiplier { get; set; }
    }
}
