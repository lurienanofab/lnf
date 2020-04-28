using LNF.Cache;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public static class ClientOrgs
    {
        public static IEnumerable<IClient> All() => CacheManager.Current.GetValue("ClientOrgs", p => p.Data.Client.GetClientOrgs(), DateTimeOffset.Now.AddHours(1));

        public static IClient Find(int clientOrgId)
        {
            return All().FirstOrDefault(x => x.ClientOrgID == clientOrgId);
        }

        public static IClient Find(int clientId, int rank)
        {
            return All().FirstOrDefault(x => x.ClientID == clientId && x.EmailRank == rank);
        }
    }
}
