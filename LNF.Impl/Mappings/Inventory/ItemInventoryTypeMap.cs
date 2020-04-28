using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Inventory;

namespace LNF.Impl.Mappings.Inventory
{
    internal class ItemInventoryTypeMap : ClassMap<ItemInventoryType>
    {
        internal ItemInventoryTypeMap()
        {
            Schema("InventoryControl.dbo");
            Id(x => x.ItemInventoryTypeID);
            Map(x => x.ItemID);
            Map(x => x.InventoryTypeID);
            Map(x => x.CheckOutCategoryID);
            Map(x => x.IsPopular);
            Map(x => x.IsCheckOutItem);
        }
    }
}
