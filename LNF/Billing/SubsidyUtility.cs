using System;
using System.Collections.Generic;

namespace LNF.Billing
{
    public static class SubsidyUtility
    {
        public static IEnumerable<ITieredSubsidyBilling> PopulateSubsidyBilling(DateTime period, int clientId = 0)
        {
            return ServiceProvider.Current.Billing.AccountSubsidy.PopulateSubsidyBilling(period, clientId);
        }
    }
}
