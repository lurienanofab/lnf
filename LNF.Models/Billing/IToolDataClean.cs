using System;

namespace LNF.Models.Billing
{
    public interface IToolDataClean
    {
        int ToolDataID { get; set; }
        int ClientID { get; set; }
        int ResourceID { get; set; }
        int RoomID { get; set; }
        DateTime BeginDateTime { get; set; }
        DateTime EndDateTime { get; set; }
        DateTime ActualBeginDateTime { get; set; }
        DateTime ActualEndDateTime { get; set; }
        int AccountID { get; set; }
        int ActivityID { get; set; }
        double SchedDuration { get; set; }
        double ActDuration { get; set; }
        double OverTime { get; set; }
        bool IsStarted { get; set; }
        double ChargeMultiplier { get; set; }
        int ReservationID { get; set; }
        double MaxReservedDuration { get; set; }
        bool IsActive { get; set; }
        DateTime? CancelledDateTime { get; set; }
        DateTime? OriginalBeginDateTime { get; set; }
        DateTime? OriginalEndDateTime { get; set; }
        DateTime? OriginalModifiedOn { get; set; }
        DateTime CreatedOn { get; set; }
        DateTime GetChargeBeginDateTime();
        DateTime GetChargeEndDateTime();
    }
}
