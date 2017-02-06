namespace LNF.Models.Inventory
{
    public class ItemModel
    {
        public int ItemID { get; set; }
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
    }
}
