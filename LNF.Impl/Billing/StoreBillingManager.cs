using LNF.Models.Billing;
using LNF.Repository;
using System;
using System.Collections.Generic;

namespace LNF.Impl.Billing
{
    public class StoreBillingManager : ManagerBase, IStoreBillingManager
    {
        public StoreBillingManager(IProvider provider) : base(provider) { }

        public IEnumerable<StoreBillingItem> CreateStoreBilling(DateTime period, int clientId = 0)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<StoreDataItem> CreateStoreData(DateTime period, int clientId = 0, int itemId = 0)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<StoreBillingItem> GetStoreBilling(DateTime period, int clientId = 0, int itemId = 0)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<StoreDataItem> GetStoreData(DateTime period, int clientId = 0, int itemId = 0)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<StoreDataCleanItem> GetStoreDataClean(DateTime sd, DateTime ed, int clientId = 0, int itemId = 0)
        {
            throw new NotImplementedException();
        }
    }
}
