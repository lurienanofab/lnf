using FluentNHibernate.Mapping;
using LNF.Repository.Inventory;

namespace LNF.Impl.Mappings.Inventory
{
    public class InventoryLocationMap : ClassMap<InventoryLocation>
    {
        public InventoryLocationMap()
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
