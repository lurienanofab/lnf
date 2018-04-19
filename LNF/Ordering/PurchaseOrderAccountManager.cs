using LNF.Data;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Ordering;
using System.Linq;

namespace LNF.Ordering
{
    public class PurchaseOrderAccountManager : ManagerBase, IPurchaseOrderAccountManager
    {
        protected IClientManager ClientManager { get; }

        public PurchaseOrderAccountManager(ISession session, IClientManager clientManager) : base(session)
        {
            ClientManager = clientManager;
        }

        public IQueryable<Account> AvailabePurchaseOrderAccounts(Client client)
        {
            IQueryable<PurchaseOrderAccount> accounts = GetAccounts(client);
            var activeAccounts = ClientManager.ActiveAccounts(client);
            var result = activeAccounts.Where(acct => !accounts.Any(x => x.AccountID == acct.AccountID));
            return result;
        }

        public IQueryable<PurchaseOrderAccount> GetAccounts(Client item)
        {
            return Session.Query<PurchaseOrderAccount>().Where(x => x.ClientID == item.ClientID && x.Active);
        }
    }
}
