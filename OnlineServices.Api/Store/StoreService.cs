using LNF.Store;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Store
{
    public class StoreService : ApiClient, IStoreService
    {
        public IPrice GetPrice(int priceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPrice> GetPrices(int itemId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IStoreOrderDetail> GetStoreOrderDetails(int soid)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IStoreOrderDetail> GetStoreOrderDetails(DateTime sd, DateTime ed, int clientId = 0, string status = null)
        {
            throw new NotImplementedException();
        }
    }
}
