using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Data;
using LNF.Billing;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Billing;

namespace LNF.Billing
{
    public static class SubsidyUtility
    {
        public static IEnumerable<TieredSubsidyBilling> PopulateSubsidyBilling(DateTime period, Client client = null)
        {
            IList<RoomBillingByAccount> roomQuery;
            IList<ToolBillingByAccount> toolQuery;

            if (client == null)
            {
                roomQuery = DA.Current.Query<RoomBillingByAccount>().Where(x => x.Period == period).ToList();
                toolQuery = DA.Current.Query<ToolBillingByAccount>().Where(x => x.Period == period).ToList();
            }
            else
            {
                roomQuery = DA.Current.Query<RoomBillingByAccount>().Where(x => x.Period == period && x.Client == client).ToList();
                toolQuery = DA.Current.Query<ToolBillingByAccount>().Where(x => x.Period == period && x.Client == client).ToList();
            }
            
            List<TieredSubsidyBilling> result = new List<TieredSubsidyBilling>();

            foreach (RoomBillingByAccount item in roomQuery)
            {
                TieredSubsidyBilling tsb = new TieredSubsidyBilling();
                tsb.Period = period;
                tsb.Client = item.Client;
                tsb.Org = item.Account.Org;
                tsb.RoomSum = item.TotalCharge;
                tsb.RoomMiscSum = 0;
                tsb.ToolSum = 0;
                tsb.ToolMiscSum = 0;
                result.Add(tsb);
            }

            return result;
        }
    }
}
