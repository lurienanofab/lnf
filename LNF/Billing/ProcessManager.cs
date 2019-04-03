using LNF.CommonTools;
using LNF.Models.Billing;
using LNF.Models.Billing.Process;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Data;

namespace LNF.Billing
{
    public class ProcessManager : ManagerBase, IProcessManager
    {
        public ProcessManager(IProvider provider) : base(provider) { }

        public BillingProcessDataCleanResult BillingProcessDataClean(BillingProcessDataCleanCommand command)
        {
            if (command.StartDate == default(DateTime))
                throw new Exception("Missing parameter: StartDate");

            if (command.EndDate == default(DateTime))
                throw new Exception("Missing parameter: EndDate");

            if (command.EndDate <= command.StartDate)
                throw new Exception("StartDate must come before EndDate.");

            BillingProcessDataCleanResult result = new BillingProcessDataCleanResult();

            if ((command.BillingCategory & BillingCategory.Tool) > 0)
                result.WriteToolDataCleanProcessResult = new WriteToolDataCleanProcess(command.StartDate, command.EndDate, command.ClientID).Start();

            if ((command.BillingCategory & BillingCategory.Room) > 0)
                result.WriteRoomDataCleanProcessResult = new WriteRoomDataCleanProcess(command.StartDate, command.EndDate, command.ClientID).Start();

            if ((command.BillingCategory & BillingCategory.Store) > 0)
                result.WriteStoreDataCleanProcessResult = new WriteStoreDataCleanProcess(command.StartDate, command.EndDate, command.ClientID).Start();

            return result;
        }

        public BillingProcessDataResult BillingProcessData(BillingProcessDataCommand command)
        {
            if (command.Period == default(DateTime))
                throw new Exception("Missing parameter: Period");

            BillingProcessDataResult result = new BillingProcessDataResult();

            if ((command.BillingCategory & BillingCategory.Tool) > 0)
                result.WriteToolDataProcessResult = new WriteToolDataProcess(command.Period, command.ClientID, command.Record).Start();

            if ((command.BillingCategory & BillingCategory.Room) > 0)
                result.WriteRoomDataProcessResult = new WriteRoomDataProcess(command.Period, command.ClientID, command.Record).Start();

            if ((command.BillingCategory & BillingCategory.Store) > 0)
                result.WriteStoreDataProcessResult = new WriteStoreDataProcess(command.Period, command.ClientID, command.Record).Start();

            return result;
        }

        public BillingProcessStep1Result BillingProcessStep1(BillingProcessStep1Command command)
        {
            if (command.Period == default(DateTime))
                throw new Exception("Missing parameter: Period");

            var step1 = new BillingDataProcessStep1(DateTime.Now, ServiceProvider.Current);

            BillingProcessStep1Result result = new BillingProcessStep1Result();

            if ((command.BillingCategory & BillingCategory.Tool) > 0)
                result.PopulateToolBillingProcessResult = step1.PopulateToolBilling(command.Period, command.ClientID, command.IsTemp);

            if ((command.BillingCategory & BillingCategory.Room) > 0)
                result.PopulateRoomBillingProcessResult = step1.PopulateRoomBilling(command.Period, command.ClientID, command.IsTemp);

            if ((command.BillingCategory & BillingCategory.Store) > 0)
                result.PopulateStoreBillingProcessResult = step1.PopulateStoreBilling(command.Period, command.IsTemp);

            return result;
        }

        public PopulateSubsidyBillingProcessResult BillingProcessStep4(BillingProcessStep4Command command)
        {
            if (command.Period == default(DateTime))
                throw new Exception("Missing parameter: Period");

            PopulateSubsidyBillingProcessResult result;

            switch (command.Command)
            {
                case "subsidy":
                    result = BillingDataProcessStep4Subsidy.PopulateSubsidyBilling(command.Period, command.ClientID);
                    break;
                case "distribution":
                    result = new PopulateSubsidyBillingProcessResult { Command = "distribution" };
                    BillingDataProcessStep4Subsidy.DistributeSubsidyMoneyEvenly(command.Period, command.ClientID);
                    break;
                default:
                    throw new Exception($"Unknown command: {command.Command}");
            }

            return result;
        }

        public DataFinalizeProcessResult BillingProcessDataFinalize(BillingProcessDataFinalizeCommand command)
        {
            return DataTableManager.Finalize(command.Period);
        }

        public DataUpdateProcessResult BillingProcessDataUpdate(BillingProcessDataUpdateCommand command)
        {
            return DataTableManager.Update(command.BillingCategory);
        }

        public bool RemoteProcessingUpdate(RemoteProcessingUpdate command)
        {
            var client = Session.Single<Client>(command.ClientID);

            if (client == null)
                return false;

            var acct = Session.Single<Account>(command.AccountID);

            if (acct == null)
                return false;

            var bt = Provider.BillingTypeManager.GetBillingType(client, acct, command.Period);
            Provider.ToolBillingManager.UpdateBillingType(client.ClientID, acct.AccountID, bt.BillingTypeID, command.Period);
            RoomBillingUtility.UpdateBillingType(client, acct, bt, command.Period);

            return true;
        }

        public int DeleteData(BillingCategory billingCategory, DateTime period, int clientId = 0, int record = 0)
        {
            string recordParam = string.Empty;

            switch (billingCategory)
            {
                case BillingCategory.Tool:
                    recordParam = "ResourceID";
                    break;
                case BillingCategory.Room:
                    recordParam = "RoomID";
                    break;
                case BillingCategory.Store:
                    recordParam = "ItemID";
                    break;
                default:
                    throw new NotImplementedException();
            }

            string sql = $"DELETE FROM {billingCategory}Data WHERE Period = @Period AND ClientID = ISNULL(@ClientID, ClientID) AND {recordParam} = ISNULL(@Record, {1})";

            int result = DA.Command(CommandType.Text)
                .Param("Period", period)
                .Param("ClientID", clientId > 0, clientId, DBNull.Value)
                .Param("Record", record > 0, record, DBNull.Value)
                .ExecuteNonQuery(sql).Value;

            return result;
        }
    }
}
