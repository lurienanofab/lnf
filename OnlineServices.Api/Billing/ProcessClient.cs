using LNF.Models.Billing;
using LNF.Models.Billing.Process;
using System;

namespace OnlineServices.Api.Billing
{
    public class ProcessClient : ApiClient, IProcessClient
    {
        public DataFinalizeProcessResult BillingProcessDataFinalize(DateTime period)
        {
            var cmd = new BillingProcessDataFinalizeCommand { Period = period };
            return Post<DataFinalizeProcessResult>("webapi/billing/process/data/finalize", cmd);
        }

        public DataUpdateProcessResult BillingProcessDataUpdate(BillingCategory billingCategory)
        {
            var cmd = new BillingProcessDataUpdateCommand { BillingCategory = billingCategory };
            return Post<DataUpdateProcessResult>("webapi/billing/process/data/update", cmd);
        }

        public WriteRoomDataProcessResult BillingProcessRoomData(DateTime period, int clientId = 0, int roomId = 0)
        {
            var cmd = new BillingProcessDataCommand { BillingCategory = BillingCategory.Room, Period = period, ClientID = clientId, Record = roomId };
            return Post<WriteRoomDataProcessResult>("webapi/billing/process/data", cmd);
        }

        public WriteRoomDataCleanProcessResult BillingProcessRoomDataClean(DateTime sd, DateTime ed, int clientId = 0)
        {
            var cmd = new BillingProcessDataCleanCommand { BillingCategory = BillingCategory.Room, StartDate = sd, EndDate = ed, ClientID = clientId };
            return Post<WriteRoomDataCleanProcessResult>("webapi/billing/process/data/clean", cmd);
        }

        public PopulateRoomBillingProcessResult BillingProcessRoomStep1(DateTime period, bool delete, bool temp, int clientId = 0, int roomId = 0)
        {
            var cmd = new BillingProcessStep1Command { BillingCategory = BillingCategory.Room, Period = period, ClientID = clientId, Record = roomId, Delete = delete, IsTemp = temp };
            return Post<PopulateRoomBillingProcessResult>("webapi/billing/process/step1", cmd);
        }

        public PopulateSubsidyBillingProcessResult BillingProcessStep4(string command, DateTime period, int clientId = 0)
        {
            var cmd = new BillingProcessStep4Command { Command = command, Period = period, ClientID = clientId };
            return Post<PopulateSubsidyBillingProcessResult>("webapi/billing/process/step4", cmd);
        }

        public WriteStoreDataProcessResult BillingProcessStoreData(DateTime period, int clientId = 0, int itemId = 0)
        {
            var cmd = new BillingProcessDataCommand { BillingCategory = BillingCategory.Store, Period = period, ClientID = clientId, Record = itemId };
            return Post<WriteStoreDataProcessResult>("webapi/billing/process/data", cmd);
        }

        public WriteStoreDataCleanProcessResult BillingProcessStoreDataClean(DateTime sd, DateTime ed, int clientId = 0)
        {
            var cmd = new BillingProcessDataCleanCommand { BillingCategory = BillingCategory.Store, StartDate = sd, EndDate = ed, ClientID = clientId };
            return Post<WriteStoreDataCleanProcessResult>("webapi/billing/process/data/clean", cmd);
        }

        public PopulateStoreBillingProcessResult BillingProcessStoreStep1(DateTime period, bool delete, bool temp, int clientId = 0, int itemId = 0)
        {
            var cmd = new BillingProcessStep1Command { BillingCategory = BillingCategory.Store, Period = period, ClientID = clientId, Record = itemId, Delete = delete, IsTemp = temp };
            return Post<PopulateStoreBillingProcessResult>("webapi/billing/process/step1", cmd);
        }

        public WriteToolDataProcessResult BillingProcessToolData(DateTime period, int clientId = 0, int resourceId = 0)
        {
            var cmd = new BillingProcessDataCommand { BillingCategory = BillingCategory.Tool, Period = period, ClientID = clientId, Record = resourceId };
            return Post<WriteToolDataProcessResult>("webapi/billing/process/data", cmd);
        }

        public WriteToolDataCleanProcessResult BillingProcessToolDataClean(DateTime sd, DateTime ed, int clientId = 0)
        {
            var cmd = new BillingProcessDataCleanCommand { BillingCategory = BillingCategory.Tool, StartDate = sd, EndDate = ed, ClientID = clientId };
            return Post<WriteToolDataCleanProcessResult>("webapi/billing/process/data/clean", cmd);
        }

        public PopulateToolBillingProcessResult BillingProcessToolStep1(DateTime period, bool delete, bool temp, int clientId = 0, int resourceId = 0)
        {
            var cmd = new BillingProcessStep1Command { BillingCategory = BillingCategory.Tool, Period = period, ClientID = clientId, Record = resourceId, Delete = delete, IsTemp = temp };
            return Post<PopulateToolBillingProcessResult>("webapi/billing/process/step1", cmd);
        }

        public int DeleteData(BillingCategory billingCategory, DateTime period, int clientId = 0, int record = 0)
        {
            return Delete("webapi/billing/process/data/{billingCategory}", UrlSegments(new { billingCategory }) & QueryStrings(new { period, clientId, record }));
        }

        public bool RemoteProcessingUpdate(DateTime period, int clientId, int accountId)
        {
            var cmd = new RemoteProcessingUpdate { Period = period, ClientID = clientId, AccountID = accountId };
            return Post<bool>("webapi/billing/process/remote-processing-update", cmd);
        }
    }
}
