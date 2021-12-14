using LNF.Billing;
using RestSharp;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Billing
{
    public class MiscBillingRepository : ApiClient, IMiscBillingRepository
    {
        internal MiscBillingRepository(IRestClient rc) : base(rc) { }

        public IMiscBillingCharge GetMiscBillingCharge(int expId)
        {
            return Get<MiscBillingCharge>("webapi/billing/{expId}", UrlSegments(new { expId }));
        }

        public IEnumerable<IMiscBillingChargeItem> GetMiscBillingCharges(DateTime period, string[] types, int clientId = 0, int accountId = 0, bool? active = null)
        {
            return Get<List<MiscBillingChargeItem>>("webapi/billing/misc", QueryStrings(new { period, clientId, accountId, types, active }));
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
