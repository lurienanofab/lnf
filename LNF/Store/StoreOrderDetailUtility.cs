using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository;
using LNF.Repository.Store;
using LNF.Repository.Inventory;

namespace LNF.Store
{
    public static class StoreOrderDetailUtility
    {
        public static decimal GetUnitPrice(this StoreOrderDetail item, bool applyMultiplier = true)
        {
            Price p = DA.Current.Single<Price>(item.PriceID);
            decimal baseQty = p.VendorPackage.Package.BaseQMultiplier;
            decimal result = p.PackagePrice / baseQty;
            if (applyMultiplier)
            {
                int accountId = item.StoreOrder.Account.AccountID;
                result = PriceUtility.ApplyStoreMultiplier(result, accountId);
            }
            return result;
        }

        public static StoreOrderDetail[] GetStoreDataRaw(DateTime startDate, DateTime endDate, int clientId = 0)
        {
            var query = DA.Current.Query<StoreOrderDetail>().Where(x => x.StoreOrder.StatusChangeDate >= startDate && x.StoreOrder.StatusChangeDate < endDate && x.StoreOrder.Status == "Closed");
            if (clientId > 0)
                return query.Where(x => x.StoreOrder.Client.ClientID == clientId).ToArray();
            else
                return query.ToArray();
        }
    }
}
