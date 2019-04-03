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

        IReportClient Report { get; }
        IToolClient Tool { get; }
        IRoomClient Room { get; }
        IStoreClient Store { get; }
        IMiscClient Misc { get; }
    }
}
