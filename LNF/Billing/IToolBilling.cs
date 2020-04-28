using System;

namespace LNF.Billing
{
    public interface IToolBilling
    {
        int AccountID { get; set; }
        DateTime ActDate { get; set; }
        decimal ActDuration { get; set; }
        int BillingTypeID { get; set; }
        decimal BookingFee { get; set; }
        decimal ChargeDuration { get; set; }
        decimal ChargeMultiplier { get; set; }
        int ChargeTypeID { get; set; }
        int ClientID { get; set; }
        decimal ForgivenDuration { get; set; }
        decimal ForgivenFee { get; set; }
        bool IsActive { get; set; }
        bool IsCancelledBeforeAllowedTime { get; set; }
        bool IsFiftyPenalty { get; set; }
        bool IsStarted { get; set; }
        bool IsTemp { get; }
        decimal MaxReservedDuration { get; set; }
        decimal OverTime { get; set; }
        decimal OverTimePenaltyFee { get; set; }
        decimal OverTimePenaltyPercentage { get; set; }
        DateTime Period { get; set; }
        decimal PerUseRate { get; set; }
        string RatePeriod { get; set; }
        decimal ReservationFee2 { get; set; }
        decimal ReservationFeeOld { get; set; }
        int ReservationID { get; set; }
        decimal ReservationRate { get; set; }
        int ResourceID { get; set; }
        decimal ResourceRate { get; set; }
        int RoomID { get; set; }
        decimal SchedDuration { get; set; }
        decimal SubsidyDiscount { get; set; }
        int ToolBillingID { get; set; }
        decimal TransferredDuration { get; set; }
        decimal TransferredFee { get; set; }
        decimal UncancelledPenaltyFee { get; set; }
        decimal UncancelledPenaltyPercentage { get; set; }
        decimal UsageFee { get; set; }
        decimal UsageFee20110401 { get; set; }
        decimal UsageFeeCharged { get; set; }
        decimal UsageFeeFiftyPercent { get; set; }
        decimal UsageFeeOld { get; set; }
        decimal Uses { get; set; }
        decimal GetTotalCharge();
        TimeSpan ActivatedUsed();
        TimeSpan ActivatedUnused();
        TimeSpan UnstartedUnused();
    }
}