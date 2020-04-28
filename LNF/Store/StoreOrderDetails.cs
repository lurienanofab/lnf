using System;
using System.Collections.Generic;

namespace LNF.Store
{
    public static class StoreOrderDetails
    {
        public static decimal GetUnitPrice(this IStoreOrderDetail item, bool applyMultiplier = true)
        {
            IPrice p = ServiceProvider.Current.Store.GetPrice(item.PriceID);
            decimal baseQty = p.BaseQMultiplier;
            decimal result = p.PackagePrice / baseQty;
            if (applyMultiplier)
            {
                int accountId = item.AccountID;
                result = Prices.ApplyStoreMultiplier(result, accountId);
            }
            return result;
        }

        public static IEnumerable<IStoreOrderDetail> GetDetails(DateTime sd, DateTime ed, int clientId = 0)
        {
            return ServiceProvider.Current.Store.GetStoreOrderDetails(sd, ed, clientId);
        }

        public static IEnumerable<IStoreOrderDetail> GetDetails(int soid)
        {
            return ServiceProvider.Current.Store.GetStoreOrderDetails(soid);
        }
    }
}
