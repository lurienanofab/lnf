using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class ClientAccountBillingTypeMap : ClassMap<ClientAccountBillingType>
    {
        internal ClientAccountBillingTypeMap()
        {
            Id(x => x.BillingTypeID);
        }
    }
}
