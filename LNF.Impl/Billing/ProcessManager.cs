using LNF.CommonTools;
using LNF.Models.Billing;
using LNF.Models.Billing.Process;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

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

            var step4 = new BillingDataProcessStep4Subsidy(command);

            switch (command.Command)
            {
                case "subsidy":
                    result = step4.PopulateSubsidyBilling();
                    break;
                case "distribution":
                    result = new PopulateSubsidyBillingProcessResult { Command = "distribution" };
                    step4.DistributeSubsidyMoneyEvenly();
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

            var bt = Provider.Billing.BillingType.GetBillingType(client.ClientID, acct.AccountID, command.Period);
            Provider.Billing.Tool.UpdateBillingType(client.ClientID, acct.AccountID, bt.BillingTypeID, command.Period);
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

        public IEnumerable<string> UpdateBilling(UpdateBillingArgs args)
        {
            // updates all billing

            DateTime startTime = DateTime.Now;

            var result = new List<string>
            {
                $"Started at {startTime:yyyy-MM-dd HH:mm:ss}"
            };

            Stopwatch sw;

            DateTime sd = args.StartDate;

            while (sd < args.EndDate)
            {
                DateTime ed = sd.AddMonths(1);

                var isTemp = (sd == DateTime.Now.FirstOfMonth());

                var populateSubsidy = false;

                sw = new Stopwatch();

                var step1 = new BillingDataProcessStep1(DateTime.Now, ServiceProvider.Current);

                if (args.BillingCategory.HasFlag(BillingCategory.Tool))
                {
                    var toolDataClean = new WriteToolDataCleanProcess(sd, ed, args.ClientID);
                    var toolData = new WriteToolDataProcess(sd, args.ClientID, args.ResourceID);

                    sw.Restart();
                    toolDataClean.Start();
                    result.Add(string.Format("Completed ToolDataClean in {0}", sw.Elapsed));

                    sw.Restart();
                    toolData.Start();
                    result.Add(string.Format("Completed ToolData in {0}", sw.Elapsed));

                    sw.Restart();
                    step1.PopulateToolBilling(sd, args.ClientID, isTemp);
                    result.Add(string.Format("Completed ToolBilling in {0}", sw.Elapsed));

                    populateSubsidy = true;
                }

                if (args.BillingCategory.HasFlag(BillingCategory.Room))
                {
                    var roomDataClean = new WriteRoomDataCleanProcess(sd, ed, args.ClientID);
                    var roomData = new WriteRoomDataProcess(sd, args.ClientID, args.RoomID);

                    sw.Restart();
                    roomDataClean.Start();
                    result.Add(string.Format("Completed RoomDataClean in {0}", sw.Elapsed));

                    sw.Restart();
                    roomData.Start();
                    result.Add(string.Format("Completed RoomData in {0}", sw.Elapsed));

                    sw.Restart();
                    step1.PopulateRoomBilling(sd, args.ClientID, isTemp);
                    result.Add(string.Format("Completed RoomBilling in {0}", sw.Elapsed));

                    populateSubsidy = true;
                }

                if (args.BillingCategory.HasFlag(BillingCategory.Store))
                {
                    var storeDataClean = new WriteStoreDataCleanProcess(sd, ed, args.ClientID);
                    var storeData = new WriteStoreDataProcess(sd, args.ClientID, args.ItemID);

                    sw.Restart();
                    storeDataClean.Start();
                    result.Add(string.Format("Completed StoreDataClean in {0}", sw.Elapsed));

                    sw.Restart();
                    storeData.Start();
                    result.Add(string.Format("Completed StoreData in {0}", sw.Elapsed));

                    sw.Restart();
                    step1.PopulateStoreBilling(sd, isTemp);
                    result.Add(string.Format("Completed StoreBilling in {0}", sw.Elapsed));
                }

                if (!isTemp && populateSubsidy)
                {
                    sw.Restart();
                    var step4 = new BillingDataProcessStep4Subsidy(new BillingProcessStep4Command
                    {
                        Command = "subsidy",
                        Period = sd,
                        ClientID = args.ClientID
                    });

                    step4.PopulateSubsidyBilling();

                    result.Add(string.Format("Completed SubsidyBilling in {0}", sw.Elapsed));
                }

                sd = sd.AddMonths(1);

                sw.Stop();
            }

            result.Add(string.Format("Completed at {0:yyyy-MM-dd HH:mm:ss}, time taken: {1}", DateTime.Now, DateTime.Now - startTime));

            return result;
        }

        public UpdateClientBillingResult UpdateClientBilling(UpdateClientBillingCommand model)
        {
            DateTime now = DateTime.Now;

            DateTime sd = model.Period;
            DateTime ed = model.Period.AddMonths(1);

            var toolDataClean = new WriteToolDataCleanProcess(sd, ed, model.ClientID);
            var toolData = new WriteToolDataProcess(sd, model.ClientID, 0);

            var roomDataClean = new WriteRoomDataCleanProcess(sd, ed, model.ClientID);
            var roomData = new WriteRoomDataProcess(sd, model.ClientID, 0);

            var pr1 = toolDataClean.Start();
            var pr2 = roomDataClean.Start();

            var pr3 = toolData.Start();
            var pr4 = roomData.Start();

            bool isTemp = DateTime.Now.FirstOfMonth() == model.Period;

            var step1 = new BillingDataProcessStep1(DateTime.Now, ServiceProvider.Current);

            var pr5 = step1.PopulateToolBilling(model.Period, model.ClientID, isTemp);
            var pr6 = step1.PopulateRoomBilling(model.Period, model.ClientID, isTemp);

            PopulateSubsidyBillingProcessResult pr7 = null;

            if (!isTemp)
            {
                var step4 = new BillingDataProcessStep4Subsidy(new BillingProcessStep4Command
                {
                    Command = "subsidy",
                    Period = model.Period,
                    ClientID = model.ClientID
                });

                pr7 = step4.PopulateSubsidyBilling();
            }

            return new UpdateClientBillingResult
            {
                WriteToolDataCleanProcessResult = pr1,
                WriteRoomDataCleanProcessResult = pr2,
                WriteToolDataProcessResult = pr3,
                WriteRoomDataProcessResult = pr4,
                PopulateToolBillingProcessResult = pr5,
                PopulateRoomBillingProcessResult = pr6,
                PopulateSubsidyBillingProcessResult = pr7
            };
        }
    }
}
