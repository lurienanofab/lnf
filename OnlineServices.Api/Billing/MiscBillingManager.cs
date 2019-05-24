using LNF.Models.Billing;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Billing
{
    public class MiscBillingManager : ApiClient, IMiscBillingManager
    {
        public IMiscBillingCharge GetMiscBillingCharge(int expId)
        {
            return Get<MiscBillingChargeItem>("webapi/billing/{expId}", UrlSegments(new { expId }));
        }

        public IEnumerable<IMiscBillingCharge> GetMiscBillingCharges(DateTime period, int? clientId = null, bool? active = null)
        {
            return Get<List<MiscBillingChargeItem>>("webapi/billing/misc", QueryStrings(new { period, clientId, active }));
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
