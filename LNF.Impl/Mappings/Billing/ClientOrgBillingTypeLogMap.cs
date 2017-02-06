using FluentNHibernate.Mapping;
using LNF.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    public class ClientOrgBillingTypeLogMap : ClassMap<ClientOrgBillingTypeLog>
    {
        public ClientOrgBillingTypeLogMap()
        {
            Schema("sselData.dbo");
            Table("ClientOrgBillingTypeTS");
            Id(x => x.ClientOrgBillingTypeLogID, "ClientOrgBillingTypeID");
            References(x => x.ClientOrg);
            References(x => x.BillingType);
            Map(x => x.EffDate);
            Map(x => x.DisableDate);
        }
    }
}
