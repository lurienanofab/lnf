using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class ActiveLogMap : ClassMap<ActiveLog>
    {
        internal ActiveLogMap()
        {
            Schema("sselData.dbo");
            Id(x => x.LogID);
            Map(x => x.TableName);
            Map(x => x.Record);
            Map(x => x.EnableDate);
            Map(x => x.DisableDate);
        }
    }
}
