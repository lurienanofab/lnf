using System;

namespace LNF.Worker
{
    public class UpdateBillingWorkerRequest : WorkerRequest
    {
        public UpdateBillingWorkerRequest()
        {
            Command = "UpdateBilling";
            Args = new string[0];
        }

        public UpdateBillingWorkerRequest(DateTime period, int clientId, string[] billingTypes)
        {
            Command = "UpdateBilling";
            Args = new[]
            {
                period.ToString("yyyy-MM-dd"),
                clientId.ToString(),
                string.Join(",", billingTypes)
            };
        }
    }
}
