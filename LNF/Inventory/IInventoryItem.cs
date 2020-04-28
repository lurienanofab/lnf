using LNF.Store;

namespace LNF.Inventory
{
    public interface IInventoryItem : IItem
    {
        int ItemInventoryTypeID { get; set; }
        int CheckOutCategoryID { get; set; }
        bool IsCheckOutItem { get; set; }
        int InventoryTypeID { get; set; }
        string InventoryTypeName { get; set; }
        bool InventoryTypeDeleted { get; set; }
    }
}