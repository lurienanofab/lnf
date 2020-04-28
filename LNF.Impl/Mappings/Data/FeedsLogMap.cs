using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class FeedsLogMap : ClassMap<FeedsLog>
    {
        internal FeedsLogMap()
        {
            Schema("sselData.dbo");
            Id(x => x.FeedsLogID);
            Map(x => x.EntryDateTime);
            Map(x => x.RequestIP);
            Map(x => x.RequestURL);
            Map(x => x.RequestUserAgent);
        }
    }
}
