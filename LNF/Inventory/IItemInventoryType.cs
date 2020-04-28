namespace LNF.Inventory
{
    public interface IItemInventoryType
    {
        int ItemInventoryTypeID { get; set; }
        int ItemID { get; set; }
        int InventoryTypeID { get; set; }
        int CheckOutCategoryID { get; set; }
        bool IsPopular { get; set; }
        bool IsCheckOutItem { get; set; }
    }
}
