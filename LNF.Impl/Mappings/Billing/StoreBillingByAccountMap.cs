using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class StoreBillingByAccountMap : ClassMap<StoreBillingByAccount>
    {
        internal StoreBillingByAccountMap()
        {
            ReadOnly();
            Schema("Billing.dbo");
            Table("v_StoreBillingByAccount");
            CompositeId()
                .KeyProperty(x => x.Period)
                .KeyReference(x => x.Client)
                .KeyReference(x => x.Account);
            References(x => x.Org);
            Map(x => x.TotalCharge);
        }
    }
}
