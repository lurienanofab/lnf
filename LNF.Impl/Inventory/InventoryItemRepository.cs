using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Inventory;
using LNF.Impl.Repository.Store;
using LNF.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Inventory
{
    public class InventoryItemRepository : RepositoryBase, IInventoryItemRepository
    {
        public InventoryItemRepository(ISessionManager mgr) : base(mgr) { }

        public IEnumerable<IInventoryItem> GetItems()
        {
            return Session.Query<InventoryItem>().ToList();
        }

        public IEnumerable<IInventoryItem> GetItems(int catId)
        {
            return Session.Query<InventoryItem>().Where(x => x.CatID == catId).ToList();
        }

        public IInventoryItem GetItem(int itemId)
        {
            return Session.Get<InventoryItem>(itemId);
        }

        public IInventoryLocation GetInventoryLocation(int inventoryLocationId)
        {
            return Require<InventoryLocation>(inventoryLocationId);
        }

        public IEnumerable<IInventoryLocation> GetInventoryLocations()
        {
            return Session.Query<InventoryLocation>().ToList();
        }

        public IInventoryType GetInventoryType(int inventoryTypeId)
        {
            return Session.Get<InventoryType>(inventoryTypeId);
        }

        public IItemInventoryType GetItemInventoryType(int itemInventoryTypeId)
        {
            return Session.Get<ItemInventoryType>(itemInventoryTypeId);
        }

        public IItemUpdate UpdateInventory(int itemId, double updateQty, double afterQty, string updateAction, int inventoryLocationId, int clientId)
        {
            var item = Require<InventoryItem>(itemId);

            var itemUpdate = new ItemUpdate
            {
                ItemID = itemId,
                BeforeQty = item.StockQuantity,
                UpdateQty = updateQty,
                AfterQty = afterQty,
                UpdateDateTime = DateTime.Now,
                UpdateAction = updateAction,
                ItemInventoryLocationID = inventoryLocationId,
                ClientID = clientId
            };

            Session.Save(itemUpdate);

            return itemUpdate;
        }

        public IInventoryItem UpdateItem(int itemId, string description, int stockQty, int? minStockQty, int? maxStockQty)
        {
            var item = Require<InventoryItem>(itemId);

            item.Description = description;
            item.StockQuantity = stockQty;

            if (minStockQty.HasValue)
                item.MinStockQuantity = minStockQty.Value;

            if (maxStockQty.HasValue)
                item.MaxStockQuantity = maxStockQty.Value;

            Session.Update(item);

            return item;
        }

        public IItemPriceHistory GetItemPriceHistory(int itemId, DateTime cutoff)
        {
            return Session.Query<ItemPriceHistory>()
                .Where(x => x.ItemID == itemId && x.DateActive < cutoff)
                .OrderByDescending(x => x.DateActive)
                .FirstOrDefault();
        }
    }
}
