using System;

namespace LNF.Billing
{
    public interface IToolBillingReservation
    {
        int ReservationID { get; set; }
        int ResourceID { get; set; }
        int ClientID { get; set; }
        int ActivityID { get; set; }
        int AccountID { get; set; }
        string AccountName { get; set; }
        string ShortCode { get; set; }
        int ChargeTypeID { get; set; }
        bool IsActive { get; set; }
        bool IsStarted { get; set; }
        DateTime BeginDateTime { get; set; }
        DateTime EndDateTime { get; set; }
        DateTime? ActualBeginDateTime { get; set; }
        DateTime? ActualEndDateTime { get; set; }
        DateTime? CancelledDateTime { get; set; }
        DateTime ChargeBeginDateTime { get; set; }
        DateTime ChargeEndDateTime { get; set; }
        DateTime LastModifiedOn { get; set; }
        double ChargeMultiplier { get; set; }
    }
}
