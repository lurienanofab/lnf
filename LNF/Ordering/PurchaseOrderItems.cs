using LNF.Data;
using LNF.Inventory;

namespace LNF.Ordering
{
    public static class PurchaseOrderItems
    {
        public static IPurchaseOrderItem Add(string partNum, string description, double unitPrice, int inventoryItemId, int vendorId)
        {
            return ServiceProvider.Current.Ordering.Item.AddItem(partNum, description, unitPrice, inventoryItemId, vendorId);
        }

        public static IPurchaseOrderItem Update(int itemId, string partNum, string description, double unitPrice, int inventoryItemId)
        {
            return ServiceProvider.Current.Ordering.Item.UpdateItem(itemId, partNum, description, unitPrice, inventoryItemId);
        }

        public static IInventoryItem GetInventoryItem(this IPurchaseOrderItem item)
        {
            if (item == null) return null;

            //may be null because InventoryItemID is nullable
            if (item.InventoryItemID.HasValue)
                return ServiceProvider.Current.Inventory.Item.GetItem(item.InventoryItemID.Value);
            else
                return null;
        }

        public static IClient GetClient(this PurchaseOrderItem item)
        {
            if (item == null) return null;
            //this may retrun null if ClientID = 0 (store manager)
            return ServiceProvider.Current.Data.Client.GetClient(item.ClientID);
        }

        public static bool IsStoreManager(this PurchaseOrderItem item)
        {
            if (item == null) return false;
            IVendor vendor = ServiceProvider.Current.Ordering.Vendor.GetVendor(item.VendorID);
            if (vendor == null) return false;
            return vendor.ClientID == 0;
        }

        public static string CleanString(string value)
        {
            // Converts to lower case and strips spaces.
            if (string.IsNullOrEmpty(value))
                return string.Empty;
            else
                return value.ToLower().Replace(" ", string.Empty);
        }
    }
}
