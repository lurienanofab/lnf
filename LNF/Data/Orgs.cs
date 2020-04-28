using LNF.Cache;
using System;
using System.Collections.Generic;

namespace LNF.Data
{
    public static class Orgs
    {
        public static IEnumerable<IOrg> All() => CacheManager.Current.GetValue("Orgs", p => p.Data.Org.GetOrgs(), DateTimeOffset.Now.AddDays(1));
    }
}
