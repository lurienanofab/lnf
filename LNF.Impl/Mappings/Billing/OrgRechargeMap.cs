using FluentNHibernate.Mapping;
using LNF.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    public class OrgRechargeMap : ClassMap<OrgRecharge>
    {
        public OrgRechargeMap()
        {
            Schema("Billing.dbo");
            Id(x => x.OrgRechargeID);
            References(x => x.Org);
            References(x => x.Account);
            Map(x => x.CreatedDate);
            Map(x => x.EnableDate);
            Map(x => x.DisableDate);
        }
    }
}
