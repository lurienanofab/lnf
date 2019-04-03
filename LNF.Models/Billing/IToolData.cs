using System;

namespace LNF.Models.Billing
{
    public interface IToolData
    {
        int AccountID { get; set; }
        int OrgID { get; set; }
        DateTime ActDate { get; set; }
        double ActDuration { get; set; }
        DateTime? ChargeBeginDateTime { get; set; }
        double ChargeDuration { get; set; }
        DateTime? ChargeEndDateTime { get; set; }
        double ChargeMultiplier { get; set; }
        int ClientID { get; set; }
        double? Days { get; set; }
        bool IsActive { get; set; }
        bool? IsCancelledBeforeAllowedTime { get; set; }
        bool IsStarted { get; set; }
        double MaxReservedDuration { get; set; }
        double? Months { get; set; }
        double OverTime { get; set; }
        DateTime Period { get; set; }
        int? ReservationID { get; set; }
        int ResourceID { get; set; }
        int? RoomID { get; set; }
        double SchedDuration { get; set; }
        int ToolDataID { get; set; }
        double TransferredDuration { get; set; }
        double Uses { get; set; }
    }
}