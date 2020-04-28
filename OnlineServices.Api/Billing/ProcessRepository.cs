using LNF.Billing;
using LNF.Billing.Process;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Billing
{
    public class ProcessRepository : ApiClient, IProcessRepository
    {
        public DataCleanResult DataClean(DataCleanCommand command)
        {
            return Post<DataCleanResult>("webapi/billing/process/data/clean", command);
        }

        public DataResult Data(DataCommand command)
        {
            return Post<DataResult>("webapi/billing/process/data", command);
        }

        public Step1Result Step1(Step1Command command)
        {
            return Post<Step1Result>("webapi/billing/process/step1", command);
        }

        public PopulateSubsidyBillingResult Step4(Step4Command command)
        {
            return Post<PopulateSubsidyBillingResult>("webapi/billing/process/step4", command);
        }

        public FinalizeResult Finalize(FinalizeCommand command)
        {
            return Post<FinalizeResult>("webapi/billing/process/data/finalize", command);
        }

        public UpdateResult Update(UpdateCommand command)
        {
            return Post<UpdateResult>("webapi/billing/process/data/update", command);
        }

        public bool RemoteProcessingUpdate(RemoteProcessingUpdate command)
        {
            return Post<bool>("webapi/billing/process/remote-processing-update", command);
        }

        public int DeleteData(BillingCategory billingCategory, DateTime period, int clientId = 0, int record = 0)
        {
            return Delete("webapi/billing/process/data/{billingCategory}", UrlSegments(new { billingCategory }) & QueryStrings(new { period, clientId, record }));
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
