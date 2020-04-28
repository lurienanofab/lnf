using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Ordering;

namespace LNF.Impl.Mappings.Ordering
{
    internal class PurchaseOrderDetailMap : ClassMap<PurchaseOrderDetail>
    {
        internal PurchaseOrderDetailMap()
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
