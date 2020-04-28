using FluentNHibernate.Mapping;
using LNF.Billing;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal abstract class ToolBillingClassMap<T> : ClassMap<T> where T : IToolBilling
    {
        internal ToolBillingClassMap()
        {
            Schema("sselData.dbo");
            Id(x => x.ToolBillingID);
            Map(x => x.Period);
            Map(x => x.ReservationID);
            Map(x => x.ClientID);
            Map(x => x.AccountID);
            Map(x => x.ChargeTypeID);
            Map(x => x.BillingTypeID);
            Map(x => x.RoomID);
            Map(x => x.ResourceID);
            Map(x => x.ActDate);
            Map(x => x.IsStarted);
            Map(x => x.IsActive);
            Map(x => x.IsFiftyPenalty);
            Map(x => x.ChargeMultiplier);
            Map(x => x.Uses);
            Map(x => x.SchedDuration);
            Map(x => x.ActDuration);
            Map(x => x.ChargeDuration);
            Map(x => x.TransferredDuration);
            Map(x => x.ForgivenDuration).Generated.Always();            // ([ChargeDuration]-[ChargeMultiplier]*[ChargeDuration])
            Map(x => x.MaxReservedDuration);
            Map(x => x.OverTime);
            Map(x => x.RatePeriod);
            Map(x => x.PerUseRate).Precision(19).Scale(4);
            Map(x => x.ResourceRate).Precision(19).Scale(4);
            Map(x => x.ReservationRate).Precision(19).Scale(4);
            Map(x => x.OverTimePenaltyPercentage);
            Map(x => x.UncancelledPenaltyPercentage);
            Map(x => x.UsageFeeCharged);
            Map(x => x.UsageFee20110401).Generated.Always();	        // ((([ChargeMultiplier]*((([ChargeDuration]-[OverTime])-[TransferredDuration])/(60)))*[ResourceRate])*((1)-[IsCancelledBeforeAllowedTime]))
            Map(x => x.UsageFee).Generated.Always();					// ((([ChargeMultiplier]*(([ActDuration]-[OverTime])/(60)))*[ResourceRate])*[IsStarted])
            Map(x => x.UsageFeeOld).Generated.Always();					// ((([ChargeMultiplier]*([ActDuration]/(60)))*[ResourceRate])*[IsStarted])
            Map(x => x.OverTimePenaltyFee).Generated.Always();			// ((([ChargeMultiplier]*([OverTime]/(60)))*[ResourceRate])*[OverTimePenaltyPercentage])
            Map(x => x.UncancelledPenaltyFee).Generated.Always();       // (((([ChargeMultiplier]*([SchedDuration]/(60)))*[ResourceRate])*[UncancelledPenaltyPercentage])*((1)-[IsStarted]))
            Map(x => x.BookingFee).Precision(19).Scale(4);
            Map(x => x.TransferredFee).Generated.Always();				// (([TransferredDuration]/(60))*[ResourceRate])
            Map(x => x.ForgivenFee).Generated.Always();					// ((([ChargeDuration]/(60))*[ResourceRate])*((1)-[ChargeMultiplier]))
            Map(x => x.SubsidyDiscount).Precision(19).Scale(4);
            Map(x => x.IsCancelledBeforeAllowedTime);
            Map(x => x.ReservationFeeOld).Generated.Always();           // ((([ReservationRate]*[Uses])*[ChargeMultiplier])*[IsStarted])
            Map(x => x.ReservationFee2).Precision(19).Scale(4);
            Map(x => x.UsageFeeFiftyPercent).Generated.Always();		// (((([SchedDuration]/(2))*[ResourceRate])/(60))*[IsFiftyPenalty])
        }
    }


    internal class ToolBillingMap : ToolBillingClassMap<ToolBilling>
    {
        internal ToolBillingMap()
        {
            Table("ToolBilling");
        }
    }

    internal class ToolBillingTempMap : ToolBillingClassMap<ToolBillingTemp>
    {
        internal ToolBillingTempMap()
        {
            Table("ToolBillingTemp");
        }
    }
}
