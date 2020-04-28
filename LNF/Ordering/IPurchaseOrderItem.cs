namespace LNF.Ordering
{
    public interface IPurchaseOrderItem
    {
        int ItemID { get; set; }
        int VendorID { get; set; }
        string Description { get; set; }
        string PartNum { get; set; }
        double UnitPrice { get; set; }
        bool Active { get; set; }
        int? InventoryItemID { get; set; }
    }
}
