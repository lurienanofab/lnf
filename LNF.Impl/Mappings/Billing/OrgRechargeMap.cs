using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class OrgRechargeMap : ClassMap<OrgRecharge>
    {
        internal OrgRechargeMap()
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
