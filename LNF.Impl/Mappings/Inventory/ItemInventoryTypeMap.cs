using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Inventory;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Inventory
{
    public class ItemInventoryTypeMap : ClassMap<ItemInventoryType>
    {
        public ItemInventoryTypeMap()
        {
            Schema("InventoryControl.dbo");
            Id(x => x.ItemInventoryTypeID);
            References(x => x.Item);
            References(x => x.InventoryType);
            References(x => x.CheckOutCategory);
            Map(x => x.IsPopular);
            Map(x => x.IsCheckOutItem);
        }
    }
}
