using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Inventory;

namespace LNF.Impl.Mappings.Inventory
{
    internal class InventoryLocationMap : ClassMap<InventoryLocation>
    {
        internal InventoryLocationMap()
        {
            Schema("InventoryControl.dbo");
            Id(x => x.InventoryLocationID);
            Map(x => x.ParentID);
            Map(x => x.LocationName);
            Map(x => x.Active);
            Map(x => x.IsStoreLocation);
            Map(x => x.LocationType);
        }
    }
}
