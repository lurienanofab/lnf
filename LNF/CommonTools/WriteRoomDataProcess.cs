﻿using LNF.Models.Billing.Process;
using LNF.Models.Data;
using LNF.Repository;
using System;
using System.Data;
using System.Linq;

namespace LNF.CommonTools
{
    #region "Explanation of Data Source Types"
    //TODO: Add support for apportioning entries for non-hourly rooms

    //Apportionment scheme - design notes:

    //Each DataRow is marked with a DataSource which can be one of:
    //  1 - User Entered
    //  2 - Propagated, propagation root is user entered
    //  4 - Apportioned by tool time
    //  5 - Propagated, propagation root is apportioned by tool time
    //  6 - Apportioned equally by accounts

    //The DataSources are listed in order of preference, i.e., User Entered is the 'best' source, etc.

    //For a specified period for a specified user for a specified room 
    //- If there is user entered apportionment data, use them
    //- Else (no user entered apportionment data)
    //  - Get list of active accounts
    //  - If no active accounts
    //    - Flag error
    //  - Else
    //    - Call ApportionTime
    //*** end

    //subroutine ApportionTime
    //- Get dataSource of previous apportionment data. If no previous data, use value of 99
    //- If valid data source
    //  - Call PropagateCheck, assigns returned value to totalLabTime
    //- if dataSource <= 2 and totalLabTime > 0
    //  - Call PropagateLabTime - pass in totalLabTime and mark row with dataSource=2
    //- else get tool data
    //  - if tool data exists
    //    - Call apportionByToolTime and mark row with dataSource=4
    //  - otherwise (no tool data)
    //    - if no previous apportionment
    //      - Call apportionEqually and mark row with dataSource=6
    //    - else if all previously used accounts disabled
    //      - Call apportionEqually and mark row with dataSource=6
    //    - else if previous apportionment was based on tool time and previously used account(s) are still enabled
    //      - Call PropagateLabTime - pass in totalLabTime and mark row with dataSource=5
    //    - else
    //      - Call apportionEqually and mark row with dataSource=6
    //*** end

    //function PropagateCheck return double
    //- returns total number of lab hours of previous apportionment that are associated with accounts that are still active
    //    subroutine(PropagateLabTime)
    //- calculates percentages from previous apportionment and totalLabTime, and uses these for the current data. (By default, if a account did not previously exist, it get a 0.)
    //    subroutine(apportionByToolTime)
    //- calculates the percentage of time charged to each account and spreads lab time according to these percentages
    //    subroutine(apportionEqually)
    //- charges each account a fraction of the total lab time based on the number of accounts

    //regardless of the sDate passed in, this must be run from the first day of the month
    #endregion

    // [2018-09-13 jg]
    // I'm going back to using stored procedures for everything. NHibernate doesn't work very well when dealing
    // with large data sets. There were too many situations where individual records where selected inside a
    // foreach loop, or similar issues, that killed performance. This is basically an ETL process and I think
    // NHibernate is not well suited for this.

    /// <summary>
    /// This process will:
    ///     1) Delete records from RoomData in the date range.
    ///     2) Select records from RoomDataClean in the date range to insert.
    ///     3) Process the RoomDataClean records.
    ///     4) Insert the processed RoomDataClean records into RoomData.
    ///     5) Adjust records in RoomData.
    /// </summary>
    public class WriteRoomDataProcess : ProcessBase<WriteRoomDataProcessResult>
    {
        public enum DataSourceType
        {
            UserEntered = 1,
            PropagateFromUser = 2,
            ApportionByToolTime = 4,
            PropagateFromToolTime = 5,
            ApportionEqually = 6,
            Undefined = 99
        }

        public IAdministrativeHelper AdministrativeHelper => ServiceProvider.Current.Use<IAdministrativeHelper>();

        public DateTime Period { get; }
        public int ClientID { get; }
        public int RoomID { get; }

        private DataSet _ds;

        protected override WriteRoomDataProcessResult CreateResult()
        {
            return new WriteRoomDataProcessResult
            {
                Period = Period,
                ClientID = ClientID,
                RoomID = RoomID
            };
        }

        public WriteRoomDataProcess(DateTime period, int clientId = 0, int roomId = 0)
        {
            Period = period;
            ClientID = clientId;
            RoomID = roomId;
        }

        public override int DeleteExisting()
        {
            return DA.Command()
                .Param("Action", "PreClean")
                .Param("Period", Period)
                .Param("ClientID", ClientID > 0, ClientID)
                .Param("RoomID", RoomID > 0, RoomID)
                .ExecuteNonQuery("dbo.RoomData_Delete").Value;
        }

        public override DataTable Extract()
        {
            DateTime sd = Period;
            DateTime ed = Period.AddMonths(1);
            _ds = ReadData.Room.ReadRoomDataClean(sd, ed, ClientID, RoomID);
            return _ds.Tables["RoomDataClean"];
        }

        public override DataTable Transform(DataTable dtExtract)
        {
            // get all access data for period - does agg by day
            DataTable dtAggRoomDataClean = AggRoomDataClean(dtExtract);
            DataTable dtClient = _ds.Tables["Client"];
            DataTable dtRoom = _ds.Tables["Room"];

            _result.DistinctClientRows = dtClient.Rows.Count;

            var aggRoomDataClean = dtAggRoomDataClean.AsEnumerable();
            var toolData = GetToolDataAggByDay().AsEnumerable();
            var clientAccount = GetClientAccountDataForRoom().AsEnumerable();
            var toolUsageData = GetToolUsageData().AsEnumerable();

            var dtRoomData = GetRoomDataPreLock();

            dtRoomData.PrimaryKey = new[] { dtRoomData.Columns["RoomDataID"] };
            dtRoomData.PrimaryKey[0].AutoIncrement = true;
            dtRoomData.PrimaryKey[0].AutoIncrementSeed = -1;
            dtRoomData.PrimaryKey[0].AutoIncrementStep = -1;

            string err = string.Empty;
            string errClient = string.Empty;
            string date = string.Empty;
            string room = string.Empty;
            DataRow[] aggRoomDataCleanRows, clientAccountRows;

            ClientPrivilege privsToCheck = ClientPrivilege.LabUser | ClientPrivilege.Staff;

            // TODO: add support for non-passback rooms
            // Wen: dtClient has three columns 1.ClientID, 2.Privs, 3.DisplayName

            // Main loop is based on dtClient.Rows, dtClient comes from the DataSet created by ReadRoomDataManager.AggRoomDataClean.
            // The table contains distinct clients that entered or exited rooms during the date range.
            foreach (DataRow drClient in dtClient.Rows)
            {
                if ((drClient.Field<int>("Privs") & (int)privsToCheck) > 0)
                {
                    errClient = string.Empty;

                    // Wen: dtRoom has three columns 1. RoomID 2.Room Name, 3. PassbackRoom
                    foreach (DataRow drRoom in dtRoom.Rows)
                    {
                        date = string.Empty;
                        room = string.Empty;

                        int cid = drClient.Field<int>("ClientID");
                        int rid = drRoom.Field<int>("RoomID");
                        int? pid = drRoom.Field<int?>("ParentID");
                        bool pbr = drRoom.Field<bool>("PassbackRoom");

                        // Wen: AggRoomDataClean has folowing 5 columns: ClientID, RoomID, EntryDT, Entries, Duration
                        aggRoomDataCleanRows = aggRoomDataClean.Where(x => x.Field<int>("ClientID") == cid && x.Field<int>("RoomID") == rid).OrderBy(x => x.Field<DateTime>("EntryDT")).ToArray();

                        foreach (DataRow drAggRoomDataClean in aggRoomDataCleanRows)
                        {
                            // Wen: so far, all NAP rooms will have ApportedTime as null, because the hours column there are all zeros

                            // get accounts for given day
                            DateTime entryDate = drAggRoomDataClean.Field<DateTime>("EntryDT");

                            // [2014-10-27 jg] I think this works better
                            // 1) For EnableDate we want to check anything less than (but not equal to) the entryDate plus one day because whatever
                            //    date they are enabled the account is active that entire day.
                            // 2) For DisableDate we want to check if it's greater than (but not equal to ) the entryDate (without adding anything) because the day they are
                            //    disabled they are still active that entire day.
                            // This mirrors the logic in sselData.dbo.udf_ActiveClientAccounts with entryDate as sDate and entryDate.AddDays(1) as eDate but here we are only checking one single day
                            clientAccountRows = clientAccount
                                .Where(x =>
                                    x.Field<int>("ClientID") == cid
                                    && x.Field<DateTime>("EnableDate") < entryDate.AddDays(1)
                                    && (x.Field<DateTime?>("DisableDate") == null || x.Field<DateTime?>("DisableDate") > entryDate))
                                .OrderBy(x => x.Field<int>("AccountID"))
                                .ToArray();

                            if (clientAccountRows.Length == 0) //no accounts - this is an error
                            {
                                if (room.Length == 0)
                                {
                                    room = drRoom.Field<string>("Room") + " on ";
                                    if (errClient.Length > 0)
                                        room = " and the " + room;
                                }

                                if (date.Length == 0)
                                    date = entryDate.ToString("dd");
                                else
                                    date += ", " + entryDate.ToString("dd");

                                HandleRoomDataException(entryDate, cid, "LNF.CommonToolsWriteRoomDataProcess.Start");
                            }
                            else
                            {
                                // set up array
                                var list = clientAccountRows.Select(x => x.Field<int>("AccountID")).ToList();
                                list.AddRange(toolData.Where(x => x.Field<int>("ClientID") == cid && x.Field<int>("RoomID") == rid && x.Field<DateTime>("ActDate") == entryDate).Select(x => x.Field<int>("AccountID")));
                                var accounts = list.Distinct().ToArray();

                                // Wen: We no longer look at last time's data to apportion

                                var eventDate = entryDate.Date;
                                var entries = drAggRoomDataClean.Field<double>("Entries");
                                var duration = pbr ? drAggRoomDataClean.Field<double>("Duration") : 0;
                                var hasToolUsage = HasToolUsage(toolUsageData, cid, rid, eventDate);

                                // a row will be added to dtRoomData for each account, handles both passback and non-passback rooms
                                AddRoomDataRow(dtRoomData, accounts, cid, rid, pid, pbr, eventDate, entries, duration, hasToolUsage);
                            }
                        } //access

                        if (date.Length > 0)
                            errClient += room + date;

                    } //room

                    if (errClient.Length > 0)
                        err += $"Client: {drClient["DisplayName"]} ({drClient["ClientID"]}) used the {errClient} of {Period:MMMM yyyy} but had no active accounts for said dates{Environment.NewLine}";

                } //priv check
            } //client

            if (!string.IsNullOrEmpty(err))
                AdministrativeHelper.SendEmailToDevelopers($"Error when creating Room Usage Data on {DateTime.Now:yyy-MM-dd HH:mm:ss}", err);

            return dtRoomData;
        }

        public override int Load(DataTable dtTransform)
        {
            var rows = dtTransform.AsEnumerable();

            int result = rows.Count(x => x.RowState == DataRowState.Added);

            using (var bcp = CreateBulkCopy())
                bcp.WriteToServer(dtTransform, DataRowState.Added);

            var deletedRows = rows.Where(x => x.RowState == DataRowState.Deleted).ToArray();

            int count = 0;

            if (deletedRows.Length > 0)
            {
                count = DA.Command().Update(deletedRows, x =>
                {
                    x.Delete.SetCommandText("dbo.RoomData_Delete");
                    x.Delete.AddParameter("Action", "BadEntry");
                    x.Delete.AddParameter("RoomDataID", DbType.Int32, 4);
                });
            }

            _result.BadEntryRowsDeleted = count;
            _result.RowsAdjusted = RoomDataAdjust();

            return result;
        }

        public override IBulkCopy CreateBulkCopy()
        {
            IBulkCopy bcp = DA.Current.GetBulkCopy("dbo.RoomData");
            bcp.AddColumnMapping("Period");
            bcp.AddColumnMapping("ClientID");
            bcp.AddColumnMapping("RoomID");
            bcp.AddColumnMapping("ParentID");
            bcp.AddColumnMapping("PassbackRoom");
            bcp.AddColumnMapping("EvtDate");
            bcp.AddColumnMapping("AccountID");
            bcp.AddColumnMapping("Entries");
            bcp.AddColumnMapping("Hours");
            bcp.AddColumnMapping("Days");
            bcp.AddColumnMapping("Months");
            bcp.AddColumnMapping("DataSource");
            bcp.AddColumnMapping("HasToolUsage");
            return bcp;
        }

        //in this function, we first find all the days an event belongs to, then agg by day
        //don't need to worry about charged rooms as clean data only has charged rooms
        private DataTable AggRoomDataClean(DataTable dtRoomData)
        {
            DataTable dtAggRoomDataClean = new DataTable("AggRoomDataClean");

            dtAggRoomDataClean.Columns.Add("ClientID", typeof(int));
            dtAggRoomDataClean.Columns.Add("RoomID", typeof(int));
            dtAggRoomDataClean.Columns.Add("EntryDT", typeof(DateTime));
            dtAggRoomDataClean.Columns.Add("Entries", typeof(double));
            dtAggRoomDataClean.Columns.Add("Duration", typeof(double));

            DataTable dtClient = _ds.Tables["Client"];
            DataTable dtRoom = _ds.Tables["Room"];

            dtRoomData.Columns.Add("eDay", typeof(int)); //used below in the compute
            dtRoomData.Columns.Add("Entries", typeof(double));

            _ds.Tables.Add(dtAggRoomDataClean);

            if (dtRoomData.Rows.Count == 0)
                return dtAggRoomDataClean;

            double entries;
            double duration;
            DataRow ndr;

            // need to use for (not foreach) because the enumeration changes when a row is added and this causes an exception
            int rowCount = dtRoomData.Rows.Count;
            for (int i = 0; i < rowCount; i++)
            {
                DataRow drRoomDataClean = dtRoomData.Rows[i];
                int rid = drRoomDataClean.Field<int>("RoomID");
                bool passbackRoom = drRoomDataClean.Field<bool>("PassbackRoom");
                DateTime entryDate = drRoomDataClean.Field<DateTime>("EntryDT");
                drRoomDataClean.SetField("eDay", entryDate.Day);
                drRoomDataClean.SetField("Entries", 1D);

                if (passbackRoom)
                {
                    DateTime exitDate = drRoomDataClean.Field<DateTime>("exitDT");

                    //note that no room event can be longer than MaxTime hours
                    if (entryDate.Day != exitDate.Day)
                    {
                        //entry date and exit date are different, we have to create a new data row
                        duration = exitDate.Subtract(entryDate).TotalHours;
                        DateTime newDate = new DateTime(exitDate.Year, exitDate.Month, exitDate.Day);

                        drRoomDataClean.SetField("ExitDT", newDate);
                        drRoomDataClean.SetField("Duration", newDate.Subtract(entryDate).TotalHours);
                        drRoomDataClean.SetField("Entries", drRoomDataClean.Field<double>("Duration") / duration);

                        ndr = dtRoomData.NewRow();
                        ndr.ItemArray = drRoomDataClean.ItemArray; //start by copying everything
                        ndr.SetField("EntryDT", newDate);
                        ndr.SetField("eDay", newDate.Day);
                        ndr.SetField("ExitDT", exitDate);
                        ndr.SetField("Duration", exitDate.Subtract(newDate).TotalHours);
                        ndr.SetField("Entries", ndr.Field<double>("Duration") / duration);
                        dtRoomData.Rows.Add(ndr);
                    }
                }
            }

            DateTime sd = Period;
            DateTime ed = Period.AddMonths(1);

            // delete the records that are out of bound of current month, typically it happens when user enters
            // at the last day of month and exit the first day of month etc.
            DataRow[] fdrDelete = dtRoomData.Select($"EntryDT < '{sd}' OR EntryDT >= '{ed}'");
            foreach (DataRow dr in fdrDelete)
                dtRoomData.Rows.Remove(dr); // Remove is the same as calling Delete and then AcceptChanges per the Microsoft docs

            // at this point, the client/room has proper number of records
            // so, at this time, agg by day and add records to new dt
            // TODO: make this more efficient
            int upperbound = ed.Subtract(sd).Days + 1;
            for (int i = 1; i <= upperbound; i++)
            {
                foreach (DataRow drClient in dtClient.Rows)
                {
                    foreach (DataRow drRoom in dtRoom.Rows)
                    {
                        int cid = drClient.Field<int>("ClientID");
                        int rid = drRoom.Field<int>("RoomID");
                        string filter = $"eDay = {i} AND ClientID = {cid} AND RoomID = {rid}";

                        object val = dtRoomData.Compute("SUM(Entries)", filter);

                        if (!Utility.IsDBNullOrNull(val))
                        {
                            entries = Utility.ConvertToDouble(val);
                            duration = Utility.ConvertToDouble(dtRoomData.Compute("SUM(Duration)", filter));
                            ndr = dtAggRoomDataClean.NewRow();
                            ndr.SetField("ClientID", cid);
                            ndr.SetField("RoomID", rid);
                            ndr.SetField("EntryDT", sd.AddDays(i - 1));
                            ndr.SetField("Entries", entries);
                            ndr.SetField("Duration", duration);
                            dtAggRoomDataClean.Rows.Add(ndr);
                        }
                    }
                }
            }

            // These are already existing rows in RoomDataClean, there's nothing to be inserted, updated, or deleted
            // so this is just a precaution to make sure nothing happens later.
            dtAggRoomDataClean.AcceptChanges();

            return dtAggRoomDataClean;
        }

        private bool HasToolUsage(EnumerableRowCollection<DataRow> toolUsageData, int clientId, int roomId, DateTime eventDate)
        {
            bool result = toolUsageData.Any(x =>
                x.Field<int>("ClientID") == clientId
                && x.Field<int>("RoomID") == roomId
                && x.Field<DateTime>("ActDate") == eventDate.Date);

            return result;
        }

        private DataTable GetRoomDataPreLock()
        {
            return DA.Command()
                .Param("Action", "PreLock")
                .Param("Period", Period)
                .Param("ClientID", ClientID > 0, ClientID)
                .Param("RoomID", RoomID > 0, RoomID)
                .FillDataTable("dbo.RoomData_Select");
        }

        private DataTable GetToolDataAggByDay()
        {
            return DA.Command()
                .Param("Action", "AggByDayTool")
                .Param("Period", Period)
                .Param("ClientID", ClientID > 0, ClientID)
                .FillDataTable("dbo.ToolData_Select");
        }

        private int RoomDataAdjust() => DA.Command().Param("Period", Period).ExecuteNonQuery("dbo.RoomData_Adjust").Value;

        private DataTable GetClientAccountDataForRoom()
        {
            DateTime sd = Period;
            DateTime ed = Period.AddMonths(1);

            return DA.Command()
                .Param("Action", "ForRoomData")
                .Param("sDate", sd)
                .Param("eDate", ed)
                .Param("ClientID", ClientID > 0, ClientID)
                .FillDataTable("dbo.ClientAccount_Select");
        }

        private void HandleRoomDataException(DateTime entryDate, int clientId, string funcName)
        {
            ExceptionManager exp = new ExceptionManager()
            {
                TimeStamp = DateTime.Now,
                ExpName = "User has no accounts",
                AppName = GetType().Assembly.GetName().Name,
                FunctionName = funcName,
                CustomData = $"ClientID = {clientId}, EntryDate = '{entryDate}'"
            };

            exp.LogException();
        }

        private DataTable GetToolUsageData()
        {
            return DA.Command()
                .Param("Action", "GetToolUsageDataForGrower")
                .Param("Period", Period)
                .FillDataTable("dbo.ToolData_Select");
        }

        private void AddRoomDataRow(DataTable dtRoomData, int[] accounts, int clientId, int roomId, int? parentId, bool passbackRoom, DateTime eventDate, double entries, double duration, bool hasToolUsage)
        {
            double count = accounts.Length;

            foreach (var accountId in accounts)
            {
                DataRow ndr = dtRoomData.NewRow();
                ndr.SetField("Period", Period);
                ndr.SetField("ClientID", clientId);
                ndr.SetField("RoomID", roomId);
                ndr.SetField("ParentID", parentId);
                ndr.SetField("PassbackRoom", passbackRoom);
                ndr.SetField("EvtDate", eventDate);
                ndr.SetField("AccountID", accountId);
                ndr.SetField("Entries", entries / count);
                ndr.SetField("Hours", duration / count);
                ndr.SetField("Days", 0D);
                ndr.SetField("Months", 0D);
                ndr.SetField("DataSource", DataSourceType.ApportionEqually);
                ndr.SetField("HasToolUsage", hasToolUsage);
                dtRoomData.Rows.Add(ndr);
            }
        }
    }
}
