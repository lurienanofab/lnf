using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class BillingTypeMap : ClassMap<BillingType>
    {
        internal BillingTypeMap()
        {
            Schema("sselData.dbo");
            Id(x => x.BillingTypeID);
            Map(x => x.BillingTypeName);
            Map(x => x.IsActive);
        }
    }
}
