namespace LNF.Ordering
{
    public class PurchaseOrderItemItem : IPurchaseOrderItem
    {
        public int ItemID { get; set; }
        public int VendorID { get; set; }
        public string Description { get; set; }
        public string PartNum { get; set; }
        public double UnitPrice { get; set; }
        public bool Active { get; set; }
        public int? InventoryItemID { get; set; }
    }
}
