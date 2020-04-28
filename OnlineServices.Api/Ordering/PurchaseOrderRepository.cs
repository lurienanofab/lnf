using LNF.Ordering;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Ordering
{
    public class PurchaseOrderRepository : ApiClient, IPurchaseOrderRepository
    {
        public IPurchaseOrderAccount AddAccount(int clientId, int accountId)
        {
            throw new NotImplementedException();
        }

        public IPurchaseOrderDetail AddDetail(int poid, int itemId, int catId, double qty, string unit, double unitPrice, bool isInventoryControlled)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAccount(int clientId, int accountId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPurchaseOrderAccount> GetAccounts(int clientId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPurchaseOrderAccount> GetActiveAccounts(int clientId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPurchaseOrderDetail> GetDetails(int poid)
        {
            throw new NotImplementedException();
        }

        public IPurchaseOrder GetPurchaseOrder(int poid)
        {
            throw new NotImplementedException();
        }

        public IPurchaseOrderDetail UpdateDetail(int podid, int catId, double qty, string unit, double unitPrice, bool isInventoryControlled)
        {
            throw new NotImplementedException();
        }
    }
}
