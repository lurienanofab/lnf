using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository;
using LNF.Repository.Store;
using LNF.Repository.Inventory;

namespace LNF.Store
{
    public static class StoreOrderUtility
    {
        public static IList<StoreOrderDetail> GetDetails(this StoreOrder order)
        {
            return DA.Current.Query<StoreOrderDetail>().Where(x => x.StoreOrder == order).ToList();
        }

        public static InventoryLocation GetInventoryLocation(this StoreOrder order)
        {
            if (order.InventoryLocationID.HasValue)
                return DA.Current.Single<InventoryLocation>(order.InventoryLocationID.Value);
            else
                return null;
        }

        public static string GetPickupLocation(this StoreOrder order)
        {
            string result = string.Empty;

            if (order != null)
            {
                var iloc = order.GetInventoryLocation();
                if (iloc != null)
                    result = iloc.LocationName;
            }

            return result;
        }

        public static decimal GetTotal(this StoreOrder order, bool applyMultiplier = true)
        {
            decimal result = 0;

            IList<StoreOrderDetail> detail = order.GetDetails();
            if (detail != null && detail.Count > 0)
            {
                result = detail.Sum(x => x.Quantity * x.GetUnitPrice(applyMultiplier));
            }

            return result;
        }
    }
}
