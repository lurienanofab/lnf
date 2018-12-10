using LNF.Models.Billing.Process;
using LNF.Models.Billing.Reports;

namespace LNF.Models.Billing
{
    public interface IBillingApi
    {
        IDefaultClient Default { get; }
        IAccountSubsidyClient AccountSubsidy { get; }
        IProcessClient Process { get; }
        IReportClient Report { get; }
        IToolClient Tool { get; }
        IRoomClient Room { get; }
        IStoreClient Store { get; }
    }
}
