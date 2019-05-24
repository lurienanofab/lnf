using LNF.Models.Billing.Process;
using LNF.Models.Billing.Reports;

namespace LNF.Models.Billing
{
    public class BillingService : IBillingServices
    {
        public IProcessManager Process { get; }

        public IApportionmentManager Apportionment { get; }

        public IAccountSubsidyManager AccountSubsidy { get; }

        public IReportManager Report { get; }

        public IToolBillingManager Tool { get; }

        public IRoomBillingManager Room { get; }

        public IStoreBillingManager Store { get; }

        public IMiscBillingManager Misc { get; }

        public IBillingTypeManager BillingType { get; }

        public BillingService(
            IProcessManager process,
            IApportionmentManager apportionment,
            IAccountSubsidyManager accountSubsidy,
            IReportManager report,
            IToolBillingManager tool,
            IRoomBillingManager room,
            IStoreBillingManager store,
            IMiscBillingManager misc,
            IBillingTypeManager billingType)
        {
            Process = process;
            Apportionment = apportionment;
            AccountSubsidy = accountSubsidy;
            Report = report;
            Tool = tool;
            Room = room;
            Store = store;
            Misc = misc;
            BillingType = billingType;
        }
    }
}
