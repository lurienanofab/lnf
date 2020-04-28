using System;

namespace LNF.Inventory
{
    public static class InventoryLocationExtensions
    {
        public static string GetFullLocationName(this IInventoryLocation item)
        {
            var result = InventoryLocations.GetFullLocationName(item);
            return result;
        }
    }

    public static class ItemExtensions
    {
        public static IItemInventoryType GetItemInventoryType(this IInventoryItem item)
        {
            return ServiceProvider.Current.Inventory.Item.GetItemInventoryType(item.ItemInventoryTypeID);
        }

        public static IInventoryType GetInventoryType(this IInventoryItem item)
        {
            return ServiceProvider.Current.Inventory.Item.GetInventoryType(item.InventoryTypeID);
        }

        public static IInventoryItem UpdateInventory(this IInventoryItem item, string description, int stockQuantity, int? minStockQuantity, int? maxStockQuantity)
        {
            var afterQuantity = item.GetAfterQuantity(stockQuantity, "UpdateInventory");

            if (item.StockQuantity != afterQuantity)
                ServiceProvider.Current.Inventory.Item.UpdateInventory(item.ItemID, stockQuantity, afterQuantity, "UpdateInventory", 0, 0);

            return ServiceProvider.Current.Inventory.Item
                .UpdateItem(item.ItemID, description, stockQuantity, minStockQuantity, maxStockQuantity);
        }

        public static double GetAfterQuantity(this IInventoryItem item, double updateQuantity, string updateAction)
        {
            if (updateAction == "UpdateInventory")
                return updateQuantity;
            else if (updateAction == "CheckOut")
                return item.StockQuantity - updateQuantity;
            else
                throw new NotImplementedException();
        }

        public static IInventoryItem CheckOut(this IInventoryItem item, int updateQuantity, IItemInventoryLocation itemLoc, int clientId)
        {
            var afterQuantity = item.GetAfterQuantity(updateQuantity, "CheckOut");
            int locId = (itemLoc == null) ? 0 : itemLoc.ItemInventoryLocationID;

            if (item.StockQuantity != afterQuantity)
            {
                ServiceProvider.Current.Inventory.Item.UpdateInventory(item.ItemID, updateQuantity, afterQuantity, "CheckOut", locId, clientId);
                return ServiceProvider.Current.Inventory.Item.UpdateItem(item.ItemID, item.Description, Convert.ToInt32(afterQuantity), null, null);
            }

            return item;
        }
    }

}
