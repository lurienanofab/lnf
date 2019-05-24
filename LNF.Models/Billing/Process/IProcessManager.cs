using System;
using System.Collections.Generic;

namespace LNF.Models.Billing.Process
{
    public interface IProcessManager
    {
        BillingProcessDataCleanResult BillingProcessDataClean(BillingProcessDataCleanCommand command);
        BillingProcessDataResult BillingProcessData(BillingProcessDataCommand command);
        BillingProcessStep1Result BillingProcessStep1(BillingProcessStep1Command command);
        PopulateSubsidyBillingProcessResult BillingProcessStep4(BillingProcessStep4Command command);
        DataFinalizeProcessResult BillingProcessDataFinalize(BillingProcessDataFinalizeCommand command);
        DataUpdateProcessResult BillingProcessDataUpdate(BillingProcessDataUpdateCommand command);
        bool RemoteProcessingUpdate(RemoteProcessingUpdate command);
        int DeleteData(BillingCategory billingCategory, DateTime period, int clientId = 0, int record = 0);
        IEnumerable<string> UpdateBilling(UpdateBillingArgs args);
        UpdateClientBillingResult UpdateClientBilling(UpdateClientBillingCommand model);
    }
}