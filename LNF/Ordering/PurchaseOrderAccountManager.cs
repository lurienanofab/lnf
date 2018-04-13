using LNF.Data;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Ordering;
using System.Linq;

namespace LNF.Ordering
{
    public class PurchaseOrderAccountManager : ManagerBase
    {
        public PurchaseOrderAccountManager(ISession session) : base(session) { }

        public IQueryable<Account> AvailabePurchaseOrderAccounts(Client client)
        {
            IQueryable<PurchaseOrderAccount> accounts = GetAccounts(client);
            var activeAccounts = Session.ClientManager().ActiveAccounts(client);
            var result = activeAccounts.Where(acct => !accounts.Any(x => x.AccountID == acct.AccountID));
            return result;
        }

        public IQueryable<PurchaseOrderAccount> GetAccounts(Client item)
        {
            return DA.Current.Query<PurchaseOrderAccount>().Where(x => x.ClientID == item.ClientID && x.Active);
        }
    }
}
