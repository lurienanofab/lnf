using LNF.Models.Billing;
using LNF.Models.Billing.Process;
using LNF.Models.Billing.Reports;

namespace OnlineServices.Api.Billing
{
    public class BillingApi : IBillingApi
    {
        public BillingApi(IDefaultClient @default, IAccountSubsidyClient accountSubsidy, IProcessClient process, IReportClient report, IToolClient tool, IRoomClient room, IStoreClient store)
        {
            Default = @default;
            AccountSubsidy = accountSubsidy;
            Process = process;
            Report = report;
            Tool = tool;
            Room = room;
            Store = store;
        }

        public IDefaultClient Default { get; }

        public IAccountSubsidyClient AccountSubsidy { get; }

        public IProcessClient Process { get; }

        public IReportClient Report { get; }

        public IToolClient Tool { get; }

        public IRoomClient Room { get; }

        public IStoreClient Store { get; }
    }
}
