using LNF.Repository;
using LNF.Repository.Billing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Billing
{
    public static class AccountSubsidyUtility
    {
        public static IEnumerable<AccountSubsidy> GetActive(DateTime sd, DateTime ed)
        {
            //base query
            var baseQuery = DA.Current.Query<AccountSubsidy>()
                .Where(x => x.EnableDate < ed && (x.DisableDate == null || x.DisableDate > sd))
                .ToArray();

            // step1: it is possible to have duplicates because of disabling and re-enabling in the same
            //        date range, in this case get the last one by joining to self grouped by max AccountSubsidyID
            var step1 = baseQuery.Join(
                baseQuery.GroupBy(x => x.AccountID).Select(g => new { Account = g.Key, AccountSubsidyID = g.Max(n => n.AccountSubsidyID) }),
                o => o.AccountSubsidyID,
                i => i.AccountSubsidyID,
                (o, i) => o);

            return step1.OrderBy(x => x.AccountID).ToArray();
        }
    }
}
