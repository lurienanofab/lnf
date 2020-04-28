using System;

namespace LNF.Billing
{
    public interface IToolDataRaw
    {
        int AccountID { get; set; }
        double ActDuration { get; set; }
        int ActivityID { get; set; }
        DateTime? ActualBeginDateTime { get; set; }
        DateTime? ActualEndDateTime { get; set; }
        DateTime BeginDateTime { get; set; }
        DateTime? CancelledDateTime { get; set; }
        double ChargeMultiplier { get; set; }
        int ClientID { get; set; }
        DateTime CreatedOn { get; set; }
        DateTime EndDateTime { get; set; }
        bool IsActive { get; set; }
        bool IsStarted { get; set; }
        double MaxReservedDuration { get; set; }
        DateTime? OriginalBeginDateTime { get; set; }
        DateTime? OriginalEndDateTime { get; set; }
        DateTime? OriginalModifiedOn { get; set; }
        double OverTime { get; set; }
        int ReservationID { get; set; }
        int ResourceID { get; set; }
        int RoomID { get; set; }
        double SchedDuration { get; set; }
    }
}