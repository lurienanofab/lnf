using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Ordering;
using System.Collections.Generic;

namespace LNF.Ordering
{
    public interface IPurchaseOrderAccountManager : IManager
    {
        IEnumerable<IAccount> AvailabePurchaseOrderAccounts(IClient client);
        IEnumerable<PurchaseOrderAccount> GetAccounts(IClient item);
    }
}