using LNF.Models.Billing;
using LNF.Models.Billing.Process;
using LNF.Models.Billing.Reports;
using System.Collections.Generic;

namespace OnlineServices.Api.Billing
{
    public class BillingApi : ApiClient, IBillingApi
    {
        public BillingApi(IAccountSubsidyClient accountSubsidy, IProcessClient process, IReportClient report, IToolClient tool, IRoomClient room, IStoreClient store, IMiscClient misc)
        {
            AccountSubsidy = accountSubsidy;
            Process = process;
            Report = report;
            Tool = tool;
            Room = room;
            Store = store;
            Misc = misc;
        }

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

        public IAccountSubsidyClient AccountSubsidy { get; }

        public IProcessClient Process { get; }

        public IReportClient Report { get; }

        public IToolClient Tool { get; }

        public IRoomClient Room { get; }

        public IStoreClient Store { get; }

        public IMiscClient Misc { get; }
    }
}
