using LNF.Data;
using LNF.Logging;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

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

    public class WriteRoomDataManager
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

        private List<int> accounts;

        protected IAdministrativeHelper AdministrativeHelper => DA.Use<IAdministrativeHelper>();

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ClientID { get; set; }
        public int RoomID { get; set; }

        // force using the static constructor
        private WriteRoomDataManager() { }

        /// <summary>
        /// Creates a new instance of WriteRoomDataManager with the given parameters.
        /// </summary>
        public static WriteRoomDataManager Create(DateTime startDate, DateTime endDate, int clientId = 0, int roomId = 0)
        {
            return new WriteRoomDataManager()
            {
                StartDate = startDate,
                EndDate = endDate,
                ClientID = clientId,
                RoomID = roomId
            };
        }

        /// <summary>
        /// This method will:
        ///     1) Select records from Prowatch in the date range to insert.
        ///     2) Delete records from RoomDataClean in the date range if delete is true.
        ///     3) Insert records from Prowatch into RoomDataClean.
        /// </summary>
        public void WriteRoomDataClean(bool delete = true)
        {
            int rowsSelectedFromProwatch = 0;
            int rowsDeletedFromRoomDataClean = 0;
            int rowsInsertedIntoRoomDataClean = 0;

            using (LogTaskTimer.Start("WriteRoomDataManager.WriteRoomDataClean", "ClientID = {0}, RoomID = {1}, StartDate= '{2}', EndDate = '{3}', RowsSelectedFromProwatch = {4}, RowsDeletedFromRoomDataClean = {5}, RowsInsertedIntoRoomDataClean = {6}", () => new object[] { ClientID, RoomID, StartDate, EndDate, rowsSelectedFromProwatch, rowsDeletedFromRoomDataClean, rowsInsertedIntoRoomDataClean }))
            {
                IReadRoomDataManager roomData = DA.Use<IReadRoomDataManager>();

                DataTable dtSource = roomData.ReadRoomDataFiltered(StartDate, EndDate, ClientID, RoomID);

                rowsSelectedFromProwatch = dtSource.Rows.Count;

                if (rowsSelectedFromProwatch > 0)
                {
                    using (var dba = DA.Current.GetAdapter())
                    {
                        if (delete)
                        {
                            //Delete the data because there are many chances that might need to re-generate the Clean table again and again
                            rowsDeletedFromRoomDataClean = dba.SelectCommand
                                .AddParameter("@sDate", StartDate)
                                .AddParameter("@eDate", EndDate)
                                .AddParameterIf("@ClientID", ClientID > 0, ClientID)
                                .AddParameterIf("@RoomID", RoomID > 0, RoomID)
                                .ExecuteNonQuery("RoomDataClean_Delete");
                        }

                        dba.InsertCommand
                            .AddParameter("@ClientID", SqlDbType.Int)
                            .AddParameter("@RoomID", SqlDbType.Int)
                            .AddParameter("@EntryDT", SqlDbType.DateTime)
                            .AddParameter("@ExitDT", SqlDbType.DateTime)
                            .AddParameter("@Duration", SqlDbType.Float);

                        rowsInsertedIntoRoomDataClean = dba.UpdateDataTable(dtSource, "RoomDataClean_Insert");
                    }
                }
            }
        }

        private int RoomDataPreClean(DateTime period, int clientId = 0, int roomId = 0)
        {
            // [2016-02-01 jg] I'm using NHibernate now because there is a strange timeout issue. The logic here is identical to RoomData_Delete @Action = 'PreClean'

            int result = 0;

            Expression<Func<RoomData, bool>> where;

            if (clientId > 0 && roomId > 0)
                where = x => x.Period == period && x.DataSource != 1 && x.Client.ClientID == clientId && x.Room.RoomID == roomId;
            else if (clientId > 0)
                where = x => x.Period == period && x.DataSource != 1 && x.Client.ClientID == clientId;
            else if (roomId > 0)
                where = x => x.Period == period && x.DataSource != 1 && x.Room.RoomID == roomId;
            else
                where = x => x.Period == period && x.DataSource != 1;

            IList<RoomData> query = DA.Current.Query<RoomData>().Where(where).ToList();

            result = query.Count;

            DA.Current.Delete(query);

            return result;
        }

        private DataTable GetRoomDataPreLock(DateTime period, int clientId = 0, int roomId = 0)
        {
            // [2016-02-01 jg] I'm using NHibernate now because there is a strange timeout issue. The logic here is identical to RoomData_Select @Action = 'PreLock'

            DataTable dt = new DataTable();

            dt.Columns.Add("RoomDataID", typeof(int));
            dt.Columns.Add("ClientID", typeof(int));
            dt.Columns.Add("EvtDate", typeof(DateTime));
            dt.Columns.Add("RoomID", typeof(int));
            dt.Columns.Add("AccountID", typeof(int));
            dt.Columns.Add("Entries", typeof(double));
            dt.Columns.Add("Hours", typeof(double));
            dt.Columns.Add("DataSource", typeof(int));
            dt.Columns.Add("hasToolUsage", typeof(bool));

            // determine the where clauses
            Expression<Func<RoomData, bool>> priorWhere; //for prior periods
            Expression<Func<RoomData, bool>> currentWhere; //for current period

            if (clientId > 0 && roomId > 0)
            {
                priorWhere = x => x.Period < period && x.Client.ClientID == clientId && x.Room.RoomID == roomId;
                currentWhere = x => x.Period == period && x.Client.ClientID == clientId && x.Room.RoomID == roomId;
            }
            else if (clientId > 0)
            {
                priorWhere = x => x.Period < period && x.Client.ClientID == clientId;
                currentWhere = x => x.Period == period && x.Client.ClientID == clientId;
            }
            else if (roomId > 0)
            {
                priorWhere = x => x.Period < period && x.Room.RoomID == roomId;
                currentWhere = x => x.Period == period && x.Room.RoomID == roomId;
            }
            else
            {
                priorWhere = x => x.Period < period;
                currentWhere = x => x.Period == period;
            }

            // 1st get the row for each max EvtDate per room, in the past and
            var agg = DA.Current.Query<RoomData>().Where(priorWhere)
                .GroupBy(x => new { x.Room.RoomID }).Select(x => new { x.Key.RoomID, MaxEvtDate = x.Max(g => g.EvtDate) }).ToArray();

            // then use the agg data to get the max RoomData entry per room (client doesn't matter apparently)
            IList<RoomData> priorPeriod = new List<RoomData>();
            foreach (var a in agg)
                priorPeriod.Add(DA.Current.Query<RoomData>().Where(priorWhere).FirstOrDefault(x => x.Room.RoomID == a.RoomID && x.EvtDate == a.MaxEvtDate));

            // 2nd get the current period data if there is any
            IList<RoomData> currentPeriod = DA.Current.Query<RoomData>().Where(currentWhere).ToList();

            // combine
            IList<RoomData> union = priorPeriod.Union(currentPeriod).OrderBy(x => x.EvtDate).ToList();

            foreach (RoomData rd in union)
            {
                DataRow ndr = dt.NewRow();
                ndr.SetField("RoomDataID", rd.RoomDataID);
                ndr.SetField("ClientID", rd.Client.ClientID);
                ndr.SetField("EvtDate", rd.EvtDate);
                ndr.SetField("RoomID", rd.Room.RoomID);
                ndr.SetField("AccountID", rd.Account.AccountID);
                ndr.SetField("Entries", rd.Entries);
                ndr.SetField("Hours", rd.Hours);
                ndr.SetField("DataSource", rd.DataSource);
                ndr.SetField("hasToolUsage", rd.HasToolUsage);
                dt.Rows.Add(ndr);
            }

            // these are all existing rows so this will make them not get added again
            dt.AcceptChanges();

            return dt;
        }

        private DataTable GetToolDataAggByDay(IList<ToolData> toolData)
        {
            // toolData should already be filtered for Period and ClientID
            // [2016-02-03 jg] The logic here is identical to ToolData_Select @Action = 'AggByDayTool'

            DataTable dt = new DataTable();

            dt.Columns.Add("ClientID", typeof(int));
            dt.Columns.Add("RoomID", typeof(int));
            dt.Columns.Add("ActDate", typeof(DateTime));
            dt.Columns.Add("AccountID", typeof(int));
            dt.Columns.Add("Duration", typeof(double));

            IList<ToolData> notStarted = toolData.Where(x => !x.IsStarted).ToList();
            IList<ToolData> isStarted = toolData.Where(x => x.IsStarted).ToList();

            var union = notStarted
                .Select(x => new { x.ClientID, x.RoomID, x.ActDate, x.AccountID, Duration = x.SchedDuration, x.IsStarted })
                .Union(isStarted.Select(x => new { x.ClientID, x.RoomID, x.ActDate, x.AccountID, Duration = x.ActDuration, x.IsStarted })).ToList();

            var group = union
                .GroupBy(x => new { x.ClientID, x.RoomID, x.ActDate, x.AccountID })
                .Select(x => new { x.Key.ClientID, x.Key.RoomID, x.Key.ActDate, x.Key.AccountID, Duration = x.Sum(g => g.Duration) }).ToList();

            foreach (var g in group)
            {
                DataRow ndr = dt.NewRow();
                ndr.SetField("ClientID", g.ClientID);
                ndr.SetField("RoomID", g.RoomID);
                ndr.SetField("ActDate", g.ActDate);
                ndr.SetField("AccountID", g.AccountID);
                ndr.SetField("Duration", g.Duration);
                dt.Rows.Add(ndr);
            }

            dt.AcceptChanges();

            return dt;
        }

        private int RoomDataAdjust(DateTime period, int clientId = 0, int roomId = 0)
        {
            // [2016-02-01 jg] I'm using NHibernate now because there is a strange timeout issue. The logic here is identical to RoomData_Adjust
            //      I think this is separate from UpdateRoomData because hours need to already be calculated. Maybe they can be combined?

            IList<RoomData> query;

            if (clientId > 0 && roomId > 0)
                query = DA.Current.Query<RoomData>().Where(x => x.Period == period && x.Room.PassbackRoom && x.Client.ClientID == clientId && x.Room.RoomID == roomId).ToList();
            else if (clientId > 0)
                query = DA.Current.Query<RoomData>().Where(x => x.Period == period && x.Room.PassbackRoom && x.Client.ClientID == clientId).ToList();
            else if (roomId > 0)
                query = DA.Current.Query<RoomData>().Where(x => x.Period == period && x.Room.PassbackRoom && x.Room.RoomID == roomId).ToList();
            else
                query = DA.Current.Query<RoomData>().Where(x => x.Period == period && x.Room.PassbackRoom).ToList();

            int result = 0;

            // this gives total entries per day per client per room

            // deal with passback rooms first. calc days, months based on apportioned hours

            // this gives total hours per month per client per room per org
            var hbo = query
                .GroupBy(x => new { x.Client.ClientID, x.Room.RoomID })
                .Select(x => new { cid = x.Key.ClientID, rid = x.Key.RoomID, hrs = x.Sum(g => g.Hours) })
                .ToList();

            // this gives total hours per day per client per room per org
            var hbd = query
                .GroupBy(x => new { x.Client.ClientID, x.Room.RoomID, x.EvtDate })
                .Select(x => new { cid = x.Key.ClientID, rid = x.Key.RoomID, hrs = x.Sum(g => g.Hours), sdate = x.Key.EvtDate })
                .ToList();

            foreach (RoomData rd in query)
            {
                var m = hbo.FirstOrDefault(x => x.cid == rd.Client.ClientID && x.rid == rd.Room.RoomID);
                if (m != null)
                    rd.Months = (m.hrs > 0) ? 1 * rd.Hours / m.hrs : 0;

                var d = hbd.FirstOrDefault(x => x.cid == rd.Client.ClientID && x.rid == rd.Room.RoomID && x.sdate == rd.EvtDate);
                if (d != null)
                    rd.Days = (d.hrs > 0) ? rd.Hours / d.hrs : 0;

                result++;
            }

            // Month and Day are already zero for non-passback rooms;

            return result;
        }

        private DataTable GetClientAccountDataForRoom(DateTime sd, DateTime ed, int clientId = 0)
        {
            // [2016-02-01 jg] The logic here is identical to ClientAccount_Select @Action = 'ForRoomData'

            DataTable dt = new DataTable();

            dt.Columns.Add("AccountID", typeof(int));
            dt.Columns.Add("ClientID", typeof(int));
            dt.Columns.Add("EnableDate", typeof(DateTime));
            dt.Columns.Add("DisableDate", typeof(DateTime));

            IList<ActiveLogClientAccount> query;

            if (clientId > 0)
                query = DA.Current.Query<ActiveLogClientAccount>().Where(x => x.EnableDate < ed && (x.DisableDate == null || x.DisableDate > sd) && x.ClientID == clientId).ToList();
            else
                query = DA.Current.Query<ActiveLogClientAccount>().Where(x => x.EnableDate < ed && (x.DisableDate == null || x.DisableDate > sd)).ToList();

            foreach (ActiveLogClientAccount ca in query)
            {
                DataRow ndr = dt.NewRow();
                ndr.SetField("AccountID", ca.AccountID);
                ndr.SetField("ClientID", ca.ClientID);
                ndr.SetField("EnableDate", ca.EnableDate);
                ndr.SetField("DisableDate", ca.DisableDate);
                dt.Rows.Add(ndr);
            }

            dt.AcceptChanges();

            return dt;
        }

        /// <summary>
        /// This method will:
        ///     1) Delete records from RoomData in the date range.
        ///     2) Select records from RoomDataClean in the date range to insert.
        ///     3) Process the RoomDataClean records.
        ///     4) Insert the processed RoomDataClean records into RoomData.
        ///     5) Adjust records in RoomData.
        /// </summary>
        public void WriteRoomData()
        {
            int rowsDeletedFromRoomData = 0;
            int distinctClientRowsFromRoomDataClean = 0;
            int rowsUpdatedInRoomData = 0;
            int rowsAdjustedInRoomData = 0;

            using (LogTaskTimer.Start("WriteRoomDataManager.WriteRoomData", "ClientID = {0}, RoomID = {1}, StartDate= '{2}', EndDate = '{3}', RowsDeletedFromRoomData = {4}, DistinctClientRowsFromRoomDataClean = {5}, RowsUpdatedInRoomData = {6}, RowsAdjustedInRoomData = {7}", () => new object[] { ClientID, RoomID, StartDate, EndDate, rowsDeletedFromRoomData, distinctClientRowsFromRoomDataClean, rowsUpdatedInRoomData, rowsAdjustedInRoomData }))
            {
                //get rid of any non-user entered entries
                rowsDeletedFromRoomData = RoomDataPreClean(StartDate, ClientID, RoomID);

                //get all access data for period - does agg by day
                IReadRoomDataManager mgr = DA.Use<IReadRoomDataManager>();
                DataTable dtAggRoomDataClean = mgr.AggRoomDataClean(StartDate, EndDate, out DataSet ds, ClientID, RoomID);
                DataTable dtClient = ds.Tables[1];
                DataTable dtRoom = ds.Tables[2];

                // only need to get this ToolData once and then we can use it in multiple functions
                IList<ToolData> toolData;
                if (ClientID > 0)
                    toolData = DA.Current.Query<ToolData>().Where(x => x.Period == StartDate && x.ClientID == ClientID).ToList();
                else
                    toolData = DA.Current.Query<ToolData>().Where(x => x.Period == StartDate).ToList();

                DataTable dtToolUsageData = GetToolUsageData(toolData);

                distinctClientRowsFromRoomDataClean = dtClient.Rows.Count;

                DataTable dtToolData = GetToolDataAggByDay(toolData);
                DataTable dtClientAccount = GetClientAccountDataForRoom(StartDate, EndDate, ClientID);
                DataTable dtRoomData = GetRoomDataPreLock(StartDate, ClientID, RoomID);

                dtRoomData.PrimaryKey = new DataColumn[] { dtRoomData.Columns["RoomDataID"] };
                dtRoomData.PrimaryKey[0].AutoIncrement = true;
                dtRoomData.PrimaryKey[0].AutoIncrementSeed = -1;
                dtRoomData.PrimaryKey[0].AutoIncrementStep = -1;

                string err = string.Empty;
                string errClient = string.Empty;
                string date = string.Empty;
                string room = string.Empty;
                DataRow[] aggRoomDataCleanRows, clientAccountRows;

                ClientPrivilege privsToCheck = ClientPrivilege.LabUser | ClientPrivilege.Staff;

                //TODO: add support for non-passback rooms
                //Wen: dtClient has three columns 1.ClientID, 2.Privs, 3.DisplayName

                //main loop is based on dtClient.Rows, dtClient comes from the DataSet created by ReadRoomDataManager.AggRoomDataClean. The table contains distinct clients that entered or exited rooms during the date range
                foreach (DataRow drClient in dtClient.Rows)
                {
                    if ((drClient.Field<int>("Privs") & (int)privsToCheck) > 0)
                    {
                        errClient = string.Empty;

                        //Wen: dtRoom has three columns 1. RoomID 2.Room Name, 3. PassbackRoom
                        foreach (DataRow drRoom in dtRoom.Rows)
                        {
                            date = string.Empty;
                            room = string.Empty;

                            //Wen: AggRoomDataClean has folowing 5 columns: ClientID, RoomID, EntryDT, Entries, Duration
                            aggRoomDataCleanRows = dtAggRoomDataClean.Select(string.Format("ClientID = {0} AND RoomID = {1}", drClient["ClientID"], drRoom["RoomID"]), "EntryDT");

                            foreach (DataRow drAggRoomDataClean in aggRoomDataCleanRows)
                            {
                                //Wen: so far, all NAP rooms will have ApportedTime as null, because the hours column there are all zeros

                                //get accounts for given day
                                DateTime entryDate = drAggRoomDataClean.Field<DateTime>("EntryDT");
                                //drCliAcct = dtClientAccount.Select(string.Format("ClientID = {0} AND EnableDate < '{1}' AND (DisableDate > '{2}' OR DisableDate IS NULL)", drClient["ClientID"], entryDate, entryDate.AddDays(1)), "AccountID");

                                //[2014-10-27 jg] I think this works better
                                //1) For EnableDate we want to check anything less than (but not equal to) the entryDate plus one day because whatever
                                //   date they are enabled the account is active that entire day.
                                //2) For DisableDate we want to check if it's greater than (but not equal to ) the entryDate (without adding anything) because the day they are
                                //   disabled they are still active that entire day.
                                //This mirrors the logic in sselData.dbo.udf_ActiveClientAccounts with entryDate as sDate and entryDate.AddDays(1) as eDate but here we are only checking one single day
                                clientAccountRows = dtClientAccount.Select(string.Format("ClientID = {0} AND EnableDate < '{1}' AND (DisableDate > '{2}' OR DisableDate IS NULL)",
                                    drClient["ClientID"], entryDate.AddDays(1), entryDate), "AccountID");

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

                                    ExceptionManager exp = new ExceptionManager
                                    {
                                        TimeStamp = DateTime.Now,
                                        ExpName = "User has zero account",
                                        AppName = this.GetType().Assembly.GetName().Name,
                                        FunctionName = "CommonTools-WriteRoomData",
                                        CustomData = string.Format("ClientID = {0}, Period = '{1}'", drClient["ClientID"], StartDate)
                                    };

                                    exp.LogException();
                                }
                                else
                                {
                                    //set up array
                                    accounts = clientAccountRows.Select(x => x.Field<int>("AccountID")).ToList();

                                    //Wen: We no longer look at last time's data to apportion

                                    //a row will be added to dtRoomData in one of these two methods
                                    if (drRoom.Field<bool>("PassbackRoom"))
                                        AntiPassBackRoom_ApportionTime(drClient.Field<int>("ClientID"), drAggRoomDataClean, dtRoomData, dtToolData, dtToolUsageData);
                                    else //handles rooms that are non anti pass back.
                                        NonAntiPassBackRoom_ApportionTime(drClient.Field<int>("ClientID"), drAggRoomDataClean, dtRoomData, dtToolData, dtToolUsageData);
                                }
                            } //access

                            if (date.Length > 0)
                                errClient += room + date;

                        } //room

                        if (errClient.Length > 0)
                            err += "Client: " + drClient["DisplayName"].ToString() + " (" + drClient["ClientID"].ToString() + ") used the " + errClient + " of " + StartDate.ToString("MMMM yyyy") + " but had no active accounts for said dates" + Environment.NewLine;

                    } //priv check
                } //client

                rowsUpdatedInRoomData = RoomDataUpdate(dtRoomData);

                rowsAdjustedInRoomData = RoomDataAdjust(StartDate, ClientID, RoomID);

                if (!string.IsNullOrEmpty(err))
                    AdministrativeHelper.SendEmailToDevelopers(string.Format("Error when creating Room Usage Data on {0:yyy-MM-dd HH:mm:ss}", DateTime.Now), err);
            }
        }

        private int RoomDataUpdate(DataTable dtRoomData)
        {
            // [2016-02-01 jg] I'm using NHibernate now because there is a strange timeout issue. The logic here is identical to RoomData_Insert and RoomData_Delete @Action = 'BadEntry'

            int result = 0;

            foreach (DataRow dr in dtRoomData.Rows)
            {
                if (dr.RowState == DataRowState.Added)
                {
                    DA.Current.Insert(new RoomData()
                    {
                        Period = StartDate,
                        Client = DA.Current.Single<Client>(dr.Field<int>("ClientID")),
                        EvtDate = dr.Field<DateTime>("EvtDate"),
                        HasToolUsage = dr.Field<bool>("hasToolUsage"),
                        Room = DA.Current.Single<Room>(dr.Field<int>("RoomID")),
                        Account = DA.Current.Single<Account>(dr.Field<int>("AccountID")),
                        Entries = Convert.ToDecimal(dr["Entries"]),
                        Hours = Convert.ToDecimal(dr["Hours"]),
                        Days = 0,
                        Months = 0,
                        DataSource = dr.Field<int>("DataSource")
                    });
                    result++;
                }
                else if (dr.RowState == DataRowState.Deleted)
                {
                    int roomDataId = dr.Field<int>("RoomDataID");
                    RoomData rd = DA.Current.Single<RoomData>(roomDataId);
                    if (rd != null)
                        DA.Current.Delete(rd);
                    result++;
                }
            }

            return result;
        }

        private DataTable GetToolUsageData(IList<ToolData> toolData)
        {
            // toolData should already be filtered for Period and ClientID

            // [2016-02-03 jg] I'm using NHibernate now because there is a strange timeout issue. The logic here is identical to ToolData_Select @Action='GetToolUsageDataForGrower'

            DataTable dt = new DataTable();

            dt.Columns.Add("ClientID", typeof(int));
            dt.Columns.Add("RoomID", typeof(int));
            dt.Columns.Add("ActDate", typeof(DateTime));

            IList<ToolData> query = toolData.Where(x => x.IsStarted && x.ChargeMultiplier > 0).ToList();

            foreach (ToolData td in query)
            {
                DataRow ndr = dt.NewRow();
                ndr.SetField("ClientID", td.ClientID);
                ndr.SetField("RoomID", td.RoomID);
                ndr.SetField("ActDate", td.ActDate);
                dt.Rows.Add(ndr);
            }

            dt.AcceptChanges();

            return dt;
        }

        private bool HasToolUsage(int clientId, int roomId, DateTime ActDate, DataTable dtToolData)
        {
            return dtToolData.AsEnumerable().Any(x => x.Field<int>("ClientID") == clientId && x.Field<int>("RoomID") == roomId && x.Field<DateTime>("ActDate") == ActDate);
        }

        private void AntiPassBackRoom_ApportionTime(int clientId, DataRow drAggRoomDataClean, DataTable dtRoomData, DataTable dtToolData, DataTable dtToolUsageData)
        {
            DataRow ndr;

            int roomId = drAggRoomDataClean.Field<int>("RoomID");
            DataRow[] fdr = dtToolData.Select(string.Format("ClientID = {0} AND RoomID = {1} AND ActDate='{2}'", clientId, roomId, drAggRoomDataClean["EntryDT"]));
            for (int i = 0; i < fdr.Length; i++)
            {
                bool matched = false;
                foreach (int accountId in accounts)
                {
                    if (accountId == fdr[i].Field<int>("AccountID"))
                    {
                        matched = true;
                        break;
                    }
                }

                if (!matched)
                    accounts.Add(fdr[i].Field<int>("AccountID"));
            }

            for (int i = 0; i < accounts.Count; i++)
            {
                DateTime eventDate = drAggRoomDataClean.Field<DateTime>("EntryDT").Date;
                ndr = dtRoomData.NewRow();
                ndr["ClientID"] = clientId;
                ndr["EvtDate"] = eventDate;
                ndr["hasToolUsage"] = HasToolUsage(clientId, roomId, eventDate, dtToolUsageData);
                ndr["RoomID"] = drAggRoomDataClean.Field<int>("RoomID");
                ndr["AccountID"] = accounts[i];
                ndr["Entries"] = drAggRoomDataClean.Field<double>("Entries") / Convert.ToDouble(accounts.Count);
                ndr["Hours"] = drAggRoomDataClean.Field<double>("Duration") / Convert.ToDouble(accounts.Count);
                ndr["DataSource"] = DataSourceType.ApportionEqually;
                dtRoomData.Rows.Add(ndr);
            }

            accounts.Clear();
        }

        private void NonAntiPassBackRoom_ApportionTime(int clientId, DataRow drAggRoomDataClean, DataTable dtRoomData, DataTable dtToolData, DataTable dtToolUsageData)
        {
            DataRow ndr;

            int roomId = drAggRoomDataClean.Field<int>("RoomID");
            DataRow[] fdr = dtToolData.Select(string.Format("ClientID = {0} AND RoomID = {1} AND ActDate = '{2}'", clientId, roomId, drAggRoomDataClean["EntryDT"]));
            for (int i = 0; i < fdr.Length; i++)
            {
                bool matched = false;
                foreach (int accountId in accounts)
                {
                    if (accountId == fdr[i].Field<int>("AccountID"))
                    {
                        matched = true;
                        break;
                    }
                }

                if (!matched)
                    accounts.Add(fdr[i].Field<int>("AccountID"));
            }

            for (int i = 0; i < accounts.Count; i++)
            {
                DateTime eventDate = drAggRoomDataClean.Field<DateTime>("EntryDT").Date;

                ndr = dtRoomData.NewRow();
                ndr["ClientID"] = clientId;
                ndr["evtDate"] = eventDate;
                ndr["hasToolUsage"] = HasToolUsage(clientId, roomId, eventDate, dtToolUsageData);
                ndr["RoomID"] = drAggRoomDataClean.Field<int>("RoomID");
                ndr["AccountID"] = accounts[i];
                ndr["Entries"] = drAggRoomDataClean.Field<double>("Entries") / Convert.ToDouble(accounts.Count);
                ndr["Hours"] = 0; //default for non anti pass back room is 0 hour, but this might affect the Months column
                ndr["DataSource"] = DataSourceType.ApportionEqually;
                dtRoomData.Rows.Add(ndr);
            }

            //Always remember to clear this accounts List
            accounts.Clear();
        }

        private void ApportionTime(int clientId, DataRow drAggRoomDataClean, DataRow[] drRoomDataLast, DataTable dtRoomData, DataTable dtToolData)
        {
            //define this to avoid checking against drRoomDataLast again
            DataSourceType lastDataSource = DataSourceType.Undefined;
            if (drRoomDataLast.Length > 0)
                lastDataSource = (DataSourceType)drRoomDataLast[0]["DataSource"];

            //always try propogating first
            double totalLabTime = 0D;
            if (lastDataSource < DataSourceType.ApportionEqually)
                totalLabTime = PropagateCheck(drRoomDataLast);

            if (lastDataSource <= DataSourceType.PropagateFromUser && totalLabTime > 0.0001)
                PropagateLabTime(clientId, drAggRoomDataClean, drRoomDataLast, totalLabTime, (int)DataSourceType.PropagateFromUser, dtRoomData);
            else
            {
                //if resev were made, use them as basis for apport
                double totalToolMinutes = 0D;
                object computeResult;
                if (dtToolData.Rows.Count > 0)
                {
                    computeResult = dtToolData.Compute("SUM(Duration)", string.Format("ClientID = {0} AND RoomID = {1} AND ActDate = '{2}", clientId, drAggRoomDataClean["RoomID"], drAggRoomDataClean["EntryDT"]));
                    if (!computeResult.Equals(DBNull.Value) && !computeResult.Equals(null))
                        totalToolMinutes = Convert.ToDouble(computeResult);
                }

                ApportionEqually(clientId, drAggRoomDataClean, dtRoomData);
            }

            accounts.Clear();
        }

        private double PropagateCheck(DataRow[] drRoomDataLast)
        {
            Double totalLabTime = 0D;
            for (int i = 0; i < drRoomDataLast.Length; i++)
            {
                totalLabTime += drRoomDataLast[i].Field<double>("Hours"); //from last apport entry
            }

            //inactive account - reduce totalLabTime by time on acct
            int j;
            for (int i = 0; i < drRoomDataLast.Length; i++)
            {
                for (j = 0; j < accounts.Count; j++)
                {
                    if (drRoomDataLast[i].Field<int>("AccountID") == accounts[j])
                        break;
                }
                if (j == accounts.Count) //account has been disabled
                    totalLabTime -= drRoomDataLast[i].Field<double>("Hours");
            }

            return totalLabTime;
        }

        private void PropagateLabTime(int clientId, DataRow drAggRoomDataClean, DataRow[] drRoomDataLast, double totalLabTime, int dataSource, DataTable dtRoomData)
        {
            DataRow ndr;
            for (int i = 0; i < drRoomDataLast.Length; i++)
            {
                ndr = dtRoomData.NewRow();
                ndr["ClientID"] = clientId;
                ndr["evtDate"] = drAggRoomDataClean.Field<DateTime>("EntryDT").Date;
                ndr["RoomID"] = drAggRoomDataClean.Field<int>("RoomID");
                ndr["AccountID"] = drRoomDataLast[i].Field<int>("AccountID");
                ndr["Entries"] = drRoomDataLast[i].Field<double>("Hours") * drAggRoomDataClean.Field<double>("Entries") / totalLabTime;
                ndr["Hours"] = drRoomDataLast[i].Field<double>("Hours") * drAggRoomDataClean.Field<double>("Duration") / totalLabTime;
                ndr["DataSource"] = dataSource;
                dtRoomData.Rows.Add(ndr);

                accounts.Remove(drRoomDataLast[i].Field<int>("AccountID"));
            }

            for (int i = 0; i < accounts.Count; i++)
            {
                ndr = dtRoomData.NewRow();
                ndr["ClientID"] = clientId;
                ndr["evtDate"] = drAggRoomDataClean.Field<DateTime>("EntryDT").Date;
                ndr["RoomID"] = drAggRoomDataClean.Field<int>("RoomID");
                ndr["AccountID"] = accounts[i];
                ndr["Entries"] = 0.0;
                ndr["Hours"] = 0.0;
                ndr["DataSource"] = dataSource;
                dtRoomData.Rows.Add(ndr);
            }
        }

        private void ApportionByToolTime(int clientId, DataRow drAggRoomDataClean, DataTable dtToolData, double totalToolMinutes, DataTable dtRoomData)
        {
            DataRow ndr;
            DataRow[] fdr = dtToolData.Select(string.Format("ClientID = {0} AND RoomID = {1} AND ActDate='{2}'", clientId, drAggRoomDataClean["RoomID"], drAggRoomDataClean["EntryDT"]));
            for (int i = 0; i < fdr.Length; i++)
            {
                ndr = dtRoomData.NewRow();
                ndr["ClientID"] = clientId;
                ndr["evtDate"] = drAggRoomDataClean.Field<DateTime>("EntryDT").Date;
                ndr["RoomID"] = drAggRoomDataClean.Field<int>("RoomID");
                ndr["AccountID"] = fdr[i].Field<int>("AccountID");
                ndr["Entries"] = fdr[i].Field<double>("Duration") * drAggRoomDataClean.Field<double>("Entries") / totalToolMinutes;
                ndr["Hours"] = fdr[i].Field<double>("Duration") * drAggRoomDataClean.Field<double>("Duration") / totalToolMinutes;
                ndr["DataSource"] = DataSourceType.ApportionByToolTime;
                dtRoomData.Rows.Add(ndr);

                accounts.Remove(fdr[i].Field<int>("AccountID"));
            }

            for (int i = 0; i < accounts.Count; i++)
            {
                ndr = dtRoomData.NewRow();
                ndr["ClientID"] = clientId;
                ndr["evtDate"] = drAggRoomDataClean.Field<DateTime>("EntryDT").Date;
                ndr["RoomID"] = drAggRoomDataClean.Field<int>("RoomID");
                ndr["AccountID"] = accounts[i];
                ndr["Entries"] = 0.0;
                ndr["Hours"] = 0.0;
                ndr["DataSource"] = DataSourceType.ApportionByToolTime;
                dtRoomData.Rows.Add(ndr);
            }
        }

        private void ApportionEqually(int clientId, DataRow drAggRoomDataClean, DataTable dtRoomData)
        {
            DataRow ndr;

            for (int i = 0; i < accounts.Count; i++)
            {
                ndr = dtRoomData.NewRow();
                ndr["ClientID"] = clientId;
                ndr["evtDate"] = drAggRoomDataClean.Field<DateTime>("EntryDT").Date;
                ndr["RoomID"] = drAggRoomDataClean.Field<int>("RoomID");
                ndr["AccountID"] = accounts[i];
                ndr["Entries"] = drAggRoomDataClean.Field<double>("Entries") / Convert.ToDouble(accounts.Count);
                ndr["Hours"] = drAggRoomDataClean.Field<double>("Duration") / Convert.ToDouble(accounts.Count);
                ndr["DataSource"] = DataSourceType.ApportionEqually;
                dtRoomData.Rows.Add(ndr);
            }
        }
    }
}
