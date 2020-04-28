using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Impl.Repository.Store
{
    public static class SessionExtensions
    {
        public static IList<PriceInfo> GetPrices(this ISession session, int itemId)
        {
            return session.Query<PriceInfo>().Where(x => x.ItemID == itemId).ToList();
        }

        public static IList<StoreOrderDetail> GetStoreOrderDetails(this ISession session, DateTime sd, DateTime ed, int clientId = 0, string status = null)
        {
            var query = session.Query<StoreOrderDetail>().Where(x => x.StoreOrder.StatusChangeDate >= sd && x.StoreOrder.StatusChangeDate < ed);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(x => x.StoreOrder.Status == status);

            if (clientId > 0)
                query = query.Where(x => x.StoreOrder.Client.ClientID == clientId);

            return query.ToList();
        }
    }
}
