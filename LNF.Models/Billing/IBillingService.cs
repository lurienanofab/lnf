using LNF.Models.Billing.Process;
using LNF.Models.Billing.Reports;
using System.Collections.Generic;

namespace LNF.Models.Billing
{
    public interface IBillingService
    {
        IProcessManager ProcessManager { get; }
        IApportionmentManager ApportionmentManager { get; }
        IAccountSubsidyManager AccountSubsidyManager { get; }
        
        string Get();
        IEnumerable<string> UpdateBilling(UpdateBillingArgs args);
        UpdateClientBillingResult UpdateClientBilling(UpdateClientBillingCommand model);

        IReportManager Report { get; }
        IToolManager Tool { get; }
        IRoomManager Room { get; }
        IStoreManager Store { get; }
        IMiscManager Misc { get; }
    }
}
