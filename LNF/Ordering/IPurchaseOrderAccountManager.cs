using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Ordering;
using System.Linq;

namespace LNF.Ordering
{
    public interface IPurchaseOrderAccountManager : IManager
    {
        IQueryable<Account> AvailabePurchaseOrderAccounts(Client client);
        IQueryable<PurchaseOrderAccount> GetAccounts(Client item);
    }
}