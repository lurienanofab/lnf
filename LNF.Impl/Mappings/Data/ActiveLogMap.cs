using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    public class ActiveLogMap : ClassMap<ActiveLog>
    {
        public ActiveLogMap()
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
