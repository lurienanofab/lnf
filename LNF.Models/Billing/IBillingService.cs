using LNF.Models.Billing.Process;
using LNF.Models.Billing.Reports;

namespace LNF.Models.Billing
{
    public interface IBillingServices
    {
        IProcessManager Process { get; }
        IApportionmentManager Apportionment { get; }
        IAccountSubsidyManager AccountSubsidy { get; }
        IReportManager Report { get; }
        IToolBillingManager Tool { get; }
        IRoomBillingManager Room { get; }
        IStoreBillingManager Store { get; }
        IMiscBillingManager Misc { get; }
        IBillingTypeManager BillingType { get; }
    }
}
