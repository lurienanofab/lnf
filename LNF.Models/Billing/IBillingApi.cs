using LNF.Models.Billing.Process;
using LNF.Models.Billing.Reports;
using System.Collections.Generic;

namespace LNF.Models.Billing
{
    public interface IBillingApi
    {
        string Get();
        IEnumerable<string> UpdateBilling(UpdateBillingArgs args);
        UpdateClientBillingResult UpdateClientBilling(UpdateClientBillingCommand model);

        IAccountSubsidyClient AccountSubsidy { get; }
        IProcessClient Process { get; }
        IReportClient Report { get; }
        IToolClient Tool { get; }
        IRoomClient Room { get; }
        IStoreClient Store { get; }
        IMiscClient Misc { get; }
    }
}
