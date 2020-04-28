using LNF.Inventory;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Inventory
{
    public class InventoryItemRepository : ApiClient, IInventoryItemRepository
    {
        public IInventoryLocation GetInventoryLocation(int inventoryLocationId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IInventoryLocation> GetInventoryLocations()
        {
            throw new NotImplementedException();
        }

        public IInventoryType GetInventoryType(int inventoryTypeId)
        {
            throw new NotImplementedException();
        }

        public IInventoryItem GetItem(int itemId)
        {
            throw new NotImplementedException();
        }

        public IItemInventoryType GetItemInventoryType(int itemInventoryTypeId)
        {
            throw new NotImplementedException();
        }

        public IItemPriceHistory GetItemPriceHistory(int itemId, DateTime cutoff)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IInventoryItem> GetItems()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IInventoryItem> GetItems(int catId)
        {
            throw new NotImplementedException();
        }

        public IItemUpdate UpdateInventory(int itemId, double updateQty, double afterQty, string updateAction, int inventoryLocationId, int clientId)
        {
            throw new NotImplementedException();
        }

        public IInventoryItem UpdateItem(int itemId, string description, int stockQty, int? minStockQty, int? maxStockQty)
        {
            throw new NotImplementedException();
        }
    }
}
