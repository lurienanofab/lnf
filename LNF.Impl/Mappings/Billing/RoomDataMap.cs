using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Billing;

namespace LNF.Impl.Mappings.Billing
{
    internal class RoomDataMap : ClassMap<RoomData>
    {
        internal RoomDataMap()
        {
            Schema("sselData.dbo");
            Id(x => x.RoomDataID);
            Map(x => x.Period);
            Map(x => x.ClientID);
            Map(x => x.RoomID);
            Map(x => x.ParentID);
            Map(x => x.PassbackRoom);
            Map(x => x.EvtDate);
            Map(x => x.AccountID);
            Map(x => x.Entries);
            Map(x => x.Hours);
            Map(x => x.Days);
            Map(x => x.Months);
            Map(x => x.DataSource);
            Map(x => x.HasToolUsage).Column("hasToolUsage");
        }
    }
}
