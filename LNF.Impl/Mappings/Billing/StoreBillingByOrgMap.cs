using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class StoreBillingByOrgMap : ClassMap<StoreBillingByOrg>
    {
        internal StoreBillingByOrgMap()
        {
            ReadOnly();
            Schema("Billing.dbo");
            Table("v_StoreBillingByOrg");
            CompositeId()
                .KeyProperty(x => x.Period)
                .KeyReference(x => x.Client)
                .KeyReference(x => x.Org);
            Map(x => x.TotalCharge);
        }
    }
}
