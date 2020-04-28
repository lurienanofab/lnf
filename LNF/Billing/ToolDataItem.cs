using System;

namespace LNF.Billing
{
    public class ToolDataItem : IToolData
    {
        public int ToolDataID { get; set; }
        public DateTime Period { get; set; }
        public int ClientID { get; set; }
        public int ResourceID { get; set; }
        public int? RoomID { get; set; }
        public DateTime ActDate { get; set; }
        public int AccountID { get; set; }
        public int OrgID { get; set; }
        public double Uses { get; set; }
        public double SchedDuration { get; set; }
        public double ActDuration { get; set; }
        public double OverTime { get; set; }
        public double? Days { get; set; }
        public double? Months { get; set; }
        public bool IsStarted { get; set; }
        public double ChargeMultiplier { get; set; }
        public int? ReservationID { get; set; }
        public double ChargeDuration { get; set; }
        public double TransferredDuration { get; set; }
        public double MaxReservedDuration { get; set; }
        public DateTime? ChargeBeginDateTime { get; set; }
        public DateTime? ChargeEndDateTime { get; set; }
        public bool IsActive { get; set; }
        public bool? IsCancelledBeforeAllowedTime { get; set; }
    }
}
