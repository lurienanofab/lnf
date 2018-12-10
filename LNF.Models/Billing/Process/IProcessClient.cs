using System;

namespace LNF.Models.Billing.Process
{
    public interface IProcessClient
    {
        WriteToolDataProcessResult BillingProcessToolData(DateTime period, int clientId = 0, int resourceId = 0);
        WriteRoomDataProcessResult BillingProcessRoomData(DateTime period, int clientId = 0, int roomId = 0);
        WriteStoreDataProcessResult BillingProcessStoreData(DateTime period, int clientId = 0, int itemId = 0);
        WriteToolDataCleanProcessResult BillingProcessToolDataClean(DateTime sd, DateTime ed, int clientId = 0);
        WriteRoomDataCleanProcessResult BillingProcessRoomDataClean(DateTime sd, DateTime ed, int clientId = 0);
        WriteStoreDataCleanProcessResult BillingProcessStoreDataClean(DateTime sd, DateTime ed, int clientId = 0);
        DataFinalizeProcessResult BillingProcessDataFinalize(DateTime period);
        DataUpdateProcessResult BillingProcessDataUpdate(BillingCategory billingCategory);
        PopulateToolBillingProcessResult BillingProcessToolStep1(DateTime period, bool delete, bool temp, int clientId = 0, int resourceId = 0);
        PopulateRoomBillingProcessResult BillingProcessRoomStep1(DateTime period, bool delete, bool temp, int clientId = 0, int roomId = 0);
        PopulateStoreBillingProcessResult BillingProcessStoreStep1(DateTime period, bool delete, bool temp, int clientId = 0, int itemId = 0);
        PopulateSubsidyBillingProcessResult BillingProcessStep4(string command, DateTime period, int clientId = 0);
        int DeleteData(BillingCategory billingCategory, DateTime period, int clientId = 0, int record = 0);
        bool RemoteProcessingUpdate(DateTime period, int clientId, int accountId);
    }
}