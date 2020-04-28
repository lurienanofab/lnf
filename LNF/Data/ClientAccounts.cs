using LNF.Cache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public static class ClientAccounts
    {
        public static IEnumerable<IClientAccount> All() => CacheManager.Current.GetValue("ClientAccounts", p => p.Data.Client.GetClientAccounts(), DateTimeOffset.Now.AddHours(1));

        public static IEnumerable<IClientAccount> Search(Func<IClientAccount, bool> filter)
        {
            return All().Where(filter).ToList();
        }

        public static IClientAccount Find(Func<IClientAccount, bool> filter)
        {
            return All().FirstOrDefault(filter);
        }

        public static IClientAccount Find(int clientAccountId)
        {
            return All().FirstOrDefault(x => x.ClientAccountID == clientAccountId);
        }

        public static IClientAccount FindByClientOrg(int clientOrgId, int accountId)
        {
            return All().FirstOrDefault(x => x.ClientOrgID == clientOrgId && x.AccountID == accountId);
        }

        public static IClientAccount FindByClient(int clientId, int accountId)
        {
            return All().FirstOrDefault(x => x.ClientID == clientId && x.AccountID == accountId);
        }
    }
}
