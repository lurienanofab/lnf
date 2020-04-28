using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class FundingSourceMap : ClassMap<FundingSource>
    {
        internal FundingSourceMap()
        {
            Schema("sselData.dbo");
            Id(x => x.FundingSourceID);
            Map(x => x.FundingSourceName, "FundingSource");
        }
    }
}
