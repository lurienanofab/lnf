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

        public IStoreOrder GetStoreOrder(int soid)
        {
            var so = Session.Get<Repository.Store.StoreOrder>(soid);
            var result = CreateStoreOrderFromEntity(so);
            return result;
        }

        public IEnumerable<IStoreOrder> GetStoreOrders(string status = null)
        {
            var list = Session.Query<Repository.Store.StoreOrder>().Where(x => x.Status.ToLower() == (string.IsNullOrEmpty(status) ? x.Status.ToLower() : status)).ToList();
            var result = list.Select(x => CreateStoreOrderFromEntity(x)).ToList();
            return result;
        }

        public IEnumerable<IStoreOrderDetail> GetStoreOrderDetails(int soid)
        {
            return Session.Query<StoreOrderDetail>().Where(x => x.StoreOrder.SOID == soid).CreateModels<IStoreOrderDetail>();
        }

        public IEnumerable<IStoreOrderDetail> GetStoreOrderDetails(DateTime sd, DateTime ed, int clientId = 0, string status = null)
        {
            return Session.GetStoreOrderDetails(sd, ed, clientId, status).CreateModels<IStoreOrderDetail>();
        }

        private IStoreOrder CreateStoreOrderFromEntity(Repository.Store.StoreOrder entity)
        {
            var result = new StoreOrder
            {
                SOID = entity.SOID,
                ClientID = entity.Client.ClientID,
                AccountID = entity.Account.AccountID,
                AccountName = entity.Account.Name,
                CreationDate = entity.CreationDate,
                Status = entity.Status,
                StatusChangeDate = entity.StatusChangeDate,
                InventoryLocationID = entity.InventoryLocationID
            };

            return result;
        }
    }
}
