using LNF.Billing;
using LNF.Billing.Process;
using LNF.CommonTools;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Billing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace LNF.Impl.Billing
{
    public class ProcessRepository : BillingRepository, IProcessRepository
    {
        public ProcessRepository(ISessionManager mgr) : base(mgr) { }

        public DataCleanResult DataClean(DataCleanCommand command)
        {
            if (command.StartDate == default)
                throw new Exception("Missing parameter: StartDate");

            if (command.EndDate == default)
                throw new Exception("Missing parameter: EndDate");

            if (command.EndDate <= command.StartDate)
                throw new Exception("StartDate must come before EndDate.");

            using (var conn = NewConnection())
            {
                conn.Open();

                DataCleanResult result = new DataCleanResult();

                if ((command.BillingCategory & BillingCategory.Tool) > 0)
                    result.WriteToolDataCleanProcessResult = new WriteToolDataCleanProcess(new WriteToolDataCleanConfig { Connection = conn, Context = "ProcessRepository.DataClean", StartDate = command.StartDate, EndDate = command.EndDate, ClientID = command.ClientID }).Start();

                if ((command.BillingCategory & BillingCategory.Room) > 0)
                    result.WriteRoomDataCleanProcessResult = new WriteRoomDataCleanProcess(new WriteRoomDataCleanConfig { Connection = conn, Context = "ProcessRepository.DataClean", StartDate = command.StartDate, EndDate = command.EndDate, ClientID = command.ClientID }).Start();

                if ((command.BillingCategory & BillingCategory.Store) > 0)
                {
                    using (var uow = NewUnitOfWork())
                        result.WriteStoreDataCleanProcessResult = new WriteStoreDataCleanProcess(new WriteStoreDataCleanConfig { Connection = conn, Context = "ProcessRepository.DataClean", StartDate = command.StartDate, EndDate = command.EndDate, ClientID = command.ClientID }).Start();
                }

                result.SetEndedAt();

                conn.Close();

                return result;
            }
        }

        public DataResult Data(DataCommand command)
        {
            if (command.Period == default)
                throw new Exception("Missing parameter: Period");

            if (command.Period.Day != 1)
                throw new Exception("Period must be the first day of the month.");

            using (var conn = NewConnection())
            {
                conn.Open();

                DataResult result = new DataResult();

                if ((command.BillingCategory & BillingCategory.Tool) > 0)
                    result.WriteToolDataProcessResult = new WriteToolDataProcess(new WriteToolDataConfig { Connection = conn, Context = "ProcessRepository.Data", Period = command.Period, ClientID = command.ClientID, ResourceID = command.Record }).Start();

                if ((command.BillingCategory & BillingCategory.Room) > 0)
                    result.WriteRoomDataProcessResult = new WriteRoomDataProcess(new WriteRoomDataConfig { Connection = conn, Context = "ProcessRepository.Data", Period = command.Period, ClientID = command.ClientID, RoomID = command.Record }).Start();

                if ((command.BillingCategory & BillingCategory.Store) > 0)
                    result.WriteStoreDataProcessResult = new WriteStoreDataProcess(new WriteStoreDataConfig { Connection = conn, Context = "ProcessRepository.Data", Period = command.Period, ClientID = command.ClientID, ItemID = command.Record }).Start();

                result.SetEndedAt();

                conn.Close();

                return result;
            }
        }

        public Step1Result Step1(Step1Command command)
        {
            if (command.Period == default)
                throw new Exception("Missing parameter: Period");

            if (command.Period.Day != 1)
                throw new Exception("Period must be the first day of the month.");

            using (var conn = NewConnection())
            {
                conn.Open();

                DateTime now = DateTime.Now;

                var step1 = new BillingDataProcessStep1(new Step1Config { Connection = conn, Context = "ProcessRepository.Step1", Period = command.Period, Now = now, ClientID = command.ClientID, IsTemp = command.IsTemp });

                Step1Result result = new Step1Result();

                if ((command.BillingCategory & BillingCategory.Tool) > 0)
                    result.PopulateToolBillingProcessResult = step1.PopulateToolBilling();

                if ((command.BillingCategory & BillingCategory.Room) > 0)
                    result.PopulateRoomBillingProcessResult = step1.PopulateRoomBilling();

                if ((command.BillingCategory & BillingCategory.Store) > 0)
                    result.PopulateStoreBillingProcessResult = step1.PopulateStoreBilling();

                result.SetEndedAt();

                conn.Close();

                return result;
            }
        }

        public PopulateSubsidyBillingResult Step4(Step4Command command)
        {
            if (command.Period == default)
                throw new Exception("Missing parameter: Period");

            if (command.Period.Day != 1 || command.Period.Hour != 0 || command.Period.Minute != 0 || command.Period.Second != 0)
                throw new Exception("Period must be midnight on the first day of the month.");

            using (var conn = NewConnection())
            {
                conn.Open();

                var step4 = new BillingDataProcessStep4Subsidy(new Step4Config { Connection = conn, Context = "ProcessRepository.Step4", Period = command.Period, ClientID = command.ClientID });

                PopulateSubsidyBillingResult result;

                switch (command.Command)
                {
                    case "subsidy":
                        result = step4.PopulateSubsidyBilling();
                        break;
                    case "distribution":
                        result = new PopulateSubsidyBillingResult { Command = "distribution" };
                        step4.DistributeSubsidyMoneyEvenly();
                        result.SetEndedAt();
                        break;
                    default:
                        throw new Exception($"Unknown command: {command.Command}");
                }

                conn.Close();

                return result;
            }
        }

        //Update all tables
        public UpdateResult Update(UpdateCommand command)
        {
            if (command.Period == default)
                throw new Exception("Missing parameter: Period");

            if (command.Period.Day != 1)
                throw new Exception("Period must be the first day of the month.");

            using (var conn = NewConnection())
            {
                conn.Open();

                var result = new UpdateResult
                {
                    BillingTypes = command.BillingTypes,
                    UpdateTypes = command.UpdateTypes,
                    Period = command.Period,
                    ClientID = command.ClientID
                };

                var types = command.BillingTypes.ToString().Split(',').Select(x => x.Trim()).ToArray();

                // types should {"Tool", "Room", "Store"} for example

                foreach (string t in types)
                {
                    //Clean data always first, then final data
                    string[] dTypes = null;
                    switch (command.UpdateTypes)
                    {
                        case UpdateDataType.DataClean:
                            dTypes = new string[] { "Clean" };
                            break;
                        case UpdateDataType.Data:
                            dTypes = new string[] { "" };
                            break;
                        case UpdateDataType.DataClean | UpdateDataType.Data:
                            dTypes = new string[] { "Clean", "" };
                            break;
                    }

                    foreach (string dType in dTypes)
                    {
                        DateTime sd;
                        DateTime ed;

                        string dataType = $"{t}Data{dType}"; //e.g. StoreData, StoreDataClean OR RoomData, RoomDataClean
                        string funcName = $"Write{dataType}"; //e.g. WriteStoreData, WriteStoreDataClean OR WriteRoomData, WriteRoomDataClean

                        DateTime fom = command.Period.FirstOfMonth();

                        //always do clean first (called from within the select)
                        if (dType == "Clean")
                        {
                            //This stored procedure will delete all records that are of today (i.e. 00:00:00 of current day)
                            //and will return the max date after the deletion (so the return date should be yesterday)

                            string procName = $"dbo.{dataType}_Select";
                            DateTime lastUpdate = Session.Command().Param("Action", "LastUpdate").Param("ClientID", command.ClientID > 0, command.ClientID).ExecuteScalar<DateTime>(procName).Value.Date;

                            /*
                             * For tool: lastUpdate is the last BeginDateTime in ToolDataClean after deleteing all records in this way:
                             * 
                             * SET @eDate = CONVERT(datetime, CONVERT(nvarchar(10), GETDATE(), 120)) -- 00:00:00 of current day
                             * 
                             * DELETE dbo.ToolDataClean -- allows multiple queries in the same day
                             * WHERE BeginDateTime >= @eDate OR ActualBeginDateTime >= @eDate 
                            */

                            if (lastUpdate == default)
                                throw new Exception($"Cannot get lastUpdate from {procName}");

                            // sd is which ever is earlier: previousDay or lastUpdate
                            sd = lastUpdate < fom ? lastUpdate : fom;
                            ed = fom.AddMonths(1).Date;
                        }
                        else
                        {
                            // For Data always import the whole period.
                            sd = fom;
                            ed = sd.AddMonths(1);
                        }

                        // At this point ed is always 1 month after period (the arg passed to this method). For Data,
                        // sd will always be period. For DataClean sd will either be period or the last date currently
                        // found in the Clean table, whichever is earliest.

                        // No looping is necessary because for Clean the methods take a start and end date, and for Data
                        // we always use period (the arg passed to this method). Assuming DataClean works, there will be
                        // something to import into Data.

                        try
                        {
                            switch (funcName)
                            {
                                case "WriteRoomDataClean":
                                    result.WriteRoomDataCleanProcessResult = new WriteRoomDataCleanProcess(new WriteRoomDataCleanConfig { Connection = conn, Context = "ProcessRepository.Update", StartDate = sd, EndDate = ed, ClientID = command.ClientID }).Start();
                                    break;
                                case "WriteRoomData":
                                    result.WriteRoomDataProcessResult = new WriteRoomDataProcess(new WriteRoomDataConfig { Connection = conn, Context = "ProcessRepository.Update", Period = sd, ClientID = command.ClientID, RoomID = 0 }).Start();
                                    break;
                                case "WriteToolDataClean":
                                    result.WriteToolDataCleanProcessResult = new WriteToolDataCleanProcess(new WriteToolDataCleanConfig { Connection = conn, Context = "ProcessRepository.Update", StartDate = sd, EndDate = ed, ClientID = command.ClientID }).Start();
                                    break;
                                case "WriteToolData":
                                    result.WriteToolDataProcessResult = new WriteToolDataProcess(new WriteToolDataConfig { Connection = conn, Context = "ProcessRepository.Update", Period = sd, ClientID = command.ClientID, ResourceID = 0 }).Start();
                                    break;
                                case "WriteStoreDataClean":
                                    result.WriteStoreDataCleanProcessResult = new WriteStoreDataCleanProcess(new WriteStoreDataCleanConfig { Connection = conn, Context = "ProcessRepository.Update", StartDate = sd, EndDate = ed, ClientID = command.ClientID }).Start();
                                    break;
                                case "WriteStoreData":
                                    result.WriteStoreDataProcessResult = new WriteStoreDataProcess(new WriteStoreDataConfig { Connection = conn, Context = "ProcessRepository.Update", Period = sd, ClientID = command.ClientID, ItemID = 0 }).Start();
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            // [2010-02-03] Test code to track who calls this function
                            // [2016-09-28 jg] Only call when there's an error
                            string body = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] function = {funcName}, ClientID = {command.ClientID}, sd = {sd}, ed = {ed}"
                                + Environment.NewLine + Environment.NewLine + ex.ToString();

                            result.Error = body;

                            string subj = $"Call from LNF.CommonTools.WriteData.UpdateTables [{t + dType}] [{DateTime.Now:yyyy-MM-dd HH:mm:ss}]";

                            SendEmail.SendDeveloperEmail("LNF.CommonTools.WriteData.UpdateTables", subj, body);
                        }
                    }
                }

                result.SetEndedAt();

                conn.Close();

                return result;
            }
        }

        public FinalizeResult Finalize(FinalizeCommand command)
        {
            if (command.Period == default)
                throw new Exception("Missing parameter: Period");

            if (command.Period.Day != 1)
                throw new Exception("Period must be the first day of the month.");

            using (var conn = NewConnection())
            {
                conn.Open();

                var now = DateTime.Now;

                var result = new FinalizeResult { Period = command.Period, };
                
                var step1 = new BillingDataProcessStep1(new Step1Config { Connection = conn, Context = "ProcessRepository.Finalize", Period = command.Period, Now = now, ClientID = 0, IsTemp = false });
                var step4 = new BillingDataProcessStep4Subsidy(new Step4Config { Connection = conn, Context = "ProcessRepository.Finalize", Period = command.Period, ClientID = 0 });

                result.WriteToolDataProcessResult = new WriteToolDataProcess(new WriteToolDataConfig { Connection = conn, Context = "ProcessRepository.Finalize", Period = command.Period, ClientID = 0, ResourceID = 0 }).Start();
                result.WriteRoomDataProcessResult = new WriteRoomDataProcess(new WriteRoomDataConfig { Connection = conn, Context = "ProcessRepository.Finalize", Period = command.Period, ClientID = 0, RoomID = 0 }).Start();
                result.WriteStoreDataProcessResult = new WriteStoreDataProcess(new WriteStoreDataConfig { Connection = conn, Context = "ProcessRepository.Finalize", Period = command.Period, ClientID = 0, ItemID = 0 }).Start();
                result.PopulateToolBillingProcessResult = step1.PopulateToolBilling();
                result.PopulateRoomBillingProcessResult = step1.PopulateRoomBilling();
                result.PopulateStoreBillingProcessResult = step1.PopulateStoreBilling();
                result.PopulateSubsidyBillingProcessResult = step4.PopulateSubsidyBilling();

                result.SetEndedAt();

                conn.Close();

                return result;
            }
        }

        public bool RemoteProcessingUpdate(RemoteProcessingUpdate command)
        {
            if (command.Period == default)
                throw new Exception("Missing parameter: Period");

            if (command.Period.Day != 1)
                throw new Exception("Period must be the first day of the month.");

            var bt = Session.GetBillingType(command.ClientID, command.AccountID, command.Period);
            if (bt != null)
            {
                Session.UpdateToolBillingType(command.ClientID, command.AccountID, bt.BillingTypeID, command.Period);
                Session.UpdateRoomBillingType(command.ClientID, command.AccountID, bt.BillingTypeID, command.Period);
                return true;
            }
            else
            {
                return false;
            }
        }

        public int DeleteData(BillingCategory billingCategory, DateTime period, int clientId = 0, int record = 0)
        {
            if (period == default)
                throw new Exception("Missing parameter: Period");

            if (period.Day != 1)
                throw new Exception("Period must be the first day of the month.");

            string recordParam;

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

            int result = Session.Command(CommandType.Text)
                .Param("Period", period)
                .Param("ClientID", clientId > 0, clientId, DBNull.Value)
                .Param("Record", record > 0, record, DBNull.Value)
                .ExecuteNonQuery(sql).Value;

            return result;
        }

        public IEnumerable<string> UpdateBilling(UpdateBillingArgs args)
        {
            // updates all billing
            using (var conn = NewConnection())
            {
                conn.Open();

                DateTime startTime = DateTime.Now;

                var result = new List<string>
                {
                    $"Started at {startTime:yyyy-MM-dd HH:mm:ss}"
                };

                Stopwatch sw;
                DateTime sd, ed;

                foreach (var p in args.Periods)
                {
                    if (p == default)
                        throw new Exception("Missing parameter: Period");

                    if (p.Day != 1)
                        throw new Exception("Period must be the first day of the month.");

                    var temp = Utility.IsCurrentPeriod(p);

                    sd = p;
                    ed = p.AddMonths(1);

                    var populateSubsidy = false;

                    sw = new Stopwatch();

                    DateTime now = DateTime.Now;

                    var step1 = new BillingDataProcessStep1(new Step1Config { Connection = conn, Context = "ProcessRepository.UpdateBilling", Period = p, Now = now, ClientID = args.ClientID, IsTemp = temp });

                    if (args.BillingCategory.HasFlag(BillingCategory.Tool))
                    {
                        var toolDataClean = new WriteToolDataCleanProcess(new WriteToolDataCleanConfig { Connection = conn, Context = "ProcessRepository.UpdateBilling", StartDate = sd, EndDate = ed, ClientID = args.ClientID });
                        var toolData = new WriteToolDataProcess(new WriteToolDataConfig { Connection = conn, Context = "ProcessRepository.UpdateBilling", Period = p, ClientID = args.ClientID, ResourceID = 0 });

                        sw.Restart();
                        toolDataClean.Start();
                        result.Add(string.Format("Completed ToolDataClean in {0}", sw.Elapsed));

                        sw.Restart();
                        toolData.Start();
                        result.Add(string.Format("Completed ToolData in {0}", sw.Elapsed));

                        sw.Restart();
                        step1.PopulateToolBilling();
                        result.Add(string.Format("Completed ToolBilling in {0}", sw.Elapsed));

                        populateSubsidy = true;
                    }

                    if (args.BillingCategory.HasFlag(BillingCategory.Room))
                    {
                        var roomDataClean = new WriteRoomDataCleanProcess(new WriteRoomDataCleanConfig { Connection = conn, Context = "ProcessRepository.UpdateBilling", StartDate = sd, EndDate = ed, ClientID = args.ClientID, RoomID = 0 });
                        var roomData = new WriteRoomDataProcess(new WriteRoomDataConfig { Connection = conn, Context = "ProcessRepository.UpdateBilling", Period = p, ClientID = args.ClientID, RoomID = 0 });

                        sw.Restart();
                        roomDataClean.Start();
                        result.Add(string.Format("Completed RoomDataClean in {0}", sw.Elapsed));

                        sw.Restart();
                        roomData.Start();
                        result.Add(string.Format("Completed RoomData in {0}", sw.Elapsed));

                        sw.Restart();
                        step1.PopulateRoomBilling();
                        result.Add(string.Format("Completed RoomBilling in {0}", sw.Elapsed));

                        populateSubsidy = true;
                    }

                    if (args.BillingCategory.HasFlag(BillingCategory.Store))
                    {
                        var storeDataClean = new WriteStoreDataCleanProcess(new WriteStoreDataCleanConfig { Connection = conn, Context = "ProcessRepository.UpdateBilling", StartDate = sd, EndDate = ed, ClientID = args.ClientID });
                        var storeData = new WriteStoreDataProcess(new WriteStoreDataConfig { Connection = conn, Context = "ProcessRepository.UpdateBilling", Period = p, ClientID = args.ClientID, ItemID = 0 });

                        sw.Restart();
                        storeDataClean.Start();
                        result.Add(string.Format("Completed StoreDataClean in {0}", sw.Elapsed));

                        sw.Restart();
                        storeData.Start();
                        result.Add(string.Format("Completed StoreData in {0}", sw.Elapsed));

                        sw.Restart();
                        step1.PopulateStoreBilling();
                        result.Add(string.Format("Completed StoreBilling in {0}", sw.Elapsed));
                    }

                    if (!temp && populateSubsidy)
                    {
                        sw.Restart();
                        var step4 = new BillingDataProcessStep4Subsidy(new Step4Config { Connection = conn, Context = "ProcessRepository.UpdateBilling", Period = p, ClientID = args.ClientID });

                        step4.PopulateSubsidyBilling();

                        result.Add(string.Format("Completed SubsidyBilling in {0}", sw.Elapsed));
                    }

                    sw.Stop();
                }

                result.Add(string.Format("Completed at {0:yyyy-MM-dd HH:mm:ss}, time taken: {1}", DateTime.Now, DateTime.Now - startTime));

                conn.Close();

                return result;
            }
        }

        public UpdateClientBillingResult UpdateClientBilling(UpdateClientBillingCommand command)
        {
            if (command.Period == default)
                throw new Exception("Missing parameter: Period");

            if (command.Period.Day != 1)
                throw new Exception("Period must be the first day of the month.");

            using (var conn = NewConnection())
            {
                conn.Open();

                DateTime now = DateTime.Now;

                DateTime sd = command.Period;
                DateTime ed = command.Period.AddMonths(1);

                var toolDataClean = new WriteToolDataCleanProcess(new WriteToolDataCleanConfig { Connection = conn, Context = "ProcessRepository.UpdateClientBilling", StartDate = sd, EndDate = ed, ClientID = command.ClientID });
                var toolData = new WriteToolDataProcess(new WriteToolDataConfig { Connection = conn, Context = "ProcessRepository.UpdateClientBilling", Period = sd, ClientID = command.ClientID, ResourceID = 0 });

                var roomDataClean = new WriteRoomDataCleanProcess(new WriteRoomDataCleanConfig { Connection = conn, Context = "ProcessRepository.UpdateClientBilling", StartDate = sd, EndDate = ed, ClientID = command.ClientID });
                var roomData = new WriteRoomDataProcess(new WriteRoomDataConfig { Connection = conn, Context = "ProcessRepository.UpdateClientBilling", Period = sd, ClientID = command.ClientID, RoomID = 0 });

                var pr1 = toolDataClean.Start();
                var pr2 = roomDataClean.Start();

                var pr3 = toolData.Start();
                var pr4 = roomData.Start();

                bool temp = DateTime.Now.FirstOfMonth() == command.Period;

                var step1 = new BillingDataProcessStep1(new Step1Config { Connection = conn, Context = "ProcessRepository.UpdateClientBilling", Period = command.Period, Now = now, ClientID = command.ClientID, IsTemp = temp });

                var pr5 = step1.PopulateToolBilling();
                var pr6 = step1.PopulateRoomBilling();

                PopulateSubsidyBillingResult pr7 = null;

                if (!temp)
                {
                    var step4 = new BillingDataProcessStep4Subsidy(new Step4Config { Connection = conn, Context = "ProcessRepository.UpdateClientBilling", Period = command.Period, ClientID = command.ClientID });
                    pr7 = step4.PopulateSubsidyBilling();
                }

                var result = new UpdateClientBillingResult
                {
                    WriteToolDataCleanProcessResult = pr1,
                    WriteRoomDataCleanProcessResult = pr2,
                    WriteToolDataProcessResult = pr3,
                    WriteRoomDataProcessResult = pr4,
                    PopulateToolBillingProcessResult = pr5,
                    PopulateRoomBillingProcessResult = pr6,
                    PopulateSubsidyBillingProcessResult = pr7
                };

                conn.Close();

                return result;
            }
        }
    }
}
