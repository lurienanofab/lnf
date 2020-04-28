using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class GlobalCostMap : ClassMap<GlobalCost>
    {
        internal GlobalCostMap()
        {
            Schema("sselData.dbo");
            Id(x => x.GlobalID);
            Map(x => x.BusinessDay);
            References(x => x.LabAccount);
            References(x => x.LabCreditAccount);
            References(x => x.SubsidyCreditAccount);
            References(x => x.Admin);
            Map(x => x.AccessToOld);
            Map(x => x.EffDate);
        }
    }
}
