using LNF.Repository;
using LNF.Repository.Inventory;
using System;

namespace LNF.Inventory
{
    public static class InventoryItemExtensions
    {
        public static Item GetItem(this InventoryItem item)
        {
            return DA.Current.Single<Item>(item.ItemID);
        }

        public static ItemInventoryType GetItemInventoryType(this InventoryItem item)
        {
            return DA.Current.Single<ItemInventoryType>(item.ItemInventoryTypeID);
        }

        public static InventoryType GetInventoryType(this InventoryItem item)
        {
            return DA.Current.Single<InventoryType>(item.InventoryTypeID);
        }
    }

    public static class InventoryLocationExtensions
    {
        public static string GetFullLocationName(this InventoryLocation item)
        {
            var result = InventoryLocationUtility.GetFullLocationName(item);
            return result;
        }
    }

    public static class ItemExtensions
    {
        public static Item UpdateInventory(this Item item, string description, int stockQuantity, int? minStockQuantity, int? maxStockQuantity)
        {
            if (item.StockQuantity != stockQuantity)
            {
                ItemUpdate iu = new ItemUpdate
                {
                    Item = item,
                    BeforeQty = item.StockQuantity,
                    UpdateQty = stockQuantity,
                    AfterQty = stockQuantity,
                    UpdateDateTime = DateTime.Now,
                    UpdateAction = "UpdateInventory"
                };
                DA.Current.Insert(iu);
            }

            item.Description = description;
            item.StockQuantity = stockQuantity;
            item.MinStockQuantity = minStockQuantity;
            item.MaxStockQuantity = maxStockQuantity;

            return item;
        }

        public static void CheckOut(this Item item, int quantity, ItemInventoryLocation itemLoc, int clientId)
        {
            int newqty = item.StockQuantity - quantity;
            if (item.StockQuantity != newqty)
            {
                int locId = (itemLoc == null) ? 0 : itemLoc.ItemInventoryLocationID;

                ItemUpdate iu = new ItemUpdate
                {
                    Item = item,
                    BeforeQty = item.StockQuantity,
                    UpdateQty = quantity,
                    AfterQty = newqty,
                    UpdateDateTime = DateTime.Now,
                    UpdateAction = "CheckOut",
                    ItemInventoryLocationID = locId,
                    ClientID = clientId
                };

                DA.Current.Insert(iu);

                item.StockQuantity = newqty;
            }
        }
    }

}
