using FluentNHibernate.Mapping;
using LNF.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class RoomDataMap : ClassMap<RoomData>
    {
        internal RoomDataMap()
        {
            Schema("sselData.dbo");
            Id(x => x.RoomDataID);
            Map(x => x.Period);
            References(x => x.Client);
            References(x => x.Room);
            Map(x => x.EvtDate);
            References(x => x.Account);
            Map(x => x.Entries);
            Map(x => x.Hours);
            Map(x => x.Days);
            Map(x => x.Months);
            Map(x => x.DataSource);
            Map(x => x.HasToolUsage).Column("hasToolUsage");
        }
    }
}
