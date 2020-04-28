using System;
using System.Collections.Generic;

namespace LNF.Billing
{
    public interface IStoreBillingRepository
    {
        IEnumerable<StoreBillingItem> CreateStoreBilling(DateTime period, int clientId = 0);
        IEnumerable<StoreDataItem> CreateStoreData(DateTime period, int clientId = 0, int itemId = 0);
        IEnumerable<StoreDataItem> GetStoreData(DateTime period, int clientId = 0, int itemId = 0);
        IEnumerable<StoreDataCleanItem> GetStoreDataClean(DateTime sd, DateTime ed, int clientId = 0, int itemId = 0);
        IEnumerable<StoreBillingItem> GetStoreBilling(DateTime period, int clientId = 0, int itemId = 0);
    }
}