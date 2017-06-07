using System;
using System.Linq;

namespace LNF.Repository.Reporting
{
    public class UserUsageCharge : UsageChargeBase
    {
        public static IQueryable<UserUsageCharge> SelectByUser(int clientId, DateTime period)
        {
            return DA.Current.Query<UserUsageCharge>().Where(x => x.Period == period && x.ClientID == clientId);
        }
    }
}
