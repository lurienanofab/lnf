using System;

namespace LNF.Models.Worker
{
    public class UpdateBillingWorkerRequest : WorkerRequest
    {
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
