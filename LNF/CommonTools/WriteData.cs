using LNF.Models;
using LNF.Models.Billing;
using LNF.Models.Billing.Process;
using LNF.Repository;
using System;
using System.Linq;

namespace LNF.CommonTools
{
    public class WriteData
    {
        //Update all tables
        public UpdateTablesResult UpdateTables(BillingCategory billingTypes, UpdateDataType updateTypes, DateTime period, int clientId)
        {
            var result = new UpdateTablesResult
            {
                BillingTypes = billingTypes,
                UpdateTypes = updateTypes,
                Period = period,
                ClientID = clientId
            };

            var types = billingTypes.ToString().Split(',').Select(x => x.Trim()).ToArray();

            // types should {"Tool", "Room", "Store"} for example

            foreach (string t in types)
            {
                //Clean data always first, then final data
                string[] dTypes = null;
                switch (updateTypes)
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

                    DateTime fom = period.FirstOfMonth();

                    //always do clean first (called from within the select)
                    if (dType == "Clean")
                    {
                        //This stored procedure will delete all records that are of today (i.e. 00:00:00 of current day)
                        //and will return the max date after the deletion (so the return date should be yesterday)

                        string procName = $"dbo.{dataType}_Select";
                        DateTime lastUpdate = DA.Command().Param("Action", "LastUpdate").Param("ClientID", clientId > 0, clientId).ExecuteScalar<DateTime>(procName).Value.Date;

                        /*
                         * For tool: lastUpdate is the last BeginDateTime in ToolDataClean after deleteing all records in this way:
                         * 
                         * SET @eDate = CONVERT(datetime, CONVERT(nvarchar(10), GETDATE(), 120)) -- 00:00:00 of current day
                         * 
                         * DELETE dbo.ToolDataClean -- allows multiple queries in the same day
                         * WHERE BeginDateTime >= @eDate OR ActualBeginDateTime >= @eDate 
                        */

                        if (lastUpdate == default(DateTime))
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
                                result.WriteRoomDataCleanProcessResult = new WriteRoomDataCleanProcess(sd, ed, clientId).Start();
                                break;
                            case "WriteRoomData":
                                result.WriteRoomDataProcessResult = new WriteRoomDataProcess(sd, clientId, 0).Start();
                                break;
                            case "WriteToolDataClean":
                                result.WriteToolDataCleanProcessResult = new WriteToolDataCleanProcess(sd, ed, clientId).Start();
                                break;
                            case "WriteToolData":
                                result.WriteToolDataProcessResult = new WriteToolDataProcess(sd, clientId, 0).Start();
                                break;
                            case "WriteStoreDataClean":
                                result.WriteStoreDataCleanProcessResult = new WriteStoreDataCleanProcess(sd, ed, clientId).Start();
                                break;
                            case "WriteStoreData":
                                result.WriteStoreDataProcessResult = new WriteStoreDataProcess(sd, clientId, 0).Start();
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        // [2010-02-03] Test code to track who calls this function
                        // [2016-09-28 jg] Only call when there's an error
                        string body = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] function = {funcName}, ClientID = {clientId}, sd = {sd}, ed = {ed}"
                            + Environment.NewLine + Environment.NewLine + ex.ToString();

                        result.Error = body;

                        string subj = $"Call from LNF.CommonTools.WriteData.UpdateTables [{t + dType}] [{DateTime.Now:yyyy-MM-dd HH:mm:ss}]";

                        SendEmail.SendDeveloperEmail("LNF.CommonTools.WriteData.UpdateTables", subj, body);
                    }
                }
            }

            return result;
        }
    }
}