using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Store;

namespace LNF.Impl.Mappings.Store
{
    internal class ItemMap : ClassMap<Item>
    {
        internal ItemMap()
        {
            Schema("sselMAS.dbo");
            Id(x => x.ItemID);
            Map(x => x.Description);
            Map(x => x.Notes);
            Map(x => x.CatID);
            Map(x => x.ManufacturerPN);
            Map(x => x.Active);
            Map(x => x.StoreDisplay);
            Map(x => x.CrossCharge);
            Map(x => x.StockQuantity);
            Map(x => x.MinStockQuantity);
            Map(x => x.StockOnOrder);
            Map(x => x.OrderDate);
            Map(x => x.EstimatedArrivalDate);
            Map(x => x.SearchKeyWords);
            Map(x => x.IsChemicalInventory);
            Map(x => x.ChemicalInventoryCatID);
            Map(x => x.IsPopular);
            Map(x => x.MaxStockQuantity);
        }
    }
}
