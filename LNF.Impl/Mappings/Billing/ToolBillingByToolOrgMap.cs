using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class ToolBillingByToolOrgMap : ClassMap<ToolBillingByToolOrg>
    {
        internal ToolBillingByToolOrgMap()
        {
            ReadOnly();
            Schema("Billing.dbo");
            Table("v_ToolBillingByToolOrg");
            CompositeId()
                .KeyProperty(x => x.Period)
                .KeyProperty(x => x.ClientID)
                .KeyProperty(x => x.OrgID)
                .KeyProperty(x => x.ResourceID);
            Map(x => x.LName);
            Map(x => x.FName);
            Map(x => x.OrgName);
            Map(x => x.ResourceName);
            Map(x => x.ChargeTypeID);
            Map(x => x.ChargeTypeName);
            Map(x => x.BillingTypeID);
            Map(x => x.BillingTypeName);
            Map(x => x.UsageFeeCharged);
            Map(x => x.OverTimePenaltyFee);
            Map(x => x.UncancelledPenaltyFee);
            Map(x => x.ReservationFee);
            Map(x => x.BookingFee);
            Map(x => x.ForgivenFee);
            Map(x => x.TransferredFee);
            Map(x => x.SubsidyDiscount);
            Map(x => x.TotalCharge);
        }
    }
}
