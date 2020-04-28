using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Inventory;

namespace LNF.Impl.Mappings.Inventory
{
    internal class ItemUpdateMap : ClassMap<ItemUpdate>
    {
        internal ItemUpdateMap()
        {
            Schema("InventoryControl.dbo");
            Id(x => x.ItemUpdateID);
            Map(x => x.ItemID);
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
