using LNF.Data;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Inventory;
using LNF.Repository.Ordering;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Ordering
{
    public static class PurchaseOrderItemUtility
    {
        private static int? ConvertToNullableInt32(int value)
        {
            int? result = null;
            if (value > 0) result = value;
            return result;
        }

        public static PurchaseOrderItem Add(string partNum, string description, double unitPrice, int inventoryItemId, Vendor vendor)
        {
            PurchaseOrderItem item = new PurchaseOrderItem()
            {
                Active = true,
                Description = description,
                InventoryItemID = ConvertToNullableInt32(inventoryItemId),
                PartNum = partNum,
                UnitPrice = unitPrice,
                Vendor = vendor
            };
            DA.Current.Insert(item);
            return item;
        }

        public static PurchaseOrderItem Update(int itemId, string description, string partNum, double unitPrice, int inventoryItemId)
        {
            PurchaseOrderItem item = DA.Current.Single<PurchaseOrderItem>(itemId);
            if (item != null)
            {
                item.Description = description;
                item.PartNum = partNum;
                item.UnitPrice = unitPrice;
                item.InventoryItemID = ConvertToNullableInt32(inventoryItemId);
            }
            return item;
        }

        public static Item GetInventoryItem(this PurchaseOrderItem item)
        {
            if (item == null) return null;

            //may be null because InventoryItemID is nullable
            if (item.InventoryItemID.HasValue)
                return DA.Current.Single<Item>(item.InventoryItemID.Value);
            else
                return null;
        }

        public static Client GetClient(this PurchaseOrderItem item)
        {
            if (item == null) return null;
            //this may retrun null if ClientID = 0 (store manager)
            return item.Vendor.GetClient();
        }

        public static bool IsStoreManager(this PurchaseOrderItem item)
        {
            if (item == null) return false;
            return item.Vendor.ClientID == 0;
        }
    }
}
