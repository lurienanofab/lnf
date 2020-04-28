using System;
using System.Collections.Generic;

namespace LNF.Billing.Process
{
    public interface IProcessRepository
    {
        DataCleanResult DataClean(DataCleanCommand command);
        DataResult Data(DataCommand command);
        Step1Result Step1(Step1Command command);
        PopulateSubsidyBillingResult Step4(Step4Command command);
        UpdateResult Update(UpdateCommand command);
        FinalizeResult Finalize(FinalizeCommand command);
        bool RemoteProcessingUpdate(RemoteProcessingUpdate command);
        int DeleteData(BillingCategory billingCategory, DateTime period, int clientId = 0, int record = 0);
        IEnumerable<string> UpdateBilling(UpdateBillingArgs args);
        UpdateClientBillingResult UpdateClientBilling(UpdateClientBillingCommand model);
    }
}