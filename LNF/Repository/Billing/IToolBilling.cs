using LNF.Repository;
using System;

namespace LNF.Repository.Billing
{
    public interface IToolBilling : IDataItem
    {
        int ToolBillingID { get; set; }
        DateTime Period { get; set; }
        int ReservationID { get; set; }
        int ClientID { get; set; }
        int AccountID { get; set; }
        int ChargeTypeID { get; set; }
        int BillingTypeID { get; set; }
        int RoomID { get; set; }
        int ResourceID { get; set; }
        DateTime ActDate { get; set; }
        bool IsStarted { get; set; }
        bool IsActive { get; set; }
        bool IsFiftyPenalty { get; set; }
        decimal ChargeMultiplier { get; set; }
        decimal Uses { get; set; }
        decimal SchedDuration { get; set; }
        decimal ActDuration { get; set; }
        decimal ChargeDuration { get; set; }
        decimal TransferredDuration { get; set; }
        decimal ForgivenDuration { get; set; }
        decimal MaxReservedDuration { get; set; }
        /// <summary>
        /// Number of overtime minutes.
        /// </summary>
        decimal OverTime { get; set; }
        string RatePeriod { get; set; }
        decimal PerUseRate { get; set; }
        decimal ResourceRate { get; set; }
        decimal ReservationRate { get; set; }
        decimal OverTimePenaltyPercentage { get; set; }
        decimal UncancelledPenaltyPercentage { get; set; }
        decimal UsageFeeCharged { get; set; }
        decimal UsageFee20110401 { get; set; }
        decimal UsageFee { get; set; }
        decimal UsageFeeOld { get; set; }
        decimal OverTimePenaltyFee { get; set; }
        decimal UncancelledPenaltyFee { get; set; }
        decimal BookingFee { get; set; }
        decimal TransferredFee { get; set; }
        decimal ForgivenFee { get; set; }
        decimal SubsidyDiscount { get; set; }
        bool IsCancelledBeforeAllowedTime { get; set; }
        decimal ReservationFeeOld { get; set; }
        decimal ReservationFee2 { get; set; }
        decimal UsageFeeFiftyPercent { get; set; }
        bool IsTemp { get; }
        decimal GetTotalCharge();
    }
}
