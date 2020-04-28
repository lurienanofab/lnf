using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Inventory;

namespace LNF.Impl.Mappings.Inventory
{
    internal class ResourceInventoryLocationMap : ClassMap<ResourceInventoryLocation>
    {
        internal ResourceInventoryLocationMap()
        {
            Schema("InventoryControl.dbo");
            Id(x => x.ResourceInventoryLocationID);
            Map(x => x.InventoryLocationID);
            References(x => x.Resource);
        }
    }
}
