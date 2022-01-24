using LNF.Billing.Process;
using LNF.CommonTools;
using System;
using System.Data;
using System.Linq;

namespace LNF.Impl.Billing
{
    public class WriteRoomDataCleanConfig : RangeProcessConfig
    {
        public int RoomID { get; set; }
    }

    // [2018-09-13 jg]
    // I'm going back to using stored procedures for everything. NHibernate doesn't work very well when dealing
    // with large data sets. There were too many situations where individual records where selected inside a
    // foreach loop, or similar issues, that killed performance. This is basically an ETL process and I think
    // NHibernate is not well suited for this.

    /// <summary>
    /// This process will:
    ///     1) Select records from Prowatch in the date range to insert.
    ///     2) Delete records from RoomDataClean in the date range if delete is true.
    ///     3) Insert records from Prowatch into RoomDataClean.
    /// </summary>
    public class WriteRoomDataCleanProcess : ProcessBase<WriteRoomDataCleanResult>
    {
        public readonly double MaxTime = 8.0;

        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public int RoomID { get; set; }

        private DataSet _ds;

        public override string ProcessName => "RoomDataClean";

        protected override WriteRoomDataCleanResult CreateResult(DateTime startedAt)
        {
            return new WriteRoomDataCleanResult(startedAt)
            {
                StartDate = StartDate,
                EndDate = EndDate,
                ClientID = ClientID,
                RoomID = RoomID
            };
        }

        public WriteRoomDataCleanProcess(WriteRoomDataCleanConfig cfg) : base(cfg)
        {
            StartDate = cfg.StartDate;
            EndDate = cfg.EndDate;
            RoomID = cfg.RoomID;
        }

        public override int DeleteExisting()
        {
            //Delete the data because there are many chances that might need to re-generate the Clean table again and again
            using (var cmd = Connection.CreateCommand("dbo.RoomDataClean_Delete"))
            {
                AddParameter(cmd, "sDate", StartDate, SqlDbType.DateTime);
                AddParameter(cmd, "eDate", EndDate, SqlDbType.DateTime);
                AddParameterIf(cmd, "ClientID", ClientID > 0, ClientID, SqlDbType.Int);
                AddParameterIf(cmd, "RoomID", RoomID > 0, RoomID, SqlDbType.Int);
                AddParameter(cmd, "Context", Context, SqlDbType.NVarChar, 50);
                int result = cmd.ExecuteNonQuery();
                return result;
            }
        }

        public override DataTable Extract()
        {
            var reader = new RoomDataReader(Connection);
            _ds = reader.ReadRoomDataRaw(StartDate, EndDate, ClientID, RoomID);
            return _ds.Tables["RoomDataRaw"];
        }

        //2007-11-08 Wen: It seemed this function will get raw data from ProWatch, and Client table that contains all users who had stepped into the lab
        //and all rooms that have people stepped in.  This function is directly called by WriteRoomData class
        //It will return a table that has the same schema as RoomDataClean table in database.
        //The raw data has 4 columns, ClientID, RoomID, Date, event Type (Grant In or Out or Error)
        public override DataTable Transform(DataTable dtExtract)
        {
            DataTable dtTransform = new DataTable();
            dtTransform.Columns.Add("ClientID", typeof(int));
            dtTransform.Columns.Add("RoomID", typeof(int));
            dtTransform.Columns.Add("EntryDT", typeof(DateTime));
            dtTransform.Columns.Add("ExitDT", typeof(DateTime));
            dtTransform.Columns.Add("Duration", typeof(double));

            if (dtExtract.Rows.Count == 0)
                return dtTransform;

            DataTable dtClient = _ds.Tables["Client"];
            DataTable dtRoom = _ds.Tables["Room"];

            //loop through clients, extract their lab usage data, subset it and then clean
            DataTable dtClean;
            DateTime entryDate, exitDate;

            DataRow[] fdr;

            foreach (DataRow drClient in dtClient.Rows)
            {
                foreach (DataRow drRoom in dtRoom.Rows)
                {
                    if (drRoom.Field<bool>("IsChargedRoom")) //only store chargeable rooms
                    {
                        int cid = drClient.Field<int>("ClientID");
                        int rid = drRoom.Field<int>("RoomID");
                        bool pbr = drRoom.Field<bool>("PassbackRoom");

                        fdr = dtExtract.AsEnumerable().Where(x => x.Field<int>("ClientID") == cid && x.Field<int>("RoomID") == rid).OrderBy(x => x.Field<DateTime>("evtDate")).ToArray();

                        if (fdr.Length > 0)
                        {
                            DataSet dsRaw = new DataSet();
                            dsRaw.Merge(fdr);

                            dtClean = DataCleaner(dsRaw.Tables[0], cid, rid, pbr);

                            if (dtClean.Rows.Count > 0)
                            {
                                //loop through the data and put into the proper format
                                int step = pbr ? 2 : 1;
                                DataRow drClean0, drClean1, dr;

                                for (int i = 0; i < dtClean.Rows.Count; i += step)
                                {
                                    if (dtClean.Rows[i].RowState != DataRowState.Deleted)
                                    {
                                        if (pbr)
                                        {
                                            drClean0 = dtClean.Rows[i];
                                            drClean1 = dtClean.Rows[i + 1];
                                            entryDate = drClean0.Field<DateTime>("evtDate");
                                            exitDate = drClean1.Field<DateTime>("evtDate");

                                            if (exitDate != EndDate) //will filter events that get cut at the eDate boundary
                                            {
                                                dr = dtTransform.NewRow();
                                                dr.SetField("ClientID", drClean0.Field<int>("ClientID"));
                                                dr.SetField("RoomID", drClean0.Field<int>("RoomID"));
                                                dr.SetField("EntryDT", entryDate);
                                                dr.SetField("ExitDT", exitDate);
                                                dr.SetField("Duration", exitDate.Subtract(entryDate).TotalHours);
                                                dtTransform.Rows.Add(dr);
                                            }
                                        }
                                        else
                                        {
                                            //Wen: non passback room handling
                                            drClean0 = dtClean.Rows[i];
                                            entryDate = drClean0.Field<DateTime>("evtDate");
                                            dr = dtTransform.NewRow();
                                            dr.SetField("ClientID", drClean0.Field<int>("ClientID"));
                                            dr.SetField("RoomID", drClean0.Field<int>("RoomID"));
                                            dr.SetField("EntryDT", entryDate);
                                            dr.SetField("ExitDT", entryDate); //It's better to have something in ExitDT, because if it's null it's difficult to do query
                                            dr.SetField("Duration", 1); //Wen: since non antipassback room cannot have the "out" event, its duration is meaningless
                                            dtTransform.Rows.Add(dr);
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
            fdr = dtTransform.Select($"(EntryDT < #{StartDate:yyyy-MM-dd HH:mm:ss}# AND ExitDT < #{StartDate:yyyy-MM-dd HH:mm:ss}#) OR ExitDT > #{DateTime.Now:yyyy-MM-dd HH:mm:ss}#");
            for (int i = 0; i < fdr.Length; i++)
            {
                dtTransform.Rows.Remove(fdr[i]);
            }

            //remove all rows that begin or end on either boundary and whose duration is maxtime
            fdr = dtTransform.Select($"(EntryDT = #{StartDate:yyyy-MM-dd HH:mm:ss}# OR ExitDT = #{EndDate:yyyy-MM-dd HH:mm:ss}#) AND Duration = {MaxTime}");
            for (int i = 0; i < fdr.Length; i++)
            {
                dtTransform.Rows.Remove(fdr[i]);
            }

            return dtTransform;
        }

        public override LNF.DataAccess.IBulkCopy CreateBulkCopy()
        {
            var bcp = new DefaultBulkCopy("dbo.RoomDataClean");
            bcp.AddColumnMapping("ClientID");
            bcp.AddColumnMapping("RoomID");
            bcp.AddColumnMapping("EntryDT");
            bcp.AddColumnMapping("ExitDT");
            bcp.AddColumnMapping("Duration");
            return bcp;
        }

        //2007-11-09 Wen: Clean the anitpassback error records
        private DataTable DataCleaner(DataTable dt, int clientId, int roomId, bool passbackRoom)
        {
            if (passbackRoom)
            {
                //passing in ClientID and RoomID for convenience
                //dates are used to check for error conditions and to set boundaries
                return CleanPassbackRoomData(dt, clientId, roomId);
            }
            else
                return CleanNonPassbackRoomData(dt);
        }

        private DataTable CleanPassbackRoomData(DataTable dt, int clientId, int roomId)
        {
            int i = 0;
            DataRow dr;
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
                                DateTime dateTimeTest = dt.Rows[i].Field<DateTime>("evtDate");
                                dateTime2 = dt.Rows[i + 1].Field<DateTime>("evtDate").AddSeconds(-1);
                                if ((dateTime2 - dateTimeTest).Seconds > 300)
                                {
                                    dateTime1 = dt.Rows[i].Field<DateTime>("evtDate").AddHours(MaxTime);
                                    dr = NewCleanRow(dt, clientId, roomId);
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
                                dateTime1 = dt.Rows[i].Field<DateTime>("evtDate");
                                dateTime2 = dt.Rows[i + 1].Field<DateTime>("evtDate");
                                if (dateTime2.Subtract(dateTime1).TotalHours > MaxTime)
                                {
                                    dr = NewCleanRow(dt, clientId, roomId);
                                    dr["evtType"] = "Local Grant - OUT";
                                    dr["evtDate"] = dateTime1.AddHours(MaxTime);
                                    dt.Rows.InsertAt(dr, i + 1); //i not incremented, will become normal case

                                    if (dateTime2.Subtract(dateTime1).TotalHours > 2 * MaxTime)
                                        dateTime2 = dateTime2.AddHours(-MaxTime);
                                    else
                                        dateTime2 = dateTime1.AddHours(MaxTime).AddSeconds(1);
                                    dr = NewCleanRow(dt, clientId, roomId);
                                    dr["evtType"] = "Local Grant - IN";
                                    dr["evtDate"] = dateTime2;
                                    dt.Rows.InsertAt(dr, i + 2); //i not incremented, will become normal case
                                }
                                else
                                    i += 2;
                                break;
                            case "Antipassback error - IN":
                                dtAP1 = dt.Rows[i + 1].Field<DateTime>("evtDate");
                                dtAPn = dtAP1;
                                while (dt.Rows[i + 1]["evtType"].ToString() == "Antipassback error - IN" && i + 1 < dt.Rows.Count - 1)
                                {
                                    dtAPn = dt.Rows[i + 1].Field<DateTime>("evtDate");
                                    dt.Rows.Remove(dt.Rows[i + 1]);
                                }
                                if (dt.Rows[i + 1]["evtType"].ToString() == "Local Grant - IN")
                                {
                                    dr = NewCleanRow(dt, clientId, roomId);
                                    dr["evtType"] = "Local Grant - OUT";
                                    dateTime1 = dtAP1.AddSeconds(-1);
                                    dateTime2 = dt.Rows[i].Field<DateTime>("evtDate").AddHours(MaxTime);
                                    if (DateTime.Compare(dateTime1, dateTime2) >= 0) //t1 > t2
                                        dr["evtDate"] = dateTime2;
                                    else
                                        dr["evtDate"] = dateTime1;
                                    dt.Rows.InsertAt(dr, i + 1); //i not incremented, will become normal case
                                }
                                else
                                {
                                    dateTime1 = dt.Rows[i].Field<DateTime>("evtDate");
                                    dateTime2 = dtAPn;
                                    TimeSpan timeDiff = dateTime2.Subtract(dateTime1);
                                    if (timeDiff.TotalSeconds > 300.0)
                                    {
                                        dr = NewCleanRow(dt, clientId, roomId);
                                        dr["evtType"] = "Local Grant - OUT";
                                        dateTime1 = dt.Rows[i].Field<DateTime>("evtDate").AddHours(MaxTime);
                                        dateTime2 = dtAP1.AddSeconds(-1);
                                        if (DateTime.Compare(dateTime1, dateTime2) >= 0) //t1 > t2
                                            dr["evtDate"] = dateTime2;
                                        else
                                            dr["evtDate"] = dateTime1;
                                        dt.Rows.InsertAt(dr, i + 1); //i not incremented, will become normal case

                                        dr = NewCleanRow(dt, clientId, roomId);
                                        dr["evtType"] = "Local Grant - IN";
                                        dateTime1 = dtAPn.AddSeconds(1);
                                        dateTime2 = dt.Rows[i + 1].Field<DateTime>("evtDate").AddHours(-MaxTime);
                                        if (DateTime.Compare(dateTime1, dateTime2) >= 0) //t1 > t2
                                            dr["evtDate"] = dateTime1;
                                        else
                                            dr["evtDate"] = dateTime2;
                                        dt.Rows.InsertAt(dr, i + 2); //i not incremented, will become normal case
                                    }
                                }
                                break;
                            case "Antipassback error - OUT":
                                dtAP1 = dt.Rows[i + 1].Field<DateTime>("evtDate");
                                dtAPn = dtAP1;
                                string eventType = dt.Rows[i + 1]["evtType"].ToString();
                                while (eventType == "Antipassback error - OUT" && i + 1 < dt.Rows.Count - 1)
                                {
                                    dtAPn = dt.Rows[i + 1].Field<DateTime>("evtDate");
                                    dt.Rows.Remove(dt.Rows[i + 1]);
                                }
                                if (dt.Rows[i + 1]["evtType"].ToString() == "Local Grant - IN")
                                {
                                    dr = NewCleanRow(dt, clientId, roomId);
                                    dr["evtType"] = "Local Grant - OUT";

                                    dateTime1 = dtAPn.AddSeconds(-1);
                                    dateTime2 = dt.Rows[i].Field<DateTime>("evtDate").AddHours(MaxTime);
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
                        dateTime1 = dt.Rows[i - 1].Field<DateTime>("evtDate").AddSeconds(1);
                        dateTime2 = dt.Rows[i].Field<DateTime>("evtDate").AddHours(-MaxTime);
                        dr = NewCleanRow(dt, clientId, roomId);
                        dr["evtType"] = "Local Grant - IN";
                        if (DateTime.Compare(dateTime1, dateTime2) >= 0) //t1 > t2
                            dr["evtDate"] = dateTime1;
                        else
                            dr["evtDate"] = dateTime2;
                        dt.Rows.InsertAt(dr, i); //i not incremented, will become normal case
                        break;
                    case "Antipassback error - IN":
                        dtAP1 = dt.Rows[i].Field<DateTime>("evtDate");
                        while (dt.Rows[i]["evtType"].ToString() == "Antipassback error - IN" && i < dt.Rows.Count - 1)
                        {
                            dt.Rows.Remove(dt.Rows[i]);
                        }
                        if (dt.Rows[i]["evtType"].ToString() == "Local Grant - OUT") //what about ap-out?
                        {
                            dr = NewCleanRow(dt, clientId, roomId);
                            dr["evtType"] = "Local Grant - IN";
                            dateTime1 = dtAP1;
                            dateTime2 = dt.Rows[i].Field<DateTime>("evtDate").AddHours(-MaxTime);
                            if (DateTime.Compare(dateTime1, dateTime2) >= 0) //t1 > t2
                                dr["evtDate"] = dateTime1;
                            else
                                dr["evtDate"] = dateTime2;
                            dt.Rows.InsertAt(dr, i); //i not incremented, will become normal case
                        }
                        break;
                    case "Antipassback error - OUT":
                        dtAP1 = dt.Rows[i - 1].Field<DateTime>("evtDate");
                        while (dt.Rows[i]["evtType"].ToString() == "Antipassback error - OUT" && i < dt.Rows.Count - 1)
                        {
                            dt.Rows.Remove(dt.Rows[i]);
                        }
                        if (dt.Rows[i]["evtType"].ToString() == "Local Grant - IN") //what about ap-out?
                        {
                            dtAPn = dt.Rows[i].Field<DateTime>("evtDate");
                            TimeSpan timeDiff = dtAPn.Subtract(dtAP1);
                            if (timeDiff.TotalSeconds > 300.0)
                            {
                                dr = NewCleanRow(dt, clientId, roomId);
                                dr["evtType"] = "Local Grant - IN";
                                dateTime1 = dt.Rows[i - 1].Field<DateTime>("evtDate").AddSeconds(1); //when I left last
                                dateTime2 = dtAPn.AddHours(-MaxTime);
                                if (DateTime.Compare(dateTime1, dateTime2) >= 0) //t1 > t2
                                    dr["evtDate"] = dateTime1;
                                else
                                    dr["evtDate"] = dateTime2;
                                dt.Rows.InsertAt(dr, i); //i not incremented, will become normal case

                                dr = NewCleanRow(dt, clientId, roomId);
                                dr["evtType"] = "Local Grant - OUT";
                                dr["evtDate"] = dtAPn;
                                dt.Rows.InsertAt(dr, i + 1); //i not incremented, will become normal case
                            }
                        }
                        else
                        {
                            //don't know what this means, but occured due to algorithm with Beach vendor in Jan 04
                            dr = NewCleanRow(dt, clientId, roomId);
                            dr["evtType"] = "Local Grant - IN";
                            dateTime1 = dtAP1;
                            dateTime2 = dt.Rows[i].Field<DateTime>("evtDate").AddHours(-MaxTime);
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
            if (dt.Rows.Count == 2)
            {
                if (dt.Rows[0].RowState != DataRowState.Deleted && dt.Rows[1].RowState != DataRowState.Deleted)
                {
                    double hoursInPeriod = EndDate.Subtract(StartDate).TotalHours;
                    DateTime eventDate0 = dt.Rows[0].Field<DateTime>("evtDate");
                    DateTime eventDate1 = dt.Rows[1].Field<DateTime>("evtDate");

                    if ((eventDate1.Subtract(eventDate0)).TotalHours == hoursInPeriod)
                        dt.Rows.Clear();
                }
            }

            return dt;
        }

        private DataTable CleanNonPassbackRoomData(DataTable dt)
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

        private DataRow NewCleanRow(DataTable dt, int clientId, int roomId)
        {
            DataRow dr = dt.NewRow();
            dr["ClientID"] = clientId;
            dr["RoomID"] = roomId;
            return dr;
        }
    }
}
