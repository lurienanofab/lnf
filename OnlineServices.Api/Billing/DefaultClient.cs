using LNF.Models.Billing;
using System.Collections.Generic;

namespace OnlineServices.Api.Billing
{
    public class DefaultClient : ApiClient, IDefaultClient
    {
        public string Get()
        {
            return Get("webapi/billing");
        }

        public IEnumerable<string> UpdateBilling(UpdateBillingArgs args)
        {
            return Post<List<string>>("webapi/billing/update", args);
        }

        public UpdateClientBillingResult UpdateClientBilling(UpdateClientBillingCommand model)
        {
            return Post<UpdateClientBillingResult>("webapi/billing/update-client", model);
        }
    }
}
