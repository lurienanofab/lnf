using LNF.Models.Billing.Process;
using LNF.Models.Billing.Reports;
using System.Collections.Generic;

namespace LNF.Models.Billing
{
    public interface IBillingService
    {
        IProcessManager Process { get; }
        IApportionmentManager Apportionment { get; }
        IAccountSubsidyManager AccountSubsidy { get; }
        IReportManager Report { get; }
        IToolManager Tool { get; }
        IRoomManager Room { get; }
        IStoreManager Store { get; }
        IMiscManager Misc { get; }

        IEnumerable<string> UpdateBilling(UpdateBillingArgs args);
        UpdateClientBillingResult UpdateClientBilling(UpdateClientBillingCommand model);
    }
}
