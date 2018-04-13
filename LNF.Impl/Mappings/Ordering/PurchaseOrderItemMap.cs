using FluentNHibernate.Mapping;
using LNF.Repository.Ordering;

namespace LNF.Impl.Mappings.Ordering
{
    internal class PurchaseOrderItemMap : ClassMap<PurchaseOrderItem>
    {
        internal PurchaseOrderItemMap()
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
            HasMany(x => x.Details).KeyColumn("ItemID").NotFound.Ignore();
        }
    }
}
