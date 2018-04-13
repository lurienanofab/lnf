using FluentNHibernate.Mapping;
using LNF.Repository.Inventory;

namespace LNF.Impl.Mappings.Inventory
{
    internal class ItemMap : ClassMap<Item>
    {
        internal ItemMap()
        {
            Schema("sselMAS.dbo");
            Id(x => x.ItemID);
            Map(x => x.Description);
            Map(x => x.Notes);
            References(x => x.Category, "CatID");
            Map(x => x.ManufacturerPN);
            Map(x => x.Active);
            Map(x => x.StoreDisplay);
            Map(x => x.CrossCharge);
            Map(x => x.StockQuantity);
            Map(x => x.MinStockQuantity);
            Map(x => x.MaxStockQuantity);
            Map(x => x.StockOnOrder);
            Map(x => x.OrderDate);
            Map(x => x.EstimatedArrivalDate);
            Map(x => x.SearchKeyWords);
            Map(x => x.IsChemicalInventory);
            Map(x => x.IsPopular);
            HasMany(x => x.ItemInventoryTypes).NotFound.Ignore();
            HasMany(x => x.ItemInventoryLocations).NotFound.Ignore();
            HasMany(x => x.ItemUpdates).NotFound.Ignore();
        }
    }
}
