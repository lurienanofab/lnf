using System;

namespace LNF.Repository.Billing
{
    public abstract class ToolBillingBase : IDataItem, IToolBilling
    {
        public virtual int ToolBillingID { get; set; }
        public virtual DateTime Period { get; set; }
        public virtual int ReservationID { get; set; }
        public virtual int ClientID { get; set; }
        public virtual int AccountID { get; set; }
        public virtual int ChargeTypeID { get; set; }
        public virtual int BillingTypeID { get; set; }
        public virtual int RoomID { get; set; }
        public virtual int ResourceID { get; set; }
        public virtual DateTime ActDate { get; set; }
        public virtual bool IsStarted { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual bool IsFiftyPenalty { get; set; }
        public virtual decimal ChargeMultiplier { get; set; }
        public virtual decimal Uses { get; set; }
        public virtual decimal SchedDuration { get; set; }
        public virtual decimal ActDuration { get; set; }
        public virtual decimal ChargeDuration { get; set; }
        public virtual decimal TransferredDuration { get; set; }
        public virtual decimal ForgivenDuration { get; set; }			// ([ChargeDuration]-[ChargeMultiplier]*[ChargeDuration])
        public virtual decimal MaxReservedDuration { get; set; }
        public virtual decimal OverTime { get; set; }
        public virtual string RatePeriod { get; set; }
        public virtual decimal PerUseRate { get; set; }
        public virtual decimal ResourceRate { get; set; }
        public virtual decimal ReservationRate { get; set; }
        public virtual decimal OverTimePenaltyPercentage { get; set; }
        public virtual decimal UncancelledPenaltyPercentage { get; set; }
        public virtual decimal UsageFeeCharged { get; set; }
        public virtual decimal UsageFee20110401 { get; set; }	        // ((([ChargeMultiplier]*((([ChargeDuration]-[OverTime])-[TransferredDuration])/(60)))*[ResourceRate])*((1)-[IsCancelledBeforeAllowedTime]))
        public virtual decimal UsageFee { get; set; }					// ((([ChargeMultiplier]*(([ActDuration]-[OverTime])/(60)))*[ResourceRate])*[IsStarted])
        public virtual decimal UsageFeeOld { get; set; }			    // ((([ChargeMultiplier]*([ActDuration]/(60)))*[ResourceRate])*[IsStarted])
        public virtual decimal OverTimePenaltyFee { get; set; }			// ((([ChargeMultiplier]*([OverTime]/(60)))*[ResourceRate])*[OverTimePenaltyPercentage])
        public virtual decimal UncancelledPenaltyFee { get; set; }      // (((([ChargeMultiplier]*([SchedDuration]/(60)))*[ResourceRate])*[UncancelledPenaltyPercentage])*((1)-[IsStarted]))
        public virtual decimal BookingFee { get; set; }
        public virtual decimal TransferredFee { get; set; }				// (([TransferredDuration]/(60))*[ResourceRate])
        public virtual decimal ForgivenFee { get; set; }				// ((([ChargeDuration]/(60))*[ResourceRate])*((1)-[ChargeMultiplier]))
        public virtual decimal SubsidyDiscount { get; set; }
        public virtual bool IsCancelledBeforeAllowedTime { get; set; }
        public virtual decimal ReservationFeeOld { get; set; }		    // ((([ReservationRate]*[Uses])*[ChargeMultiplier])*[IsStarted])
        public virtual decimal ReservationFee2 { get; set; }
        public virtual decimal UsageFeeFiftyPercent { get; set; }		// (((([SchedDuration]/(2))*[ResourceRate])/(60))*[IsFiftyPenalty])
        public abstract bool IsTemp { get; }

        /// <summary>
        /// The total charge used to calculate subsidy.
        /// </summary>
        public virtual decimal GetTotalCharge()
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
    }

    public class ToolBilling : ToolBillingBase
    {
        public override bool IsTemp
        {
            get { return false; }
        }
    }

    public class ToolBillingTemp : ToolBillingBase
    {
        public override bool IsTemp
        {
            get { return true; }
        }
    }
}
