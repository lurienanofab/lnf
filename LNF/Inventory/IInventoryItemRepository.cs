using System;
using System.Collections.Generic;

namespace LNF.Inventory
{
    public interface IInventoryItemRepository
    {
        IEnumerable<IInventoryItem> GetItems();
        IEnumerable<IInventoryItem> GetItems(int catId);
        IInventoryItem GetItem(int itemId);
        IItemInventoryType GetItemInventoryType(int itemInventoryTypeId);
        IInventoryType GetInventoryType(int inventoryTypeId);
        IInventoryLocation GetInventoryLocation(int inventoryLocationId);
        IEnumerable<IInventoryLocation> GetInventoryLocations();
        IItemUpdate UpdateInventory(int itemId, double updateQty, double afterQty, string updateAction, int inventoryLocationId, int clientId);
        IInventoryItem UpdateItem(int itemId, string description, int stockQty, int? minStockQty, int? maxStockQty);
        IItemPriceHistory GetItemPriceHistory(int itemId, DateTime cutoff);
        IInventoryItem GetInventoryItem(int itemId);
        void CheckOut(int itemId, int itemInventoryLocationId, int qty);
    }
}
