using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class StoreBillingByItemOrgMap : ClassMap<StoreBillingByItemOrg>
    {
        internal StoreBillingByItemOrgMap()
        {
            ReadOnly();
            Schema("Billing.dbo");
            Table("v_StoreBillingByItemOrg");
            CompositeId()
                .KeyProperty(x => x.Period)
                .KeyReference(x => x.Client)
                .KeyReference(x => x.Org)
                .KeyReference(x => x.Item);
            References(x => x.ChargeType);
            Map(x => x.TotalCharge);
        }
    }
}
