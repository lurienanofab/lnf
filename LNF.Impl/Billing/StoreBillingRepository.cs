using LNF.Billing;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using System;
using System.Collections.Generic;

namespace LNF.Impl.Billing
{
    public class StoreBillingRepository : RepositoryBase, IStoreBillingRepository
    {
        public StoreBillingRepository(ISessionManager mgr) : base(mgr) { }

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
