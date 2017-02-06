using FluentNHibernate.Mapping;
using LNF.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    public class BillingTypeMap : ClassMap<BillingType>
    {
        public BillingTypeMap()
        {
            Schema("sselData.dbo");
            Id(x => x.BillingTypeID);
            Map(x => x.BillingTypeName);
            Map(x => x.IsActive);
        }
    }
}
