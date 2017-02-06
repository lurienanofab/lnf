using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Ordering;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Ordering
{
    public static class PurchaseOrderAccountUtility
    {
        public static IList<IOFAccount> SelectIOFAccounts(int clientId)
        {
            IList<PurchaseOrderAccount> query = DA.Current.Query<PurchaseOrderAccount>().Where(x => x.ClientID == clientId && x.Active).ToList();
            int[] records = query.Select(x => x.AccountID).ToArray();
            IList<ClientAccountInfo> accts = DA.Current.Query<ClientAccountInfo>().Where(x => records.Contains(x.AccountID) && x.ClientID == clientId).ToList();
            List<IOFAccount> result = new List<IOFAccount>();
            result.AddRange(accts.Select(x => new IOFAccount(x)).ToArray());
            return result;
        }

        public static IList<IOFClientAccount> SelectIOFClientAccounts(int clientId)
        {
            IList<ClientAccountInfo> accts = DA.Current.Query<ClientAccountInfo>().Where(x => x.ClientID == clientId && x.ClientAccountActive && x.ClientOrgActive).ToList();
            List<IOFClientAccount> result = new List<IOFClientAccount>();
            result.AddRange(accts.Select(x => new IOFClientAccount(x)).ToArray());
            return result;
        }

        public static bool InsertAccount(int clientId, int accountId)
        {
            PurchaseOrderAccount acct = new PurchaseOrderAccount { AccountID = accountId, ClientID = clientId };

            PurchaseOrderAccount existingAcct = DA.Current.Single<PurchaseOrderAccount>(acct);

            if (existingAcct == null)
            {
                //insert new
                acct.Active = true;
                DA.Current.Insert(acct);
            }
            else
            {
                //update existing
                existingAcct.Active = true;
            }

            return true;
        }

        public static bool DeleteAccount(int clientId, int accountId)
        {
            PurchaseOrderAccount acct = DA.Current.Single<PurchaseOrderAccount>(new PurchaseOrderAccount { AccountID = accountId, ClientID = clientId });

            if (acct == null) return false;

            acct.Active = false;

            return true;
        }
    }
}
