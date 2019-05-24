using LNF.Models.Billing;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Billing
{
    public class StoreBillingManager : ApiClient, IStoreBillingManager
    {
        public IEnumerable<StoreBillingItem> CreateStoreBilling(DateTime period, int clientId = 0)
        {
            return Get<List<StoreBillingItem>>("webapi/billing/store/create", QueryStrings(new { period, clientId }));
        }

        public IEnumerable<StoreDataItem> CreateStoreData(DateTime period, int clientId = 0, int itemId = 0)
        {
            return Get<List<StoreDataItem>>("webapi/billing/store/data/create", QueryStrings(new { period, clientId, itemId }));
        }

        public IEnumerable<StoreBillingItem> GetStoreBilling(DateTime period, int clientId = 0, int itemId = 0)
        {
            return Get<List<StoreBillingItem>>("webapi/billing/store", QueryStrings(new { period, clientId, itemId }));
        }

        public IEnumerable<StoreDataItem> GetStoreData(DateTime period, int clientId = 0, int itemId = 0)
        {
            return Get<List<StoreDataItem>>("webapi/billing/store/data", QueryStrings(new { period, clientId, itemId }));
        }

        public IEnumerable<StoreDataCleanItem> GetStoreDataClean(DateTime sd, DateTime ed, int clientId = 0, int itemId = 0)
        {
            return Get<List<StoreDataCleanItem>>("webapi/billing/store/data/clean", QueryStrings(new { sd, ed, clientId, itemId }));
        }
    }
}
