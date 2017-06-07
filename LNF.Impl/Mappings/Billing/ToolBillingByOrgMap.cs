using FluentNHibernate.Mapping;
using LNF.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class ToolBillingByOrgMap : ClassMap<ToolBillingByOrg>
    {
        internal ToolBillingByOrgMap()
        {
            ReadOnly();
            Schema("Billing.dbo");
            Table("v_ToolBillingByOrg");
            CompositeId()
                .KeyProperty(x => x.Period)
                .KeyProperty(x => x.ClientID)
                .KeyProperty(x => x.OrgID);
            Map(x => x.LName);
            Map(x => x.FName);
            Map(x => x.OrgName);
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
