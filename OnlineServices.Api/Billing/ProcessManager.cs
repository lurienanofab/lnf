using LNF.Models.Billing;
using LNF.Models.Billing.Process;
using System;

namespace OnlineServices.Api.Billing
{
    public class ProcessManager : ApiClient, IProcessManager
    {
        public BillingProcessDataCleanResult BillingProcessDataClean(BillingProcessDataCleanCommand command)
        {
            return Post<BillingProcessDataCleanResult>("webapi/billing/process/data/clean", command);
        }

        public BillingProcessDataResult BillingProcessData(BillingProcessDataCommand command)
        {
            return Post<BillingProcessDataResult>("webapi/billing/process/data", command);
        }

        public BillingProcessStep1Result BillingProcessStep1(BillingProcessStep1Command command)
        {
            return Post<BillingProcessStep1Result>("webapi/billing/process/step1", command);
        }

        public PopulateSubsidyBillingProcessResult BillingProcessStep4(BillingProcessStep4Command command)
        {
            return Post<PopulateSubsidyBillingProcessResult>("webapi/billing/process/step4", command);
        }

        public DataFinalizeProcessResult BillingProcessDataFinalize(BillingProcessDataFinalizeCommand command)
        {
            return Post<DataFinalizeProcessResult>("webapi/billing/process/data/finalize", command);
        }

        public DataUpdateProcessResult BillingProcessDataUpdate(BillingProcessDataUpdateCommand command)
        {
            return Post<DataUpdateProcessResult>("webapi/billing/process/data/update", command);
        }

        public bool RemoteProcessingUpdate(RemoteProcessingUpdate command)
        {
            return Post<bool>("webapi/billing/process/remote-processing-update", command);
        }

        public int DeleteData(BillingCategory billingCategory, DateTime period, int clientId = 0, int record = 0)
        {
            return Delete("webapi/billing/process/data/{billingCategory}", UrlSegments(new { billingCategory }) & QueryStrings(new { period, clientId, record }));
        }
    }
}
