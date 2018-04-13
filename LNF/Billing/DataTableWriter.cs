using LNF.CommonTools;
using LNF.Logging;
using LNF.Models.Billing;
using LNF.Repository;
using System;
using System.Collections.Generic;
using System.Data;

namespace LNF.Billing
{
    public class DataTableWriter
    {
        private BillingCategory _BillingCategory;
        private int _ClientID;
        private int _RecordID;
        private DateTime _EndDate;
        private UpdateDataType _UpdateTypes;
        private bool _Delete;
        private bool _IsDailyImport;

        public BillingCategory BillingCategory { get { return _BillingCategory; } }
        public int ClientID { get { return _ClientID; } }
        public int RecordID { get { return _RecordID; } }
        public DateTime EndDate { get { return _EndDate; } }
        public UpdateDataType UpdateTypes { get { return _UpdateTypes; } }
        public bool Delete { get { return _Delete; } }
        public bool IsDailyImport { get { return _IsDailyImport; } }

        public DataTableWriter(BillingCategory billingCategory, int clientId, int recordId, DateTime eDate, UpdateDataType updateTypes, bool delete, bool isDailyImport)
        {
            _BillingCategory = billingCategory;
            _ClientID = clientId;
            _RecordID = recordId;
            _EndDate = eDate;
            _UpdateTypes = updateTypes;
            _Delete = delete;
            _IsDailyImport = isDailyImport;
        }

        public void Update()
        {
            string err = string.Empty;

            using (var timer = LogTaskTimer.Start("DataTableWriter.Update", "Updating all data tables (room, tool, store). From last update until {0:yyyy-MM-dd}", () => new object[] { EndDate }))
            {
                foreach (var tt in GetTableTypes())
                {
                    string recordParam = tt.Value;

                    //Clean data always first, then final data
                    foreach (string ut in GetUpdateTypes())
                    {
                        string tableName = string.Format("{0}Data{1}", tt.Key, ut); //e.g. StoreData, StoreDataClean OR RoomData, RoomDataClean
                        string funcName = string.Format("Write{0}", tableName); //e.g. WriteStoreData, WriteStoreDataClean OR WriteRoomData, WriteRoomDataClean
                        DateTime sDate = DateTime.MinValue;

                        using (LogTaskTimer.Start("DataTableWriter.Update", "tableName = {0}, funcName = {1}, sDate = '{2:yyyy-MM-dd}', EndDate = '{3:yyyy-MM-dd}', IsDailyImport = {4}", () => new object[] { tableName, funcName, sDate, EndDate, IsDailyImport }))
                        {
                            //always do clean first (called from within the select)
                            sDate = GetLastUpdate(tableName, recordParam);

                            //IsDailyImport is only ever set by the scheduler service
                            //this is needed because an intermediate update would cause sDate to be equal to eDate

                            //2007-03-07 If it's from Scheduler Service, eDate is always today, if it's from user application,
                            //then the eDate is always the first day of next month

                            //2007-03-08 Just realized that it's possible to have eDate = Now from user app as well, such as aggSumUsage

                            //[2015-02-06 jg]
                            //Adding one day makes sense in the context of ToolData, RoomData, and StoreData because these tables do not have date *and time*
                            //fields like BeginDateTime and EntryDT. They only have the ActDate column which is always at midnight. So this makes sure
                            //that sDate, which we know already exists in the table, is not re-imported. However when we are processing the daily import
                            //sDate and eDate will be equal at this point so we need to decrease sDate by one day so the full previous day is imported.
                            sDate = sDate.AddDays(1).Date;

                            if (IsDailyImport && sDate == EndDate)
                                sDate = sDate.AddDays(-1);

                            DateTime lDate = new DateTime(sDate.Year, sDate.Month, 1); //sDate is either today or yesterday if it's from scheduler service
                            DateTime sd, ed;

                            while (lDate <= EndDate)
                            {
                                sd = sDate;
                                ed = EndDate;

                                if (lDate.AddMonths(1) < EndDate)
                                    ed = lDate.AddMonths(1);

                                if (lDate > sDate)
                                    sd = lDate;

                                switch (funcName)
                                {
                                    case "WriteRoomDataClean":
                                        WriteRoomDataClean(sd, ed);
                                        break;
                                    case "WriteRoomData":
                                        WriteRoomData(sd, ed);
                                        break;
                                    case "WriteToolDataClean":
                                        WriteToolDataClean(sd, ed);
                                        break;
                                    case "WriteToolData":
                                        WriteToolData(sd, ed);
                                        break;
                                    case "WriteStoreDataClean":
                                        WriteStoreDataClean(sd, ed);
                                        break;
                                    case "WriteStoreData":
                                        WriteStoreData(sd, ed);
                                        break;
                                }

                                //2010-02-03 Test code to track who calls this function
                                string body = string.Empty;
                                if (_ClientID != 0 && _RecordID != 0)
                                    body = string.Format("[{0}] function = {1}, ClientID = {2}, RecordID = {3}, sDate = {4}, eDate = {5}, err = {6}", DateTime.Now, funcName, _ClientID, _RecordID, sd, ed, err);
                                else
                                    body = string.Format("[{0}] function = {1}, sDate = {2}, eDate = {3}, err = {4}", DateTime.Now, funcName, sd, ed, err);

                                string subject = string.Format("Call from DataTableWriter.Update [{0}] [{1:yyyy-MM-dd HH:mm:ss}]", tableName, DateTime.Now);

                                ServiceProvider.Current.Email.SendMessage(0, "LNF.Billing.DataTableWriter.Update()", subject, body, SendEmail.SystemEmail, SendEmail.DeveloperEmails);

                                lDate = lDate.AddMonths(1);
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(err))
                throw new Exception(err); //this is a non-fatal error
        }

        public DataTable ReadToolDataRaw(DateTime sd, DateTime ed)
        {
            //if a ActDuration is longer than the max schedulable, chop it
            //ResourceID currently does nothing
            using (var adapter = DA.Current.GetAdapter())
            {
                adapter.SelectCommand
                    .AddParameter("@Action", "ToolDataRaw")
                    .AddParameter("@sDate", sd)
                    .AddParameter("@eDate", ed)
                    .AddParameterIf("@ClientID", ClientID > 0, ClientID)
                    .AddParameterIf("@ResouceID", RecordID > 0, RecordID);

                DataSet ds = adapter.FillDataSet("sselScheduler_Select");

                DataTable dt = ds.Tables[0];

                foreach (DataRow dr in dt.Rows)
                {
                    //2009-08-02 Sandrine said there is no upper limit on any reservation, so i have to comment out this code

                    if (dr["OverTime"] != DBNull.Value)
                    {
                        if (dr.Field<double>("OverTime") < 0.0)
                            dr.SetField<double>("OverTime", 0);
                    }
                    else
                        dr.SetField<double>("OverTime", 0);
                }

                return dt;
            }
        }

        public void WriteToolDataClean(DateTime sd, DateTime ed)
        {
            int rowsSelectedFromScheduler = 0;
            int rowsDeletedFromToolDataClean = 0;
            int rowsInsertedIntoToolDataClean = 0;

            using (LogTaskTimer.Start("DataTableWriter.WriteToolDataClean", "sd = '{0:yyyy-MM-dd}', ed = '{1:yyyy-MM-dd}', rowsSelectedFromScheduler = {2}, rowsDeletedFromToolDataClean = {3}, rowsInsertedIntoToolDataClean = {4}", () => new object[] { sd, ed, rowsSelectedFromScheduler, rowsDeletedFromToolDataClean, rowsInsertedIntoToolDataClean }))
            {
                //write data for all resources at the same time
                DataTable dtSource = ReadToolDataRaw(sd, ed);
                rowsSelectedFromScheduler = dtSource.Rows.Count;

                if (dtSource.Rows.Count > 0)
                {
                    using (var adapter = DA.Current.GetAdapter())
                    {
                        rowsDeletedFromToolDataClean = adapter.SelectCommand
                            .AddParameter("@sDate", sd)
                            .AddParameter("@eDate", ed)
                            .AddParameterIf("@ClientID", ClientID > 0, ClientID)
                            .AddParameterIf("@ResourceID", RecordID > 0, RecordID)
                            .ExecuteNonQuery("ToolDataClean_Delete");

                        DataTable dtClean = LNF.CommonTools.Utility.CopyDT(dtSource);

                        //insert the table into the DB
                        adapter.InsertCommand
                            .AddParameter("@ReservationID", SqlDbType.Int)
                            .AddParameter("@ClientID", SqlDbType.Int)
                            .AddParameter("@ResourceID", SqlDbType.Int)
                            .AddParameter("@RoomID", SqlDbType.Int, 4)
                            .AddParameter("@BeginDateTime", SqlDbType.DateTime)
                            .AddParameter("@EndDateTime", SqlDbType.DateTime)
                            .AddParameter("@ActualBeginDateTime", SqlDbType.DateTime)
                            .AddParameter("@ActualEndDateTime", SqlDbType.DateTime)
                            .AddParameter("@AccountID", SqlDbType.Int)
                            .AddParameter("@ActivityID", SqlDbType.Int)
                            .AddParameter("@SchedDuration", SqlDbType.Float)
                            .AddParameter("@ActDuration", SqlDbType.Float)
                            .AddParameter("@OverTime", SqlDbType.Float)
                            .AddParameter("@IsStarted", SqlDbType.Bit)
                            .AddParameter("@ChargeMultiplier", SqlDbType.Float)
                            //2007-05-22 Add ChargeMultiplier column becaue now we have the forgive charge feature
                            .AddParameter("@IsActive", SqlDbType.Bit)
                            //2011-04 we need this for new tool billing that charge reservation fee
                            .AddParameter("@MaxReservedDuration", SqlDbType.Float)
                            .AddParameter("@CancelledDateTime", SqlDbType.DateTime2)
                            .AddParameter("@OriginalBeginDateTime", SqlDbType.DateTime2)
                            .AddParameter("@OriginalEndDateTime", SqlDbType.DateTime2)
                            .AddParameter("@OriginalModifiedOn", SqlDbType.DateTime2)
                            .AddParameter("@CreatedOn", SqlDbType.DateTime2);

                        rowsInsertedIntoToolDataClean = adapter.UpdateDataTable(dtClean, "ToolDataClean_Insert");
                    }
                }
            }
        }

        public DataTable ReadToolDataClean(DateTime sd, DateTime ed)
        {
            var ds = new DataSet();
            DataTable dtClean;
            DataTable dtClient;
            DataTable dtResource;

            using (var adapter = DA.Current.GetAdapter())
            {
                adapter.SelectCommand
                    .AddParameter("@Action", "ByDateRange")
                    .AddParameter("@sDate", sd)
                    .AddParameter("@eDate", ed)
                    .AddParameterIf("@ClientID", ClientID > 0, ClientID)
                    .AddParameterIf("@ResourceID", RecordID > 0, RecordID);

                ds = adapter.FillDataSet("ToolDataClean_Select");

                dtClean = ds.Tables[0];
                dtClient = ds.Tables[1];
                dtResource = ds.Tables[2];
            }

            //ReadToolDataManager mReadToolData = new ReadToolDataManager();
            DataTable dt = new DataTable();

            if (sd < DateTime.Parse("2011-04-01"))
            {
                //dtToolDataClean = mReadToolData.DailyToolData(sDate, eDate, clientId, resourceId);
                dt.Columns.Add("Period", typeof(DateTime));
                dt.Columns.Add("ReservationID", typeof(int)); //2010-05 we need this to forgive charge on tools
                dt.Columns.Add("ClientID", typeof(int));
                dt.Columns.Add("ResourceID", typeof(int));
                dt.Columns.Add("RoomID", typeof(int));
                dt.Columns.Add("ActDate", typeof(DateTime));
                dt.Columns.Add("AccountID", typeof(int));
                dt.Columns.Add("Uses", typeof(double));
                dt.Columns.Add("SchedDuration", typeof(double));
                dt.Columns.Add("ActDuration", typeof(double));
                dt.Columns.Add("Overtime", typeof(double));
                dt.Columns.Add("IsStarted", typeof(bool));
                dt.Columns.Add("ChargeMultiplier", typeof(double));

                //the select statement pulls in all reservations that overlap the period
                //only write those whose ActDate is within the period

                if (dtClean.Rows.Count > 0)
                {
                    dtClean.Columns.Add("Uses", typeof(double));

                    using (var dba = DA.Current.GetAdapter())
                    {
                        dba.SelectCommand.ApplyParameters(new { Action = "ActivityType" });

                        DataTable dtActivity = dba.FillDataTable("sselScheduler_Select");

                        dtActivity.PrimaryKey = new DataColumn[] { dtActivity.Columns["ActivityID"] };

                        //handled started separately from unstarted
                        int ToolDataCleanRowCount = dtClean.Rows.Count;
                        for (int i = 0; i < ToolDataCleanRowCount; i++)
                        {
                            DataRow wdr = dtClean.Rows[i];

                            //only chargeable activities get written to Tooldata
                            var activityId = wdr.Field<int>("ActivityID");
                            bool isChargeable = dtActivity.Rows.Find(activityId).Field<bool>("Chargeable");

                            if (isChargeable)
                            {
                                //this means reservation was started
                                double schedDuration = wdr.Field<double>("SchedDuration");
                                double actDuration = wdr.Field<double>("ActDuration");
                                DateTime beginDateTime = wdr.Field<DateTime>("BeginDateTime");
                                DateTime endDateTime = beginDateTime.AddMinutes(schedDuration);
                                DateTime actualBeginDateTime = wdr.Field<DateTime>("ActualBeginDateTime");
                                DateTime actualEndDateTime = wdr.Field<DateTime>("ActualEndDateTime");
                                bool isStarted = wdr.Field<bool>("IsStarted");

                                DateTime begDate = isStarted ? actualBeginDateTime : beginDateTime;
                                begDate = begDate.Date;
                                DateTime finDate = isStarted ? actualEndDateTime : endDateTime;
                                finDate = finDate.Date;

                                if (begDate != finDate)
                                {
                                    //handle case where resrevation starts and ends on different days
                                    double runningOverTime = 0;

                                    for (int j = 0; j <= finDate.Subtract(begDate).Days; j++)
                                    {
                                        DateTime bDate = begDate.AddDays(j);
                                        DateTime fDate = begDate.AddDays(j + 1);

                                        DataRow adr = dtClean.NewRow();
                                        adr.ItemArray = wdr.ItemArray; //start by copying everything

                                        if (isStarted)
                                        {
                                            adr["ActualBeginDateTime"] = (bDate > actualBeginDateTime) ? bDate : actualBeginDateTime;
                                            adr["ActualEndDateTime"] = (fDate < actualEndDateTime) ? fDate : actualEndDateTime;
                                            adr["BeginDateTime"] = adr["ActualBeginDateTime"]; //for convenience below when writing new table
                                            adr["ActDuration"] = Convert.ToDateTime(adr["ActualEndDateTime"]).Subtract(Convert.ToDateTime(adr["ActualBeginDateTime"])).TotalMinutes;
                                            adr["Uses"] = Convert.ToDouble(adr["ActDuration"]) / actDuration;
                                            if (Convert.ToDateTime(adr["ActualEndDateTime"]) > endDateTime)
                                            {
                                                adr["OverTime"] = Convert.ToDateTime(adr["ActualEndDateTime"]).Subtract(endDateTime).TotalMinutes - runningOverTime;
                                                runningOverTime += Convert.ToDouble(adr["OverTime"]);
                                            }
                                            else
                                                adr["OverTime"] = 0.0;
                                        }
                                        else
                                        {
                                            adr["BeginDateTime"] = (bDate > beginDateTime) ? bDate : beginDateTime;
                                            adr["EndDateTime"] = (fDate < endDateTime) ? fDate : endDateTime;
                                            adr["SchedDuration"] = Convert.ToDateTime(adr["EndDateTime"]).Subtract(Convert.ToDateTime(adr["BeginDateTime"])).TotalMinutes;
                                            adr["ActDuration"] = adr["SchedDuration"]; //should be 0, but the adjust SP would be way too complex
                                            adr["Uses"] = Convert.ToDouble(adr["SchedDuration"]) / schedDuration;
                                        }

                                        dtClean.Rows.Add(adr); //will add to end
                                    }
                                    wdr.Delete(); //remove original, multi-day row
                                }
                                else
                                {
                                    wdr["Uses"] = 1.0;
                                    if (!Convert.ToBoolean(wdr["IsStarted"]))
                                        wdr["ActDuration"] = wdr["SchedDuration"];
                                }
                            }
                            else
                                wdr.Delete(); //not chargeable, so remove
                        }

                        DateTime ActDate;
                        DataRow ndr;
                        foreach (DataRow wdr in dtClean.Rows)
                        {
                            if (wdr.RowState != DataRowState.Deleted)
                            {
                                ActDate = Convert.ToDateTime(wdr["BeginDateTime"]).Date;
                                if ((ActDate >= sd && ActDate < ed) && Convert.ToDouble(wdr["SchedDuration"]) != 0.0)
                                {
                                    ndr = dt.NewRow();
                                    ndr["ReservationID"] = wdr["ReservationID"];
                                    ndr["ClientID"] = wdr["ClientID"];
                                    ndr["ResourceID"] = wdr["ResourceID"];
                                    ndr["RoomID"] = wdr["RoomID"];
                                    ndr["ActDate"] = ActDate;
                                    ndr["AccountID"] = wdr["AccountID"];
                                    ndr["Uses"] = wdr["Uses"];
                                    ndr["SchedDuration"] = wdr["SchedDuration"];
                                    ndr["ActDuration"] = wdr["ActDuration"];
                                    ndr["Overtime"] = wdr["Overtime"];
                                    ndr["IsStarted"] = wdr["IsStarted"];
                                    ndr["ChargeMultiplier"] = wdr["ChargeMultiplier"];
                                    dt.Rows.Add(ndr);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                //dtClean = mReadToolData.DailyToolData20110401(sDate, eDate, clientId, resourceId);
            }

            return dt;
        }

        public void WriteToolData(DateTime sd, DateTime ed)
        {
            int rowsDeletedFromToolData = 0;
            int rowsSelectedFromToolDataClean = 0;
            int rowsInsertedIntoToolData = 0;
            int rowsAdjustedInToolData = 0;

            using (LogTaskTimer.Start("DataTableWriter.WriteToolData", "sd = '{0:yyyy-MM-dd}', ed = '{1:yyyy-MM-dd}', rowsDeletedFromToolData = {2}, rowsSelectedFromToolDataClean = {3}, rowsInsertedIntoToolData = {4}, rowsAdjustedInToolData = {5}", () => new object[] { sd, ed, rowsDeletedFromToolData, rowsSelectedFromToolDataClean, rowsInsertedIntoToolData, rowsAdjustedInToolData }))
            {
                DateTime period = sd.FirstOfMonth();

                using (var adapter = DA.Current.GetAdapter())
                {
                    //get rid of any non-user entered entries
                    rowsDeletedFromToolData = adapter.SelectCommand
                        .AddParameter("@Period", period)
                        .AddParameterIf("@ClientID", ClientID > 0, ClientID)
                        .AddParameterIf("@ResourceID", RecordID > 0, RecordID)
                        .ExecuteNonQuery("ToolData_Delete");

                    //get all tool data for period
                    DataTable dtToolDataClean = ReadToolDataClean(period, ed);
                    rowsSelectedFromToolDataClean = dtToolDataClean.Rows.Count;

                    adapter.InsertCommand
                        .AddParameter("@Period", period)
                        .AddParameter("@ReservationID", SqlDbType.Int)
                        .AddParameter("@ClientID", SqlDbType.Int)
                        .AddParameter("@ResourceID", SqlDbType.Int)
                        .AddParameter("@RoomID", SqlDbType.Int)
                        .AddParameter("@ActDate", SqlDbType.DateTime)
                        .AddParameter("@AccountID", SqlDbType.Int)
                        .AddParameter("@Uses", SqlDbType.Float)
                        .AddParameter("@SchedDuration", SqlDbType.Float)
                        .AddParameter("@ActDuration", SqlDbType.Float)
                        .AddParameter("@ChargeDuration", SqlDbType.Float)
                        .AddParameter("@TransferredDuration", SqlDbType.Float)
                        .AddParameter("@MaxReservedDuration", SqlDbType.Float)
                        .AddParameter("@OverTime", SqlDbType.Float)
                        .AddParameter("@IsStarted", SqlDbType.Bit)
                        .AddParameter("@IsActive", SqlDbType.Bit)
                        .AddParameter("@ChargeMultiplier", SqlDbType.Float)
                        .AddParameter("@IsCancelledBeforeAllowedTime", SqlDbType.Bit)
                        .AddParameter("@ChargeBeginDateTime", SqlDbType.DateTime2)
                        .AddParameter("@ChargeEndDateTime", SqlDbType.DateTime2);

                    rowsInsertedIntoToolData = adapter.UpdateDataTable(dtToolDataClean, "ToolData_Insert");

                    //adjust ToolData to add the days and months data
                    rowsAdjustedInToolData = adapter.SelectCommand.ApplyParameters(new { Period = period }).ExecuteNonQuery("ToolData_Adjust");
                }
            }
        }

        public void WriteRoomDataClean(DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public void WriteRoomData(DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public void WriteStoreDataClean(DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public void WriteStoreData(DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> GetTableTypes()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (_BillingCategory.HasFlag(BillingCategory.Tool))
                result.Add("Tool", "@ResourceID");
            if (_BillingCategory.HasFlag(BillingCategory.Room))
                result.Add("Room", "@RoomID");
            if (_BillingCategory.HasFlag(BillingCategory.Store))
                result.Add("Store", "@ItemID");

            return result;
        }

        public string[] GetUpdateTypes()
        {
            switch (_UpdateTypes)
            {
                case UpdateDataType.DataClean:
                    return new string[] { "Clean" };
                case UpdateDataType.DataClean | UpdateDataType.Data:
                    return new string[] { "Clean", "" };
                default:
                    return new string[] { "" };
            }
        }

        public DateTime GetLastUpdate(string tableName, string recordParam)
        {
            using (var adapter = DA.Current.GetAdapter())
            {
                adapter.SelectCommand
                    .AddParameter("@Action", "LastUpdate")
                    .AddParameterIf("@ClientID", ClientID > 0, ClientID)
                    .AddParameterIf(recordParam, RecordID > 0, RecordID);

                //This store procedure will delete all record that's of today
                //and will return the max date after the deletion (so the return date should be yesterday)
                var proc = string.Format("{0}_Select", tableName);
                var result = adapter.SelectCommand.ExecuteScalar<DateTime>(proc);

                if (result == default(DateTime))
                    throw new Exception("Cannot get sDate from " + proc);

                return result;
            }
        }
    }
}
