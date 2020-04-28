using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Inventory;

namespace LNF.Impl.Mappings.Inventory
{
    internal class ItemInventoryLocationMap : ClassMap<ItemInventoryLocation>
    {
        internal ItemInventoryLocationMap()
        {
            Schema("InventoryControl.dbo");
            Id(x => x.ItemInventoryLocationID);
            Map(x => x.InventoryLocationID);
            Map(x => x.ItemID);
        }
    }
}
