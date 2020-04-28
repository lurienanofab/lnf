using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Inventory;

namespace LNF.Impl.Mappings.Inventory
{
    internal class InventoryTypeMap : ClassMap<InventoryType>
    {
        internal InventoryTypeMap()
        {
            Schema("InventoryControl.dbo");
            Id(x => x.InventoryTypeID);
            Map(x => x.InventoryTypeName);
            Map(x => x.Deleted);
            HasMany(x => x.ItemInventoryTypes).KeyColumn("InventoryTypeID");
            HasMany(x => x.CheckOutCategories).KeyColumn("InventoryTypeID");
        }
    }
}
