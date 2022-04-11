using System;

namespace LNF.Billing
{
    public class ToolLineCostParameters
    {
        public ToolLineCostParameters() { }

        public ToolLineCostParameters(IToolBilling item, string resourceName)
        {
            Period = item.Period;
            ResourceID = item.ResourceID;
            ResourceName = resourceName;
            RatePeriod = item.RatePeriod;
            RoomID = item.RoomID;
            BillingTypeID = item.BillingTypeID;
            IsStarted = item.IsStarted;
            ResourceRate = item.ResourceRate;
            ReservationRate = item.ReservationRate;
            PerUseRate = item.PerUseRate;
            Uses = item.Uses;
            UsageFeeCharged = item.UsageFeeCharged;
            OverTimePenaltyPercentage = item.OverTimePenaltyPercentage;
            OverTimePenaltyFee = item.OverTimePenaltyFee;
            UncancelledPenaltyPercentage = item.UncancelledPenaltyPercentage;
            UncancelledPenaltyFee = item.UncancelledPenaltyFee;
            BookingFee = item.BookingFee;
            ReservationFee2 = item.ReservationFee2;
            SchedDuration = item.SchedDuration;
            ChargeDuration = item.ChargeDuration;
            OverTime = item.OverTime;
            TransferredDuration = item.TransferredDuration;
            ChargeMultiplier = item.ChargeMultiplier;
            IsCancelledBeforeAllowedTime = item.IsCancelledBeforeAllowedTime;
        }

        public DateTime Period { get; set; }
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public string RatePeriod { get; set; }
        public int RoomID { get; set; }
        public int BillingTypeID { get; set; }
        public bool IsStarted { get; set; }
        public decimal ResourceRate { get; set; }
        public decimal ReservationRate { get; set; }
        public decimal PerUseRate { get; set; }
        public decimal Uses { get; set; }
        public decimal UsageFeeCharged { get; set; }
        public decimal OverTimePenaltyPercentage { get; set; }
        public decimal OverTimePenaltyFee { get; set; }
        public decimal UncancelledPenaltyPercentage { get; set; }
        public decimal UncancelledPenaltyFee { get; set; }
        public decimal BookingFee { get; set; }
        public decimal ReservationFee2 { get; set; }
        public decimal SchedDuration { get; set; }
        public decimal ChargeDuration { get; set; }
        public decimal OverTime { get; set; }
        public decimal TransferredDuration { get; set; }
        public decimal ChargeMultiplier { get; set; }
        public bool IsCancelledBeforeAllowedTime { get; set; }
    }
}
