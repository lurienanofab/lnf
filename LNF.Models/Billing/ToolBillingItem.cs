using System;

namespace LNF.Models.Billing
{
    public class ToolBillingItem : IToolBilling
    {
        public int ToolBillingID { get; set; }
        public DateTime Period { get; set; }
        public int ReservationID { get; set; }
        public int ClientID { get; set; }
        public int AccountID { get; set; }
        public int ChargeTypeID { get; set; }
        public int BillingTypeID { get; set; }
        public int RoomID { get; set; }
        public int ResourceID { get; set; }
        public DateTime ActDate { get; set; }
        public bool IsStarted { get; set; }
        public bool IsActive { get; set; }
        public bool IsFiftyPenalty { get; set; }
        public decimal ChargeMultiplier { get; set; }
        public decimal Uses { get; set; }
        public decimal SchedDuration { get; set; }
        public decimal ActDuration { get; set; }
        public decimal ChargeDuration { get; set; }
        public decimal TransferredDuration { get; set; }
        public decimal ForgivenDuration { get; set; }
        public decimal MaxReservedDuration { get; set; }
        public decimal OverTime { get; set; }
        public string RatePeriod { get; set; }
        public decimal PerUseRate { get; set; }
        public decimal ResourceRate { get; set; }
        public decimal ReservationRate { get; set; }
        public decimal OverTimePenaltyPercentage { get; set; }
        public decimal UncancelledPenaltyPercentage { get; set; }
        public decimal UsageFeeCharged { get; set; }
        public decimal UsageFee20110401 { get; set; }
        public decimal UsageFee { get; set; }
        public decimal UsageFeeOld { get; set; }
        public decimal OverTimePenaltyFee { get; set; }
        public decimal UncancelledPenaltyFee { get; set; }
        public decimal BookingFee { get; set; }
        public decimal TransferredFee { get; set; }
        public decimal ForgivenFee { get; set; }
        public decimal SubsidyDiscount { get; set; }
        public bool IsCancelledBeforeAllowedTime { get; set; }
        public decimal ReservationFeeOld { get; set; }
        public decimal ReservationFee2 { get; set; }
        public decimal UsageFeeFiftyPercent { get; set; }
        public bool IsTemp { get; set; }

        public decimal GetTotalCharge()
        {
            // We can include everything now because the value in each column is correct.
            // For example: after 2015-10-01 UncancelledPenaltyFee and ReservationFee2 will be zero
            // and before 2011-04-01 BookingFee will be zero. And UsageFeeCharged is whatever value
            // was calculated based on the rules in place at the time. By making the data correct
            // based on the current rules we don't have to check the period and apply different
            // logic in many different places. In other words this formula will work for any
            // period - much easier to manage.
            return UsageFeeCharged + OverTimePenaltyFee + BookingFee + UncancelledPenaltyFee + ReservationFee2;
        }

        public TimeSpan ActivatedUsed()
        {
            // if a reservation is started IsCancelledBeforeAllowedTime must be false, right?
            decimal activatedUsed = (IsStarted && !IsCancelledBeforeAllowedTime) ? (ActDuration - OverTime) : 0;
            return TimeSpan.FromMinutes((double)activatedUsed);
        }

        public TimeSpan ActivatedUnused()
        {
            decimal activatedUnused = (IsStarted && !IsCancelledBeforeAllowedTime) ? Math.Max(ChargeDuration - ActDuration, 0) : 0;
            return TimeSpan.FromMinutes((double)activatedUnused);
        }

        public TimeSpan UnstartedUnused()
        {
            decimal unstartedUnused = (!IsStarted && !IsCancelledBeforeAllowedTime) ? ChargeDuration : 0;
            return TimeSpan.FromMinutes((double)unstartedUnused);
        }
    }
}
