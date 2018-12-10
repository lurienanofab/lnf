using System.Collections.Generic;

namespace LNF.Models.Billing
{
    public interface IDefaultClient
    {
        string Get();
        IEnumerable<string> UpdateBilling(UpdateBillingArgs args);
        UpdateClientBillingResult UpdateClientBilling(UpdateClientBillingCommand model);
    }
}