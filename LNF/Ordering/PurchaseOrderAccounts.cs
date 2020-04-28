using LNF.Data;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Ordering
{
    public static class PurchaseOrderAccounts
    {
        public static IEnumerable<IAccount> AvailabePurchaseOrderAccounts(int clientId)
        {
            var accounts = ServiceProvider.Current.Ordering.PurchaseOrder.GetAccounts(clientId);
            var activeAccounts = ServiceProvider.Current.Data.Client.GetActiveClientAccounts(clientId);
            var result = activeAccounts.Where(acct => !accounts.Any(x => x.AccountID == acct.AccountID));
            return result;
        }

        public static IEnumerable<IAccount> SelectAccounts(int clientId)
        {
            var accounts = ServiceProvider.Current.Ordering.PurchaseOrder.GetActiveAccounts(clientId);
            int[] records = accounts.Select(x => x.AccountID).ToArray();
            var result = ServiceProvider.Current.Data.Client.GetClientAccounts(clientId, records);
            return result;
        }

        public static IEnumerable<IClientAccount> SelectClientAccounts(int clientId)
        {
            return ServiceProvider.Current.Data.Client.GetActiveClientAccounts(clientId);
        }

        public static IPurchaseOrderAccount InsertAccount(int clientId, int accountId)
        {
            return ServiceProvider.Current.Ordering.PurchaseOrder.AddAccount(clientId, accountId);
        }

        public static bool DeleteAccount(int clientId, int accountId)
        {
            return ServiceProvider.Current.Ordering.PurchaseOrder.DeleteAccount(clientId, accountId);
        }
    }
}
