using LNF.Store;
using RestSharp;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Store
{
    public class StoreService : ApiClient, IStoreService
    {
        internal StoreService(IRestClient rc) : base(rc) { }

        public IPrice GetPrice(int priceId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPrice> GetPrices(int itemId)
        {
            throw new NotImplementedException();
        }

        public IStoreOrder GetStoreOrder(int soid)
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

        public IEnumerable<IStoreOrder> GetStoreOrders(string status = null)
        {
            throw new NotImplementedException();
        }
    }
}
