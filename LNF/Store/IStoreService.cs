using System;
using System.Collections.Generic;

namespace LNF.Store
{
    public interface IStoreService
    {
        IPrice GetPrice(int priceId);
        IEnumerable<IPrice> GetPrices(int itemId);
        IEnumerable<IStoreOrderDetail> GetStoreOrderDetails(int soid);
        IEnumerable<IStoreOrderDetail> GetStoreOrderDetails(DateTime sd, DateTime ed, int clientId = 0, string status = null);
    }
}
