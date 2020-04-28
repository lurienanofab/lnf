using System.Collections.Generic;

namespace LNF.Ordering
{
    public interface IPurchaseOrderRepository
    {
        IPurchaseOrder GetPurchaseOrder(int poid);
        IEnumerable<IPurchaseOrderAccount> GetAccounts(int clientId);
        IEnumerable<IPurchaseOrderAccount> GetActiveAccounts(int clientId);
        IEnumerable<IPurchaseOrderDetail> GetDetails(int poid);
        IPurchaseOrderAccount AddAccount(int clientId, int accountId);
        bool DeleteAccount(int clientId, int accountId);
        IPurchaseOrderDetail AddDetail(int poid, int itemId, int catId, double qty, string unit, double unitPrice, bool isInventoryControlled);
        IPurchaseOrderDetail UpdateDetail(int podid, int catId, double qty, string unit, double unitPrice, bool isInventoryControlled);
    }
}
