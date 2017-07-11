using LNF.Data;
using LNF.PhysicalAccess;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.CommonTools
{
    //sdate must always be first of the month. edate can never be > first of next month
    //raw means straight from the DB
    //filtered means that extraneous data have been removed - this is stored in RoomData
    //clean means read from clean table
    public class ReadRoomDataManager
    {
        //private DataSet ds = new DataSet();
        private const double MaxTime = 8.0;

        internal ReadRoomDataManager() { }

        public DataTable ReadRoomData(DateTime period, int clientId = 0, int roomId = 0)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.SelectCommand
                    .AddParameter("@Action", "AggByMonthRoom")
                    .AddParameter("@Period", period)
                    .AddParameterIf("@ClientID", clientId > 0, clientId)
                    .AddParameterIf("@RoomID", roomId > 0, roomId);
                DataTable dt = dba.FillDataTable("RoomData_Select");
                dt.TableName = "RoomUsage";
                return dt;
            }
        }

        //in this function, we first find all the days an event belongs to, then agg by day
        //don't need to worry about charged rooms as clean data only has charged rooms
        public DataTable AggRoomDataClean(DateTime sDate, DateTime eDate, out DataSet ds, int clientId = 0, int roomId = 0)
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("ClientID", typeof(int));
            dt.Columns.Add("RoomID", typeof(int));
            dt.Columns.Add("EntryDT", typeof(DateTime));
            dt.Columns.Add("Entries", typeof(double));
            dt.Columns.Add("Duration", typeof(double));

            DataTable dtRoomDataClean = ReadRoomDataClean(sDate, eDate, out ds, clientId, roomId);

            if (dtRoomDataClean.Rows.Count == 0) return dt;

            dtRoomDataClean.Columns.Add("eDay", typeof(int)); //used below in the compute
            dtRoomDataClean.Columns.Add("Entries", typeof(double));

            DataTable dtClient = ds.Tables[1];
            DataTable dtRoom = ds.Tables[2];

            double duration;
            DataRow ndr;

            // need to use for (not foreach) because the enumeration changes when a row is added and this causes an exception
            int rowCount = dtRoomDataClean.Rows.Count;
            for (int i = 0; i < rowCount; i++)
            {
                DataRow drRoomDataClean = dtRoomDataClean.Rows[i];
                DateTime entryDT = drRoomDataClean.Field<DateTime>("EntryDT");
                drRoomDataClean.SetField("eDay", entryDT.Day);
                drRoomDataClean.SetField("Entries", 1D);
                if (drRoomDataClean.Field<bool>("PassbackRoom"))
                {
                    DateTime exitDT = drRoomDataClean.Field<DateTime>("exitDT");

                    //note that no room event can be longer than MaxTime hours
                    if (entryDT.Day != exitDT.Day)
                    {
                        //entry date and exit date are different, we have to create a new data row
                        duration = exitDT.Subtract(entryDT).TotalHours;
                        DateTime newDT = new DateTime(exitDT.Year, exitDT.Month, exitDT.Day);

                        drRoomDataClean.SetField("ExitDT", newDT);
                        drRoomDataClean.SetField("Duration", newDT.Subtract(entryDT).TotalHours);
                        drRoomDataClean.SetField("Entries", drRoomDataClean.Field<double>("Duration") / duration);

                        ndr = dtRoomDataClean.NewRow();
                        ndr.ItemArray = drRoomDataClean.ItemArray; //start by copying everything
                        ndr.SetField("EntryDT", newDT);
                        ndr.SetField("eDay", newDT.Day);
                        ndr.SetField("ExitDT", exitDT);
                        ndr.SetField("Duration", exitDT.Subtract(newDT).TotalHours);
                        ndr.SetField("Entries", ndr.Field<double>("Duration") / duration);
                        dtRoomDataClean.Rows.Add(ndr);
                    }
                }
            }

            // delete the records that are out of bound of current month, typically it happens when user enters
            // at the last day of month and exit the first day of month etc.
            DataRow[] fdrDelete = dtRoomDataClean.Select(string.Format("EntryDT < '{0}' OR EntryDT >= '{1}'", sDate, eDate));
            foreach (DataRow dr in fdrDelete)
                dtRoomDataClean.Rows.Remove(dr); // Remove is the same as calling Delete and then AcceptChanges per the Microsoft docs

            // at this point, the client/room has proper number of records
            // so, at this time, agg by day and add records to new dt
            // TODO: make this more efficient
            object entries;
            int upperbound = eDate.Subtract(sDate).Days + 1;
            for (int i = 1; i <= upperbound; i++)
            {
                foreach (DataRow drClient in dtClient.Rows)
                {
                    foreach (DataRow drRoom in dtRoom.Rows)
                    {
                        entries = dtRoomDataClean.Compute("SUM(Entries)", string.Format("eDay = {0} AND ClientID = {1} AND RoomID = {2}", i, drClient["ClientID"], drRoom["RoomID"]));
                        if (!Utility.IsDBNullOrNull(entries))
                        {
                            duration = Convert.ToDouble(dtRoomDataClean.Compute("SUM(Duration)", string.Format("eDay = {0} AND ClientID = {1} AND RoomID = {2}", i, drClient["ClientID"], drRoom["RoomID"])));
                            ndr = dt.NewRow();
                            ndr.SetField("ClientID", drClient.Field<int>("ClientID"));
                            ndr.SetField("RoomID", drRoom.Field<int>("RoomID"));
                            ndr.SetField("EntryDT", sDate.AddDays(i - 1));
                            ndr.SetField("Entries", Convert.ToDouble(entries));
                            ndr.SetField("Duration", duration);
                            dt.Rows.Add(ndr);
                        }
                    }
                }
            }

            // these are already existing rows in RoomDataClean, there's nothing to be inserted, updated, or deleted
            // so this is just a precaution to make sure nothing happens later
            dt.AcceptChanges();

            return dt;
        }

        public DataTable ReadRoomDataForUpdate(DateTime TargetDate, out DataSet ds, int clientId = 0, int roomId = 0)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.SelectCommand
                    .AddParameter("@Action", "TargetDate")
                    .AddParameter("@TargetDate", TargetDate)
                    .AddParameterIf("@ClientID", clientId > 0, clientId)
                    .AddParameterIf("@RoomID", roomId > 0, roomId);
                ds = dba.FillDataSet("RoomDataClean_Select");
                return ds.Tables[0];
            }
        }

        public DataTable ReadRoomDataClean(DateTime sDate, DateTime eDate, out DataSet ds, int clientId = 0, int roomId = 0)
        {
            // [2016-02-01 jg] I'm using NHibernate now because there is a strange timeout issue. The logic here is identical to RoomDataClean_Select @Action = 'ByDateRange'
            //      This function gets three tables and returns the first one, and sets ds to a DataSet containing all three (not sure why we don't simply return the DataSet).

            DataTable dtRoomDataClean = new DataTable();
            dtRoomDataClean.Columns.Add("RoomDataID", typeof(int));
            dtRoomDataClean.Columns.Add("ClientID", typeof(int));
            dtRoomDataClean.Columns.Add("RoomID", typeof(int));
            dtRoomDataClean.Columns.Add("EntryDT", typeof(DateTime));
            dtRoomDataClean.Columns.Add("ExitDT", typeof(DateTime));
            dtRoomDataClean.Columns.Add("Duration", typeof(double));
            dtRoomDataClean.Columns.Add("PassbackRoom", typeof(bool));

            DataTable dtClient = new DataTable();
            dtClient.Columns.Add("ClientID", typeof(int));
            dtClient.Columns.Add("Privs", typeof(int));
            dtClient.Columns.Add("DisplayName", typeof(string));

            DataTable dtRoom = new DataTable();
            dtRoom.Columns.Add("RoomID", typeof(int));
            dtRoom.Columns.Add("Room", typeof(string));
            dtRoom.Columns.Add("PassbackRoom", typeof(bool));

            IList<RoomDataClean> baseQuery = DA.Current.Query<RoomDataClean>().Where(x => x.EntryDT < eDate && x.ExitDT > sDate).ToList();
            IList<RoomDataClean> query;

            // Clients
            if (clientId > 0)
                query = baseQuery.Where(x => x.Client.ClientID == clientId).ToList();
            else
                query = baseQuery.ToList();

            int[] cIds = query.Select(x => x.Client.ClientID).Distinct().ToArray();

            IList<Client> clients = DA.Current.Query<Client>().Where(x => cIds.Contains(x.ClientID)).ToList();

            // Rooms
            if (roomId > 0)
                query = baseQuery.Where(x => x.Room.RoomID == roomId).ToList();
            else
                query = baseQuery.ToList();

            int[] rIds = query.Select(x => x.Room.RoomID).Distinct().ToArray();

            IList<Room> rooms = DA.Current.Query<Room>().Where(x => rIds.Contains(x.RoomID)).ToList();

            // RoomDataClean
            query = baseQuery.Where(x => cIds.Contains(x.Client.ClientID) && rIds.Contains(x.Room.RoomID)).ToList();

            foreach (var rdc in query)
            {
                DataRow ndr = dtRoomDataClean.NewRow();
                ndr.SetField("RoomDataID", rdc.RoomDataID);
                ndr.SetField("ClientID", rdc.Client.ClientID);
                ndr.SetField("RoomID", rdc.Room.RoomID);
                ndr.SetField("EntryDT", rdc.EntryDT);
                ndr.SetField("ExitDT", rdc.ExitDT);
                ndr.SetField("Duration", rdc.Duration);
                ndr.SetField("PassbackRoom", rdc.Room.PassbackRoom);
                dtRoomDataClean.Rows.Add(ndr);
            }

            foreach (var c in clients)
            {
                DataRow ndr = dtClient.NewRow();
                ndr.SetField("ClientID", c.ClientID);
                ndr.SetField("Privs", (int)c.Privs);
                ndr.SetField("DisplayName", c.DisplayName);
                dtClient.Rows.Add(ndr);
            }

            foreach (var r in rooms)
            {
                DataRow ndr = dtRoom.NewRow();
                ndr.SetField("RoomID", r.RoomID);
                ndr.SetField("Room", r.RoomName);
                ndr.SetField("PassbackRoom", r.PassbackRoom);
                dtRoom.Rows.Add(ndr);
            }

            dtClient.AcceptChanges();
            dtRoom.AcceptChanges();
            dtRoomDataClean.AcceptChanges();

            ds = new DataSet();
            ds.Tables.Add(dtRoomDataClean);
            ds.Tables.Add(dtClient);
            ds.Tables.Add(dtRoom);

            return dtRoomDataClean;
        }

        //2007-11-08 Wen: It seemed this function will get raw data from ProWatch, and Client table that contains all users who had stepped into the lab
        //and all rooms that have people stepped in.  This function is directly called by WriteRoomData class
        //It will return a table that has the same schema as RoomDataClean table in database.
        //The raw data has 4 columns, ClientID, RoomID, Date, event Type (Grant In or Out or Error)
        public DataTable ReadRoomDataFiltered(DateTime sDate, DateTime eDate, int clientId = 0, int roomId = 0)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ClientID", typeof(int));
            dt.Columns.Add("RoomID", typeof(int));
            dt.Columns.Add("EntryDT", typeof(DateTime));
            dt.Columns.Add("ExitDT", typeof(DateTime));
            dt.Columns.Add("Duration", typeof(double));

            DataSet ds;
            DataTable dtRaw = ReadRoomDataRaw(sDate, eDate, out ds, clientId, roomId);
            if (dtRaw.Rows.Count == 0)
                return dt;

            DataTable dtClient = ds.Tables[1];
            DataTable dtRoom = ds.Tables[2];

            //loop through clients, extract their lab usage data, subset it and then clean
            DataRow[] fdr;
            DataTable dtClean;
            DateTime EntryDT, ExitDT;

            foreach (DataRow drClient in dtClient.Rows)
            {
                foreach (DataRow drRoom in dtRoom.Rows)
                {
                    if (Convert.ToBoolean(drRoom["IsChargedRoom"])) //only store chargeable rooms
                    {
                        fdr = dtRaw.Select(string.Format("ClientID = {0} AND RoomID = {1}", drClient["ClientID"], drRoom["RoomID"]), "evtDate");
                        if (fdr.Length > 0)
                        {
                            DataSet dsRaw = new DataSet();
                            dsRaw.Merge(fdr);

                            dtClean = DataCleaner(dsRaw.Tables[0], Convert.ToInt32(drClient["ClientID"]), Convert.ToInt32(drRoom["RoomID"]), Convert.ToBoolean(drRoom["PassbackRoom"]), sDate, eDate);

                            if (dtClean.Rows.Count > 0)
                            {
                                //loop through the data and put into the proper format
                                int stepCntr = (Convert.ToBoolean(drRoom["PassbackRoom"])) ? 2 : 1;
                                DataRow drClean0, drClean1, dr;

                                for (int i = 0; i < dtClean.Rows.Count; i = i + stepCntr)
                                {
                                    if (dtClean.Rows[i].RowState != DataRowState.Deleted)
                                    {
                                        if (Convert.ToBoolean(drRoom["PassbackRoom"]))
                                        {
                                            drClean0 = dtClean.Rows[i];
                                            drClean1 = dtClean.Rows[i + 1];
                                            EntryDT = Convert.ToDateTime(drClean0["evtDate"]);
                                            ExitDT = Convert.ToDateTime(drClean1["evtDate"]);
                                            if (ExitDT != eDate) //will filter events that get cut at the eDate boundary
                                            {
                                                dr = dt.NewRow();
                                                dr["ClientID"] = drClean0["ClientID"];
                                                dr["RoomID"] = drClean0["RoomID"];
                                                dr["EntryDT"] = EntryDT;
                                                dr["ExitDT"] = ExitDT;
                                                dr["Duration"] = ExitDT.Subtract(EntryDT).TotalHours;
                                                dt.Rows.Add(dr);
                                            }
                                        }
                                        else
                                        {
                                            //Wen: non passback room handling
                                            drClean0 = dtClean.Rows[i];
                                            EntryDT = Convert.ToDateTime(drClean0["evtDate"]);
                                            dr = dt.NewRow();
                                            dr["ClientID"] = drClean0["ClientID"];
                                            dr["RoomID"] = drClean0["RoomID"];
                                            dr["EntryDT"] = EntryDT;
                                            dr["ExitDT"] = EntryDT; //It's better to have something in ExitDT, because if it's null it's difficult to do query
                                            //Wen: should we set dr("ExitDT") here?  we have to find out later
                                            dr["Duration"] = 1; //Wen: since non antipassback room cannot have the "out" event, its duration is meaningless
                                            dt.Rows.Add(dr);
                                        }
                                    }
                                    else
                                        i -= 1;
                                }
                            }
                            dsRaw = null;
                        }
                    }
                }
            }

            //now, remove all rows that occurred in the past day - can happen if first entry is PB error
            //also remove any entries that are in the future
            fdr = dt.Select(string.Format("(EntryDT < '{0}' AND ExitDT < '{1}') OR ExitDT > '{2}'", sDate, sDate, DateTime.Now));
            for (int i = 0; i < fdr.Length; i++)
            {
                dt.Rows.Remove(fdr[i]);
            }

            //remove all rows that begin or end on either boundary and whose duration is maxtime
            fdr = dt.Select(string.Format("(EntryDT = '{0}' OR ExitDT = '{1}') AND Duration = {2}", sDate, eDate, MaxTime));
            for (int i = 0; i < fdr.Length; i++)
            {
                dt.Rows.Remove(fdr[i]);
            }

            return dt;
        }

        //2007-11-09 Wen: Clean the anitpassback error records
        private DataTable DataCleaner(DataTable dtRaw, int clientId, int roomId, bool passbackRoom, DateTime sDate, DateTime eDate)
        {
            if (passbackRoom)
            {
                //passing in ClientID and RoomID for convenience
                //dates are used to check for error conditions and to set boundaries
                return CleanPBRoomData(dtRaw, clientId, roomId, sDate, eDate);
            }
            else
                return CleanNonPBRoomData(dtRaw);
        }

        private DataTable CleanPBRoomData(DataTable dt, int clientId, int roomId, DateTime sDate, DateTime eDate)
        {
            int i = 0;
            DataRow dr;
            DateTime evStart = sDate; //if first record is out, this will start at beginning of period
            DateTime dateTime1, dateTime2;
            DateTime dtAP1, dtAPn;

            //the function newCleanRow works because
            //this function is always run one user at a time
            //room is passed in for this purpose

            //changed from < to <= to catch the case of the last row being 'wrong'
            while (i <= dt.Rows.Count - 1) //this allows checking the next row easily
            {
                switch (dt.Rows[i]["evtType"].ToString())
                {
                    case "Local Grant - IN":
                        switch (dt.Rows[i + 1]["evtType"].ToString())
                        {
                            case "Local Grant - IN":
                                //2009-11-23 The clean room double door system usually cause problems for users on multiple badge-in or badge-out instances.  The system always consider it's IN/OUT
                                //while the user is still not in/out of clean room.
                                //To prevent from overcharging the users, we set 5 minutes minimum policy - if there are two concurring INs and the time between them is less than 5 minutes,
                                //We delete that IN entry.  Same thing to OUT entry
                                DateTime dateTimeTest = Convert.ToDateTime(dt.Rows[i]["evtDate"]);
                                dateTime2 = Convert.ToDateTime(dt.Rows[i + 1]["evtDate"]).AddSeconds(-1);
                                if ((dateTime2 - dateTimeTest).Seconds > 300)
                                {
                                    dateTime1 = Convert.ToDateTime(dt.Rows[i]["evtDate"]).AddHours(MaxTime);
                                    dr = newCleanRow(dt, clientId, roomId);
                                    dr["evtType"] = "Local Grant - OUT";
                                    if (DateTime.Compare(dateTime1, dateTime2) >= 0) //t1 > t2
                                        dr["evtDate"] = dateTime2;
                                    else
                                        dr["evtDate"] = dateTime1;
                                    dt.Rows.InsertAt(dr, i + 1); //i not incremented, will become normal case
                                }
                                else
                                {
                                    //less than 5 minutes, so we delete this row
                                    dt.Rows[i].AcceptChanges();
                                    dt.Rows[i].Delete();
                                    i += 1;
                                }
                                break;
                            case "Local Grant - OUT":
                                dateTime1 = Convert.ToDateTime(dt.Rows[i]["evtDate"]);
                                dateTime2 = Convert.ToDateTime(dt.Rows[i + 1]["evtDate"]);
                                if (dateTime2.Subtract(dateTime1).TotalHours > MaxTime)
                                {
                                    dr = newCleanRow(dt, clientId, roomId);
                                    dr["evtType"] = "Local Grant - OUT";
                                    dr["evtDate"] = dateTime1.AddHours(MaxTime);
                                    dt.Rows.InsertAt(dr, i + 1); //i not incremented, will become normal case

                                    if (dateTime2.Subtract(dateTime1).TotalHours > 2 * MaxTime)
                                        dateTime2 = dateTime2.AddHours(-MaxTime);
                                    else
                                        dateTime2 = dateTime1.AddHours(MaxTime).AddSeconds(1);
                                    dr = newCleanRow(dt, clientId, roomId);
                                    dr["evtType"] = "Local Grant - IN";
                                    dr["evtDate"] = dateTime2;
                                    dt.Rows.InsertAt(dr, i + 2); //i not incremented, will become normal case
                                }
                                else
                                    i = i + 2;
                                break;
                            case "Antipassback error - IN":
                                dtAP1 = Convert.ToDateTime(dt.Rows[i + 1]["evtDate"]);
                                dtAPn = dtAP1;
                                while (dt.Rows[i + 1]["evtType"].ToString() == "Antipassback error - IN" && i + 1 < dt.Rows.Count - 1)
                                {
                                    dtAPn = Convert.ToDateTime(dt.Rows[i + 1]["evtDate"]);
                                    dt.Rows.Remove(dt.Rows[i + 1]);
                                }
                                if (dt.Rows[i + 1]["evtType"].ToString() == "Local Grant - IN")
                                {
                                    dr = newCleanRow(dt, clientId, roomId);
                                    dr["evtType"] = "Local Grant - OUT";
                                    dateTime1 = dtAP1.AddSeconds(-1);
                                    dateTime2 = Convert.ToDateTime(dt.Rows[i]["evtDate"]).AddHours(MaxTime);
                                    if (DateTime.Compare(dateTime1, dateTime2) >= 0) //t1 > t2
                                        dr["evtDate"] = dateTime2;
                                    else
                                        dr["evtDate"] = dateTime1;
                                    dt.Rows.InsertAt(dr, i + 1); //i not incremented, will become normal case
                                }
                                else
                                {
                                    dateTime1 = Convert.ToDateTime(dt.Rows[i]["evtDate"]);
                                    dateTime2 = dtAPn;
                                    TimeSpan timeDiff = dateTime2.Subtract(dateTime1);
                                    if (timeDiff.TotalSeconds > 300.0)
                                    {
                                        dr = newCleanRow(dt, clientId, roomId);
                                        dr["evtType"] = "Local Grant - OUT";
                                        dateTime1 = Convert.ToDateTime(dt.Rows[i]["evtDate"]).AddHours(MaxTime);
                                        dateTime2 = dtAP1.AddSeconds(-1);
                                        if (DateTime.Compare(dateTime1, dateTime2) >= 0) //t1 > t2
                                            dr["evtDate"] = dateTime2;
                                        else
                                            dr["evtDate"] = dateTime1;
                                        dt.Rows.InsertAt(dr, i + 1); //i not incremented, will become normal case

                                        dr = newCleanRow(dt, clientId, roomId);
                                        dr["evtType"] = "Local Grant - IN";
                                        dateTime1 = dtAPn.AddSeconds(1);
                                        dateTime2 = Convert.ToDateTime(dt.Rows[i + 1]["evtDate"]).AddHours(-MaxTime);
                                        if (DateTime.Compare(dateTime1, dateTime2) >= 0) //t1 > t2
                                            dr["evtDate"] = dateTime1;
                                        else
                                            dr["evtDate"] = dateTime2;
                                        dt.Rows.InsertAt(dr, i + 2); //i not incremented, will become normal case
                                    }
                                }
                                break;
                            case "Antipassback error - OUT":
                                dtAP1 = Convert.ToDateTime(dt.Rows[i + 1]["evtDate"]);
                                dtAPn = dtAP1;
                                while (dt.Rows[i + 1]["evtType"].ToString() == "Antipassback error - OUT" && i + 1 < dt.Rows.Count - 1)
                                {
                                    dtAPn = Convert.ToDateTime(dt.Rows[i + 1]["evtDate"]);
                                    dt.Rows.Remove(dt.Rows[i + 1]);
                                }
                                if (dt.Rows[i + 1]["evtType"].ToString() == "Local Grant - IN")
                                {
                                    dr = newCleanRow(dt, clientId, roomId);
                                    dr["evtType"] = "Local Grant - OUT";

                                    dateTime1 = dtAPn.AddSeconds(-1);
                                    dateTime2 = Convert.ToDateTime(dt.Rows[i]["evtDate"]).AddHours(MaxTime);
                                    if (DateTime.Compare(dateTime1, dateTime2) >= 0) //t1 > t2
                                        dr["evtDate"] = dateTime2;
                                    else
                                        dr["evtDate"] = dateTime1;
                                    dt.Rows.InsertAt(dr, i + 1); //i not incremented, will become normal case
                                }
                                break;
                            default:
                                dt.Rows.Remove(dt.Rows[i + 1]); //to pick up cases like "Local Grant"
                                break;
                        }
                        break;
                    case "Local Grant - OUT":
                        dateTime1 = Convert.ToDateTime(dt.Rows[i - 1]["evtDate"]).AddSeconds(1);
                        dateTime2 = Convert.ToDateTime(dt.Rows[i]["evtDate"]).AddHours(-MaxTime);
                        dr = newCleanRow(dt, clientId, roomId);
                        dr["evtType"] = "Local Grant - IN";
                        if (DateTime.Compare(dateTime1, dateTime2) >= 0) //t1 > t2
                            dr["evtDate"] = dateTime1;
                        else
                            dr["evtDate"] = dateTime2;
                        dt.Rows.InsertAt(dr, i); //i not incremented, will become normal case
                        break;
                    case "Antipassback error - IN":
                        dtAP1 = Convert.ToDateTime(dt.Rows[i]["evtDate"]);
                        while (dt.Rows[i]["evtType"].ToString() == "Antipassback error - IN" && i < dt.Rows.Count - 1)
                        {
                            dt.Rows.Remove(dt.Rows[i]);
                        }
                        if (dt.Rows[i]["evtType"].ToString() == "Local Grant - OUT") //what about ap-out?
                        {
                            dr = newCleanRow(dt, clientId, roomId);
                            dr["evtType"] = "Local Grant - IN";
                            dateTime1 = dtAP1;
                            dateTime2 = Convert.ToDateTime(dt.Rows[i]["evtDate"]).AddHours(-MaxTime);
                            if (DateTime.Compare(dateTime1, dateTime2) >= 0) //t1 > t2
                                dr["evtDate"] = dateTime1;
                            else
                                dr["evtDate"] = dateTime2;
                            dt.Rows.InsertAt(dr, i); //i not incremented, will become normal case
                        }
                        break;
                    case "Antipassback error - OUT":
                        dtAP1 = Convert.ToDateTime(dt.Rows[i - 1]["evtDate"]);
                        while (dt.Rows[i]["evtType"].ToString() == "Antipassback error - OUT" && i < dt.Rows.Count - 1)
                        {
                            dt.Rows.Remove(dt.Rows[i]);
                        }
                        if (dt.Rows[i]["evtType"].ToString() == "Local Grant - IN") //what about ap-out?
                        {
                            dtAPn = Convert.ToDateTime(dt.Rows[i]["evtDate"]);
                            TimeSpan timeDiff = dtAPn.Subtract(dtAP1);
                            if (timeDiff.TotalSeconds > 300.0)
                            {
                                dr = newCleanRow(dt, clientId, roomId);
                                dr["evtType"] = "Local Grant - IN";
                                dateTime1 = Convert.ToDateTime(dt.Rows[i - 1]["evtDate"]).AddSeconds(1); //when I left last
                                dateTime2 = dtAPn.AddHours(-MaxTime);
                                if (DateTime.Compare(dateTime1, dateTime2) >= 0) //t1 > t2
                                    dr["evtDate"] = dateTime1;
                                else
                                    dr["evtDate"] = dateTime2;
                                dt.Rows.InsertAt(dr, i); //i not incremented, will become normal case

                                dr = newCleanRow(dt, clientId, roomId);
                                dr["evtType"] = "Local Grant - OUT";
                                dr["evtDate"] = dtAPn;
                                dt.Rows.InsertAt(dr, i + 1); //i not incremented, will become normal case
                            }
                        }
                        else
                        {
                            //don't know what this means, but occured due to algorithm with Beach vendor in Jan 04
                            dr = newCleanRow(dt, clientId, roomId);
                            dr["evtType"] = "Local Grant - IN";
                            dateTime1 = dtAP1;
                            dateTime2 = Convert.ToDateTime(dt.Rows[i]["evtDate"]).AddHours(-MaxTime);
                            if (DateTime.Compare(dateTime1, dateTime2) >= 0) //t1 > t2
                                dr["evtDate"] = dateTime1;
                            else
                                dr["evtDate"] = dateTime2;
                            dt.Rows.InsertAt(dr, i); //i not incremented, will become normal case
                        }
                        break;
                    default:
                        dt.Rows.Remove(dt.Rows[i]); //to pick up cases like "Local Grant"
                        break;
                }
            }

            //if there is only a single event and it spans the entire period, treat it as bogus
            double hoursInPeriod = eDate.Subtract(sDate).TotalHours;
            if (dt.Rows.Count == 2 && (Convert.ToDateTime(dt.Rows[1]["evtDate"]).Subtract(Convert.ToDateTime(dt.Rows[0]["evtDate"]))).TotalHours == hoursInPeriod)
                dt.Rows.Clear();

            return dt;
        }

        private DataRow newCleanRow(DataTable dt, int clientId, int roomId)
        {
            DataRow dr = dt.NewRow();
            dr["ClientID"] = clientId;
            dr["RoomID"] = roomId;
            return dr;
        }

        private DataTable CleanNonPBRoomData(DataTable dt)
        {
            int i = 0;
            while (i < dt.Rows.Count)
            {
                if (dt.Rows[i]["evtType"].ToString().Contains("Antipassback"))
                    dt.Rows.Remove(dt.Rows[i]);
                else
                    i += 1;
            }
            return dt;
        }

        //used above and in mscModTIL
        //RoomID and ClientID are passed in because this can be called from an app
        //  and without these, the returned dataset is too big

        //2007-11-08 Wen: The strange thing here is this function will also set a global dataset, so the return data tables are not limited to the raw data
        //from prowatch, but also room data and client data for this month as well.
        public DataTable ReadRoomDataRaw(DateTime sDate, DateTime eDate, out DataSet ds, int clientId = 0, int roomId = 0)
        {
            //string roomName = DA.Current.Single<Room>(roomId).RoomName;
            //IEnumerable<RoomDataRaw> raw = Providers.PhysicalAccess.GetRoomData(sDate, eDate, clientId, roomName);

            using (var dba = DA.Current.GetAdapter())
            {
                dba.SelectCommand.CommandTimeout = 600;
                dba.SelectCommand
                    .AddParameter("@Action", "RoomDataRaw")
                    .AddParameter("@sDate", sDate)
                    .AddParameter("@eDate", eDate)
                    .AddParameterIf("@ClientID", clientId > 0, clientId)
                    .AddParameterIf("@RoomID", roomId > 0, roomId);
                ds = dba.FillDataSet("NexWatch_Select");
                return ds.Tables[0];
            }
        }

        public DataTable NewReadRoomDataRaw(DateTime sDate, DateTime eDate, out DataSet ds, int clientId = 0, int roomId = 0)
        {
            ds = new DataSet();
            ds.Tables.Add("raw");
            ds.Tables.Add("clients");
            ds.Tables.Add("rooms");

            List<Event> result = Providers.PhysicalAccess.GetEvents(sDate, eDate, DA.Current.Single<Client>(clientId), DA.Current.Single<Room>(roomId)).ToList();

            DataTable dtRaw = ds.Tables["raw"];
            //public string UserName { get; set; }
            //public string CardNumber { get; set; }
            //public string AreaName { get; set; }
            //public DateTime BadgeExpiration { get; set; }
            //public DateTime CardExpiration { get; set; }
            //public DateTime EventDateTime { get; set; }
            //public string EventType { get; set; }
            //public string Status { get; set; }
            dtRaw.Columns.Add("ClientID", typeof(int));

            result.ForEach(x =>
            {
                dtRaw.Rows.Add("");
            });

            var client = DA.Current.Single<Client>(clientId);

            int[] clients = result.Where(x => x.Client == client).Select(x => x.Client.ClientID).Distinct().ToArray();
            DataTable dtClients = ds.Tables["clients"];
            dtClients.Columns.Add("ClientID", typeof(int));
            foreach (int id in clients)
                dtClients.Rows.Add(id);

            var rooms = DA.Current.Query<Room>().Where(x => x.Active).ToArray()
                .Join(CostUtility.FindCosts("RoomCost", eDate), x => x.RoomID, y => y.RecordID, (x, y) => new { RoomID = x.RoomID, RoomName = x.RoomName, PassbackRoom = x.PassbackRoom, IsChargeRoom = (y.AcctPer != "None") });
            DataTable dtRooms = ds.Tables["rooms"];
            dtRooms.Columns.Add("RoomID", typeof(int));
            dtRooms.Columns.Add("RoomName", typeof(string));
            dtRooms.Columns.Add("PassbackRoom", typeof(bool));
            dtRooms.Columns.Add("IsChargeRoom", typeof(bool));
            foreach (var r in rooms)
                dtRooms.Rows.Add(r.RoomID, r.RoomName, r.PassbackRoom, r.IsChargeRoom);

            return ds.Tables["raw"];
        }
    }
}
