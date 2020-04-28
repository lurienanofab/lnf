using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Data;

namespace LNF.Impl.Mappings.Data
{
    internal class RoomMap : ClassMap<Room>
    {
        internal RoomMap()
        {
            Schema("sselData.dbo");
            Id(x => x.RoomID);
            Map(x => x.ParentID);
            Map(x => x.RoomName, "Room");
            Map(x => x.DisplayName);
            Map(x => x.PassbackRoom);
            Map(x => x.Billable);
            Map(x => x.ApportionDailyFee);
            Map(x => x.ApportionEntryFee);
            Map(x => x.Active);
        }
    }
}
