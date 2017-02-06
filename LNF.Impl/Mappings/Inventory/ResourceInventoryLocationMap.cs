using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Inventory;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Inventory
{
    public class ResourceInventoryLocationMap : ClassMap<ResourceInventoryLocation>
    {
        public ResourceInventoryLocationMap()
        {
            Schema("InventoryControl.dbo");
            Id(x => x.ResourceInventoryLocationID);
            Map(x => x.InventoryLocationID);
            References(x => x.Resource);
        }
    }
}
