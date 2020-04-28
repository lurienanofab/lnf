using System;

namespace LNF.Store
{
    public interface IItem
    {
        int ItemID { get; set; }
        string Description { get; set; }
        string Notes { get; set; }
        int CatID { get; set; }
        string ManufacturerPN { get; set; }
        bool Active { get; set; }
        bool StoreDisplay { get; set; }
        bool CrossCharge { get; set; }
        int StockQuantity { get; set; }
        int? MinStockQuantity { get; set; }
        int StockOnOrder { get; set; }
        DateTime? OrderDate { get; set; }
        DateTime? EstimatedArrivalDate { get; set; }
        string SearchKeyWords { get; set; }
        bool IsChemicalInventory { get; set; }
        int ChemicalInventoryCatID { get; set; }
        bool IsPopular { get; set; }
        int? MaxStockQuantity { get; set; }
    }
}
