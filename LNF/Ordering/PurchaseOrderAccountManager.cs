using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Ordering;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Ordering
{
    public class PurchaseOrderAccountManager : ManagerBase, IPurchaseOrderAccountManager
    {
        public PurchaseOrderAccountManager(IProvider provider) : base(provider) { }

        public IEnumerable<IAccount> AvailabePurchaseOrderAccounts(IClient client)
        {
            var accounts = GetAccounts(client);
            var activeAccounts = Provider.Data.ClientManager.ActiveAccounts(client.ClientID);
            var result = activeAccounts.Where(acct => !accounts.Any(x => x.AccountID == acct.AccountID));
            return result;
        }

        public IEnumerable<PurchaseOrderAccount> GetAccounts(IClient item)
        {
            return Session.Query<PurchaseOrderAccount>().Where(x => x.ClientID == item.ClientID && x.Active).ToList();
        }
    }
}
