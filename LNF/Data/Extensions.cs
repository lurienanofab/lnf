using System;
using System.Collections.Generic;

namespace LNF.Data
{
    public static class Extenstions
    {
        public static bool HasPriv(this ClientPrivilege priv1, ClientPrivilege priv2)
        {
            return (priv1 & priv2) > 0;
        }

        public static IEnumerable<ICost> GetToolCosts(this ICostRepository cost, DateTime period, int resourceId)
        {
            var cutoff = period.AddMonths(1);
            var costs = cost.FindToolCosts(resourceId, cutoff);
            return costs;
        }
    }
}
