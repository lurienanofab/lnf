using LNF.Inventory;
using System.Linq;

namespace LNF.Store
{
    public static class StoreOrders
    {
        public static IInventoryLocation GetInventoryLocation(IStoreOrder order)
        {
            if (order.InventoryLocationID.HasValue)
                return ServiceProvider.Current.Inventory.Item.GetInventoryLocation(order.InventoryLocationID.Value);
            else
                return null;
        }

        public static string GetPickupLocation(IStoreOrder order)
        {
            string result = string.Empty;

            if (order != null)
            {
                var iloc = GetInventoryLocation(order);
                if (iloc != null)
                    result = iloc.LocationName;
            }

            return result;
        }

        public static decimal GetTotal(IStoreOrder order, bool applyMultiplier = true)
        {
            decimal result = 0;

            var details = StoreOrderDetails.GetDetails(order.SOID);
            if (details != null && details.Count() > 0)
            {
                result = details.Sum(x => x.Quantity * x.GetUnitPrice(applyMultiplier));
            }

            return result;
        }
    }
}
