using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Ordering;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Ordering
{
    public class PurchaseOrderItemMap : ClassMap<PurchaseOrderItem>
    {
        public PurchaseOrderItemMap()
        {
            Schema("IOF.dbo");
            Table("Item");
            Id(x => x.ItemID);
            References(x => x.Vendor);
            Map(x => x.Description);
            Map(x => x.PartNum);
            Map(x => x.UnitPrice);
            Map(x => x.Active);
            Map(x => x.InventoryItemID, "StoreItemID");
        }
    }
}
