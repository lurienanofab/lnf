using LNF.CommonTools;
using LNF.Scheduler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Data
{
    public class ClientPreferenceUtility
    {
        public IProvider Provider { get; }

        public ClientPreferenceUtility(IProvider provider)
        {
            Provider = provider;
        }

        public static IClientPreference Find(int clientId, string app)
        {
            //It's possible to have client.ClientID == 0 if no user is logged in. In this case result will be null.
            if (clientId == 0)
                return null;

            return ServiceProvider.Current.Data.Client.GetClientPreference(clientId, app);
        }

        public IList<IAccount> OrderAccountsByUserPreference(IClient client, IEnumerable<IAccount> accounts = null)
        {
            if (accounts == null)
            {
                accounts = Provider.Data.Client.GetActiveAccounts(client.ClientID);
            }

            if (accounts == null) return null;

            return OrderListByUserPreference(client, accounts, x => x.AccountID, x => x.AccountName);
        }

        public static IList<T> OrderListByUserPreference<T>(IClient client, IEnumerable<T> items, Func<T, int> id, Func<T, string> defaultOrder)
        {
            //for now the acct order prefence is stored in sselScheduler.dbo.ClientSetting but we should move this to sselData.dbo.ClientPreference
            IClientSetting cs = ServiceProvider.Current.Scheduler.ClientSetting.GetClientSettingOrDefault(client.ClientID);
            return OrderListByUserPreference<T>(cs, items, id, defaultOrder);
        }

        public static IList<T> OrderListByUserPreference<T>(IClientSetting cs, IEnumerable<T> items, Func<T, int> id, Func<T, string> sort)
        {
            List<T> result = new List<T>();

            if (items.Count() == 0)
                return result;

            string pref = string.Empty;

            if (null == cs)
                return items.OrderBy(sort).ToList();

            pref = cs.AccountOrder;

            //in the future...
            //ClientPreference cp = Find(client, "common");
            //pref = cp.GetPreference("account-order", string.Empty);

            int[] accountOrder = Utility.ConvertStringToIntArray(pref);

            if (accountOrder == null)
                return items.OrderBy(sort).ToList();

            return OrderListByUserPreference(accountOrder, items, id, sort);
        }

        public static IList<T> OrderListByUserPreference<T>(int[] accountOrder, IEnumerable<T> items, Func<T, int> id, Func<T, string> sort)
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
                result.AddRange(list.OrderBy(sort).ToList());

            return result;
        }
    }
}
