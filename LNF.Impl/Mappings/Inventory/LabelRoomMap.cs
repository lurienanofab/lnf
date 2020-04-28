using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Inventory;

namespace LNF.Impl.Mappings.Inventory
{
    internal class LabelRoomMap: ClassMap<LabelRoom>
    {
        internal LabelRoomMap()
        {
            Schema("InventoryControl.dbo");
            Id(x => x.LabelRoomID);
            Map(x => x.Slug);
            Map(x => x.RoomName);
        }
    }
}
