using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class RoomBillingByAccountMap : ClassMap<RoomBillingByAccount>
    {
        internal RoomBillingByAccountMap()
        {
            ReadOnly();
            Schema("Billing.dbo");
            Table("v_RoomBillingByAccount");
            CompositeId()
                .KeyProperty(x => x.Period)
                .KeyReference(x => x.Client)
                .KeyReference(x => x.Account);
            Map(x => x.RoomCharge);
            Map(x => x.EntryCharge);
            Map(x => x.SubsidyDiscount);
            Map(x => x.TotalCharge);
        }
    }
}
