using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class ApportionmentDefaultMap : ClassMap<ApportionmentDefault>
    {
        internal ApportionmentDefaultMap()
        {
            Schema("sselData.dbo");
            Id(x => x.ApportionmentDefaultID, "AppID");
            References(x => x.Client);
            References(x => x.Room);
            References(x => x.Account);
            Map(x => x.Percentage);
        }
    }
}
