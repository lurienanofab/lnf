using LNF.CommonTools;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Data
{
    public static class ClientPreferenceUtility
    {
        public static IClientManager ClientManager => ServiceProvider.Current.Use<IClientManager>();

        public static ClientPreference Find(int clientId, string app)
        {
            //It's possible to have client.ClientID == 0 if no user is logged in. In this case result will be null.
            if (clientId == 0)
                return null;

            ClientPreference result = DA.Current.Query<ClientPreference>().FirstOrDefault(x => x.Client.ClientID == clientId && x.ApplicationName == app);

            if (result == null)
            {
                result = new ClientPreference() { Client = DA.Current.Single<Client>(clientId), ApplicationName = app };
                DA.Current.Insert(result);
            }

            return result;
        }

        public static IList<Account> OrderAccountsByUserPreference(int clientId, IEnumerable<Account> accounts = null)
        {
            if (accounts == null)
            {
                Client client = DA.Current.Single<Client>(clientId);
                accounts = ClientManager.ActiveAccounts(client);
            }

            if (accounts == null) return null;

            return OrderListByUserPreference(clientId, accounts, x => x.AccountID, x => x.Name);
        }

        public static IList<T> OrderListByUserPreference<T>(int clientId, IEnumerable<T> items, Func<T, int> id, Func<T, string> defaultOrder)
        {
            List<T> result = new List<T>();

            if (items.Count() == 0)
                return result;

            string pref = string.Empty;

            //for now the acct order prefence is stored in sselScheduler.dbo.ClientSetting but we should move this to sselData.dbo.ClientPreference
            ClientSetting cs = DA.Current.Query<ClientSetting>().FirstOrDefault(x => x.ClientID == clientId);

            if (null == cs)
                return items.OrderBy(defaultOrder).ToList();

            pref = cs.AccountOrder;

            //in the future...
            //ClientPreference cp = Find(client, "common");
            //pref = cp.GetPreference("account-order", string.Empty);

            int[] accountOrder = Utility.ConvertStringToIntArray(pref);

            if (accountOrder == null)
                return items.OrderBy(defaultOrder).ToList();

            return OrderListByUserPreference(accountOrder, items, id, defaultOrder);
        }

        public static IList<T> OrderListByUserPreference<T>(int[] accountOrder, IEnumerable<T> items, Func<T, int> id, Func<T, string> defaultOrder)
        {
            List<T> list = items.ToList();
            List<T> result = new List<T>();

            foreach (int acctId in accountOrder)
            {
                //check if it is in active accounts if so add it to result
                T acct = list.FirstOrDefault(x => id(x) == acctId);

                if (acct != null)
                {
                    result.Add(acct);
                    //this is used to add the remaining elements into result
                    list.Remove(acct);
                }
            }

            if (list.Count > 0)
                result.AddRange(list.OrderBy(defaultOrder).ToList());

            return result;
        }
    }
}
