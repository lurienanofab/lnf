using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Inventory;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Inventory
{
    public class ItemInventoryLocationMap : ClassMap<ItemInventoryLocation>
    {
        public ItemInventoryLocationMap()
        {
            Schema("InventoryControl.dbo");
            Id(x => x.ItemInventoryLocationID);
            References(x => x.InventoryLocation);
            References(x => x.Item);
        }
    }
}
