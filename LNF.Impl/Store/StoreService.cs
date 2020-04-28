using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Store;
using LNF.Store;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Store
{
    public class StoreService : RepositoryBase, IStoreService
    {
        public StoreService(ISessionManager mgr) : base(mgr) { }

        public IPrice GetPrice(int priceId)
        {
            return Require<PriceInfo>(priceId);
        }

        public IEnumerable<IPrice> GetPrices(int itemId)
        {
            return Session.GetPrices(itemId);
        }

        public IEnumerable<IStoreOrderDetail> GetStoreOrderDetails(int soid)
        {
            return Session.Query<StoreOrderDetail>().Where(x => x.StoreOrder.SOID == soid).CreateModels<IStoreOrderDetail>();
        }

        public IEnumerable<IStoreOrderDetail> GetStoreOrderDetails(DateTime sd, DateTime ed, int clientId = 0, string status = null)
        {
            return Session.GetStoreOrderDetails(sd, ed, clientId, status).CreateModels<IStoreOrderDetail>();
        }
    }
}
