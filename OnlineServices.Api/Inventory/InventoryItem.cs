using LNF.Inventory;
using System;

namespace OnlineServices.Api.Inventory
{
    public class InventoryItem : IInventoryItem
    {
        public int ItemID { get; set; }
        public int ItemInventoryTypeID { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public int CatID { get; set; }
        public string ManufacturerPN { get; set; }
        public bool Active { get; set; }
        public bool StoreDisplay { get; set; }
        public bool CrossCharge { get; set; }
        public int StockQuantity { get; set; }
        public int? MinStockQuantity { get; set; }
        public int? MaxStockQuantity { get; set; }
        public int StockOnOrder { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? EstimatedArrivalDate { get; set; }
        public string SearchKeyWords { get; set; }
        public bool IsChemicalInventory { get; set; }
        public bool IsPopular { get; set; }
        public int CheckOutCategoryID { get; set; }
        public bool IsCheckOutItem { get; set; }
        public int InventoryTypeID { get; set; }
        public string InventoryTypeName { get; set; }
        public bool InventoryTypeDeleted { get; set; }
        public int ChemicalInventoryCatID { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
