using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class ToolBillingByAccountMap : ClassMap<ToolBillingByAccount>
    {
        internal ToolBillingByAccountMap()
        {
            ReadOnly();
            Schema("Billing.dbo");
            Table("v_ToolBillingByAccount");
            CompositeId()
                .KeyProperty(x => x.Period)
                .KeyReference(x => x.Client)
                .KeyReference(x => x.Account);
            References(x => x.Org);
            Map(x => x.UsageFeeCharged);
            Map(x => x.OverTimePenaltyFee);
            Map(x => x.UncancelledPenaltyFee);
            Map(x => x.ReservationFee);
            Map(x => x.BookingFee);
            Map(x => x.ForgivenFee);
            Map(x => x.TransferredFee);
            Map(x => x.TotalCharge);
        }
    }
}
