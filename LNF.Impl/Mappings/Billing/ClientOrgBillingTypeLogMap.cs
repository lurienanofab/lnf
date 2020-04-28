using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class ClientOrgBillingTypeLogMap : ClassMap<ClientOrgBillingTypeLog>
    {
        internal ClientOrgBillingTypeLogMap()
        {
            Schema("sselData.dbo");
            Table("ClientOrgBillingTypeTS");
            Id(x => x.ClientOrgBillingTypeLogID, "ClientOrgBillingTypeID");
            Map(x => x.ClientOrgID);
            Map(x => x.BillingTypeID);
            Map(x => x.EffDate);
            Map(x => x.DisableDate);
        }
    }
}
