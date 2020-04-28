using LNF.Billing;
using System;
using System.Data;

namespace OnlineServices.Api.Billing
{
    public class StoreDataRepository : ApiClient, IStoreDataRepository
    {
        public int DeleteStoreDataClean(DateTime sd, DateTime ed, int clientId = 0, int itemId = 0, int catId = 0)
        {
            throw new NotImplementedException();
        }

        public int LoadDryBoxBilling(DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public DataTable ReadStoreData(DateTime period, int clientId = 0, int itemId = 0)
        {
            throw new NotImplementedException();
        }

        public DataTable ReadStoreDataClean(DateTime sd, DateTime ed, int clientId = 0, int itemId = 0, StoreDataCleanOption option = StoreDataCleanOption.AllItems)
        {
            throw new NotImplementedException();
        }

        public DataTable ReadStoreDataRaw(DateTime sd, DateTime ed, int clientId = 0)
        {
            throw new NotImplementedException();
        }
    }
}
