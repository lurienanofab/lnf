using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Inventory;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Inventory
{
    public class ItemUpdateMap : ClassMap<ItemUpdate>
    {
        public ItemUpdateMap()
        {
            Schema("InventoryControl.dbo");
            Id(x => x.ItemUpdateID);
            References(x => x.Item);
            Map(x => x.BeforeQty);
            Map(x => x.UpdateQty);
            Map(x => x.AfterQty);
            Map(x => x.UpdateDateTime);
            Map(x => x.UpdateAction);
            Map(x => x.ItemInventoryLocationID);
            Map(x => x.ClientID);
        }
    }
}
