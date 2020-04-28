using LNF.Cache;
using System;
using System.Collections.Generic;

namespace LNF.Data
{
    public static class AccountTypes
    {
        public static IEnumerable<IAccountType> All() => CacheManager.Current.GetValue("AccountTypes", p => p.Data.Account.GetAccountTypes(), DateTimeOffset.Now.AddDays(7));
    }
}
