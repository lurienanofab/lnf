using LNF.Billing;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Billing
{
    public class MiscBillingRepository : ApiClient, IMiscBillingRepository
    {
        public IMiscBillingCharge GetMiscBillingCharge(int expId)
        {
            return Get<MiscBillingCharge>("webapi/billing/{expId}", UrlSegments(new { expId }));
        }

        public IEnumerable<IMiscBillingCharge> GetMiscBillingCharges(DateTime period, int clientId = 0, int accountId = 0, string[] types = null, bool? active = null)
        {
            return Get<List<MiscBillingCharge>>("webapi/billing/misc", QueryStrings(new { period, clientId, accountId, types, active }));
        }

        public int CreateMiscBillingCharge(MiscBillingChargeCreateArgs args)
        {
            return Post<int>("webapi/billing/misc/create", args);
        }

        public int UpdateMiscBilling(MiscBillingChargeUpdateArgs args)
        {
            return Post<int>("webapi/billing/misc/update", args);
        }

        public int DeleteMiscBillingCharge(int expId)
        {
            return Delete("webapi/billing/misc/delete/{expId}", UrlSegments(new { expId }));
        }
    }
}
