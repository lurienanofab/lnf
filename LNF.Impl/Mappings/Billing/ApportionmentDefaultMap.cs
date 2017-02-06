using FluentNHibernate.Mapping;
using LNF.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    public class ApportionmentDefaultMap : ClassMap<ApportionmentDefault>
    {
        public ApportionmentDefaultMap()
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
