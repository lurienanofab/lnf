using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Ordering;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Ordering
{
    public class PurchaseOrderDetailMap : ClassMap<PurchaseOrderDetail>
    {
        public PurchaseOrderDetailMap()
        {
            Schema("IOF.dbo");
            Id(x => x.PODID);
            References(x => x.PurchaseOrder, "POID");
            References(x => x.Item, "ItemID");
            References(x => x.Category, "CatID");
            Map(x => x.Quantity);
            Map(x => x.UnitPrice);
            Map(x => x.Unit);
            Map(x => x.ToInventoryDate);
            Map(x => x.IsInventoryControlled);
        }
    }
}
