using LNF.Billing;
using LNF.Billing.Process;
using LNF.CommonTools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LNF.Impl.Billing
{
    public class WriteToolDataConfig : PeriodProcessConfig
    {
        public int ResourceID { get; set; }
    }

    /// <summary>
    /// This process will:
    ///     1) Delete records from ToolData in the date range.
    ///     2) Select records from ToolDataClean in the date range to insert.
    ///     3) Insert records from ToolDataClean records into ToolData.
    ///     4) Adjust records in ToolData.
    /// </summary>
    public class WriteToolDataProcess : PeriodProcessBase<WriteToolDataResult>
    {
        public static readonly DateTime NewBillingDate = new DateTime(2011, 4, 1);

        //This list of reservations were cancelled before 2011-04-01 so they should only be charged with booking fees
        //Delete this ONLY when there is no need to regenerate the April 2011 data
        public static readonly List<int> CancelledReservation20110401 = new List<int>(new int[] { 286867, 302663, 302703, 303807, 303818, 303856, 302876, 303041, 303155, 303156, 303168, 303288, 303444, 303534, 303556, 303697, 303735, 303787 });

        private readonly WriteToolDataConfig _config;

        public int ResourceID => _config.ResourceID;

        private DataSet _ds = null;
        private DataTable _activities = null;
        private int _rowsAdjusted;

        public WriteToolDataProcess(WriteToolDataConfig cfg) : base(cfg)
        {
            _config = cfg;
        }

        public override string ProcessName => "ToolData";

        protected override WriteToolDataResult CreateResult(DateTime startedAt)
        {
            return new WriteToolDataResult(startedAt)
            {
                Period = Period,
                ClientID = ClientID,
                ResourceID = ResourceID
            };
        }

        protected override void FinalizeResult(WriteToolDataResult result)
        {
            result.RowsAdjusted = _rowsAdjusted;
        }

        public override int DeleteExisting()
        {
            using (var cmd = Connection.CreateCommand("dbo.ToolData_Delete"))
            {
                AddParameter(cmd, "Period", Period, SqlDbType.DateTime);
                AddParameterIf(cmd, "ClientID", ClientID > 0, ClientID, SqlDbType.Int);
                AddParameterIf(cmd, "ResourceID", ResourceID > 0, ResourceID, SqlDbType.Int);
                AddParameter(cmd, "Context", _config.Context, SqlDbType.NVarChar, 50);

                var result = cmd.ExecuteNonQuery();

                return result;
            }
        }

        public override DataTable Extract()
        {
            DateTime sd = Period;
            DateTime ed = Period.AddMonths(1);

            // Data for all clients must be selected for transferred duration calculations so always pass 0 for clientId
            var reader = new ToolDataReader(Connection);
            _ds = reader.ReadToolDataClean(sd, ed, 0, ResourceID);

            return _ds.Tables["ToolDataClean"];
        }

        public override DataTable Transform(DataTable dtExtract)
        {
            DataTable dtTransform;

            if (Period < NewBillingDate)
                dtTransform = DailyToolDataOld(dtExtract);
            else
                dtTransform = DailyToolData20110401(dtExtract);

            return dtTransform;
        }

        public override int Load(DataTable dtTransform)
        {
            using (var cmd = Connection.CreateCommand("dbo.ToolData_Insert"))
            using (var adap = new SqlDataAdapter { InsertCommand = cmd })
            {
                cmd.Parameters.Add("Period", SqlDbType.DateTime, 0, "Period");
                cmd.Parameters.Add("ReservationID", SqlDbType.Int, 0, "ReservationID");
                cmd.Parameters.Add("ClientID", SqlDbType.Int, 0, "ClientID");
                cmd.Parameters.Add("ResourceID", SqlDbType.Int, 0, "ResourceID");
                cmd.Parameters.Add("RoomID", SqlDbType.Int, 0, "RoomID");
                cmd.Parameters.Add("ActDate", SqlDbType.DateTime, 0, "ActDate");
                cmd.Parameters.Add("AccountID", SqlDbType.Int, 0, "AccountID");
                cmd.Parameters.Add("Uses", SqlDbType.Float, 0, "Uses");
                cmd.Parameters.Add("SchedDuration", SqlDbType.Float, 0, "SchedDuration");
                cmd.Parameters.Add("ActDuration", SqlDbType.Float, 0, "ActDuration");
                cmd.Parameters.Add("ChargeDuration", SqlDbType.Float, 0, "ChargeDuration");
                cmd.Parameters.Add("TransferredDuration", SqlDbType.Float, 0, "TransferredDuration");
                cmd.Parameters.Add("MaxReservedDuration", SqlDbType.Float, 0, "MaxReservedDuration");
                cmd.Parameters.Add("OverTime", SqlDbType.Float, 0, "OverTime");
                cmd.Parameters.Add("IsStarted", SqlDbType.Bit, 0, "IsStarted");
                cmd.Parameters.Add("IsActive", SqlDbType.Bit, 0, "IsActive");
                cmd.Parameters.Add("ChargeMultiplier", SqlDbType.Float, 0, "ChargeMultiplier");
                cmd.Parameters.Add("IsCancelledBeforeAllowedTime", SqlDbType.Bit, 0, "IsCancelledBeforeAllowedTime");
                cmd.Parameters.Add("ChargeBeginDateTime", SqlDbType.DateTime, 0, "ChargeBeginDateTime");
                cmd.Parameters.Add("ChargeEndDateTime", SqlDbType.DateTime, 0, "ChargeEndDateTime");

                int result = adap.Update(dtTransform);

                _rowsAdjusted = ToolDataAdjust();

                return result;
            }
        }

        private DataTable DailyToolDataOld(DataTable dtToolDataClean)
        {
            DataTable dtToolData = CreateToolDataTable();

            //the select statement pulls in all reservations that overlap the period
            //only write those whose ActDate is within the period

            if (dtToolDataClean.Rows.Count > 0)
            {
                dtToolDataClean.Columns.Add("Uses", typeof(double));
                dtToolDataClean.Columns.Add("ChargeDuration", typeof(double));
                dtToolDataClean.Columns.Add("ChargeBeginDateTime", typeof(DateTime));
                dtToolDataClean.Columns.Add("ChargeEndDateTime", typeof(DateTime));
                dtToolDataClean.Columns.Add("IsCancelledBeforeAllowedTime", typeof(bool));

                //handled started separately from unstarted
                int toolDataCleanRowCount = dtToolDataClean.Rows.Count;
                for (int i = 0; i < toolDataCleanRowCount; i++)
                {
                    DataRow dr = dtToolDataClean.Rows[i];

                    // ClientID check. dtToolDataClean contains data for all clients even when ClientID > 0 because of transferred duration calculation.
                    if (ClientID == 0 || ClientID == dr.Field<int>("ClientID"))
                    {
                        //only chargeable activities get written to Tooldata
                        if (IsChargeable(dr))
                        {
                            //this means reservation was started
                            double schedDuration = dr.Field<double>("SchedDuration");
                            double actDuration = dr.Field<double>("ActDuration");
                            DateTime beginDateTime = dr.Field<DateTime>("BeginDateTime");
                            DateTime endDateTime = beginDateTime.AddMinutes(schedDuration);
                            DateTime actualBeginDateTime = dr.Field<DateTime>("ActualBeginDateTime");
                            DateTime actualEndDateTime = dr.Field<DateTime>("ActualEndDateTime");

                            dr["IsCancelledBeforeAllowedTime"] = false; // for the old billing method only active reservations were selected so none were cancelled

                            bool isStarted = dr.Field<bool>("IsStarted");
                            DateTime begDate = isStarted ? actualBeginDateTime : beginDateTime;
                            DateTime finDate = isStarted ? actualEndDateTime : endDateTime;

                            var chargeBeginDateTime = begDate;
                            var chargeEndDateTime = finDate;
                            var chargeDuration = (chargeEndDateTime - chargeBeginDateTime).TotalMinutes;
                            dr["ChargeBeginDateTime"] = begDate;
                            dr["ChargeEndDateTime"] = finDate;
                            dr["ChargeDuration"] = chargeDuration;

                            begDate = begDate.Date;
                            finDate = finDate.Date;

                            if (begDate != finDate)
                            {
                                //handle case where resrevation starts and ends on different days
                                double runningOverTime = 0;

                                for (int j = 0; j <= finDate.Subtract(begDate).Days; j++)
                                {
                                    DateTime bDate = begDate.AddDays(j);
                                    DateTime fDate = begDate.AddDays(j + 1);

                                    DataRow adr = dtToolDataClean.NewRow();
                                    adr.ItemArray = dr.ItemArray; //start by copying everything

                                    if (Convert.ToBoolean(dr["IsStarted"]))
                                    {
                                        adr["ActualBeginDateTime"] = (bDate > actualBeginDateTime) ? bDate : actualBeginDateTime;
                                        adr["ActualEndDateTime"] = (fDate < actualEndDateTime) ? fDate : actualEndDateTime;
                                        adr["BeginDateTime"] = adr["ActualBeginDateTime"]; //for convenience below when writing new table
                                        adr["ActDuration"] = adr.Field<DateTime>("ActualEndDateTime").Subtract(adr.Field<DateTime>("ActualBeginDateTime")).TotalMinutes;
                                        adr["Uses"] = adr.Field<double>("ActDuration") / actDuration;

                                        if (adr.Field<DateTime>("ActualEndDateTime") > endDateTime)
                                        {
                                            adr["OverTime"] = adr.Field<DateTime>("ActualEndDateTime").Subtract(endDateTime).TotalMinutes - runningOverTime;
                                            runningOverTime += adr.Field<double>("OverTime");
                                        }
                                        else
                                            adr["OverTime"] = 0D;
                                    }
                                    else
                                    {
                                        adr["BeginDateTime"] = (bDate > beginDateTime) ? bDate : beginDateTime;
                                        adr["EndDateTime"] = (fDate < endDateTime) ? fDate : endDateTime;
                                        adr["SchedDuration"] = adr.Field<DateTime>("EndDateTime").Subtract(adr.Field<DateTime>("BeginDateTime")).TotalMinutes;
                                        adr["ActDuration"] = adr["SchedDuration"]; //should be 0, but the adjust SP would be way too complex
                                        adr["Uses"] = adr.Field<double>("SchedDuration") / schedDuration;
                                    }

                                    adr["ChargeBeginDateTime"] = (bDate > chargeBeginDateTime) ? bDate : chargeBeginDateTime;
                                    adr["ChargeEndDateTime"] = (fDate < chargeEndDateTime) ? fDate : chargeEndDateTime;
                                    adr["ChargeDuration"] = (adr.Field<DateTime>("ChargeEndDateTime") - adr.Field<DateTime>("ChargeBeginDateTime")).TotalMinutes;
                                    adr["MaxReservedDuration"] = GetMaxReservedDuration(adr);

                                    dtToolDataClean.Rows.Add(adr); //will add to end
                                }
                                dr.Delete(); //remove original, multi-day row
                            }
                            else
                            {
                                dr["Uses"] = 1D;
                                if (!dr.Field<bool>("IsStarted"))
                                    dr["ActDuration"] = dr["SchedDuration"];
                                dr["MaxReservedDuration"] = GetMaxReservedDuration(dr);
                            }
                        }
                        else
                            dr.Delete(); //not chargeable, so remove
                    }
                }

                foreach (DataRow dr in dtToolDataClean.Rows)
                {
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        // ClientID check. dtToolDataClean contains data for all clients even when ClientID > 0 because of transferred duration calculation.
                        int clientId = dr.Field<int>("ClientID");
                        if (ClientID == 0 || ClientID == clientId)
                        {
                            DateTime actDate = dr.Field<DateTime>("BeginDateTime").Date;
                            if ((actDate >= Period && actDate < Period.AddMonths(1)) && dr.Field<double>("SchedDuration") != 0D)
                            {
                                DataRow ndr = dtToolData.NewRow();
                                ndr.SetField("Period", new DateTime(actDate.Year, actDate.Month, 1));
                                ndr.SetField("ClientID", clientId);
                                ndr.SetField("ResourceID", dr.Field<int>("ResourceID"));
                                ndr.SetField("RoomID", dr.Field<int>("RoomID"));
                                ndr.SetField("ActDate", actDate);
                                ndr.SetField("AccountID", dr.Field<int>("AccountID"));
                                ndr.SetField("Uses", dr.Field<double>("Uses"));
                                ndr.SetField("SchedDuration", dr.Field<double>("SchedDuration"));
                                ndr.SetField("ActDuration", dr.Field<double>("ActDuration"));
                                ndr.SetField("Overtime", dr.Field<double>("Overtime"));
                                ndr.SetField("Days", 0D); //calculated in ToolDataAdjust
                                ndr.SetField("Months", 0D); //calulated in ToolDataAdjust
                                ndr.SetField("IsStarted", dr.Field<bool>("IsStarted"));
                                ndr.SetField("ChargeMultiplier", dr.Field<double>("ChargeMultiplier"));
                                ndr.SetField("ReservationID", dr.Field<int>("ReservationID"));
                                ndr.SetField("ChargeDuration", dr.Field<double>("ChargeDuration"));
                                ndr.SetField("TransferredDuration", 0D); //not applicable in old billing model
                                ndr.SetField("MaxReservedDuration", dr.Field<double>("MaxReservedDuration"));
                                ndr.SetField("ChargeBeginDateTime", dr.Field<DateTime>("ChargeBeginDateTime"));
                                ndr.SetField("ChargeEndDateTime", dr.Field<DateTime>("ChargeEndDateTime"));
                                ndr.SetField("IsActive", dr.Field<bool>("IsActive"));
                                ndr.SetField("IsCancelledBeforeAllowedTime", dr.Field<bool>("IsCancelledBeforeAllowedTime"));
                                dtToolData.Rows.Add(ndr);
                            }
                        }
                    }
                }
            }

            return dtToolData;
        }

        //This handles generation of toolData table since 2011-04-01
        public DataTable DailyToolData20110401(DataTable dtToolDataClean)
        {
            DataTable dtOutput = CreateToolDataTable();

            //the select statement pulls in all reservations that overlap the period
            //only write those whose ActDate is within the period

            if (dtToolDataClean.Rows.Count == 0)
                return dtOutput;

            // must be done before ProcessCleanData to avoid issue with deleted DataRows
            var durations = GetReservationDurations(dtToolDataClean);

            ProcessCleanData(dtToolDataClean);
            CalculateTransferTime(dtToolDataClean, durations);
            FillOutputTable(dtOutput, dtToolDataClean);

            return dtOutput;
        }

        public ReservationDurations GetReservationDurations(DataTable dtToolDataClean)
        {
            var reservations = GetReservationDateRangeItems(dtToolDataClean);
            var range = new ReservationDateRange(reservations);
            var result = new ReservationDurations(range);
            return result;
        }

        private DataTable CreateToolDataTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Period", typeof(DateTime));
            dt.Columns.Add("ClientID", typeof(int));
            dt.Columns.Add("ResourceID", typeof(int));
            dt.Columns.Add("RoomID", typeof(int));
            dt.Columns.Add("ActDate", typeof(DateTime));
            dt.Columns.Add("AccountID", typeof(int));
            dt.Columns.Add("Uses", typeof(double));
            dt.Columns.Add("SchedDuration", typeof(double));
            dt.Columns.Add("ActDuration", typeof(double));
            dt.Columns.Add("Overtime", typeof(double));
            dt.Columns.Add("Days", typeof(double));
            dt.Columns.Add("Months", typeof(double));
            dt.Columns.Add("IsStarted", typeof(bool));
            dt.Columns.Add("ChargeMultiplier", typeof(double));
            dt.Columns.Add("ReservationID", typeof(int)); //2010-05 we need this to forgive charge on tools
            dt.Columns.Add("ChargeDuration", typeof(double));
            dt.Columns.Add("TransferredDuration", typeof(double));
            dt.Columns.Add("MaxReservedDuration", typeof(double));
            dt.Columns.Add("ChargeBeginDateTime", typeof(DateTime));
            dt.Columns.Add("ChargeEndDateTime", typeof(DateTime));
            dt.Columns.Add("IsActive", typeof(bool));
            dt.Columns.Add("IsCancelledBeforeAllowedTime", typeof(bool));

            return dt;
        }

        public void ProcessCleanData(DataTable dtToolDataClean)
        {
            dtToolDataClean.Columns.Add("Uses", typeof(double));
            dtToolDataClean.Columns.Add("TransferredDuration", typeof(double));
            dtToolDataClean.Columns.Add("IsCancelledBeforeAllowedTime", typeof(bool));
            dtToolDataClean.Columns.Add("ChargeBeginDateTime", typeof(DateTime));
            dtToolDataClean.Columns.Add("ChargeEndDateTime", typeof(DateTime));
            dtToolDataClean.Columns.Add("ChargeDuration", typeof(double));

            //handled started separately from unstarted
            double schedDuration, chargeDuration;
            DateTime beginDateTime, endDateTime, actualBeginDateTime, actualEndDateTime, chargeBeginDateTime, chargeEndDateTime;
            int toolDataCleanRowCount = dtToolDataClean.Rows.Count;

            //2011-12-05 Get ReservationHistory so we can calculate the max charge time
            DataTable dtResHistory = GetReservationHistory();

            for (int i = 0; i < toolDataCleanRowCount; i++)
            {
                DataRow dr = dtToolDataClean.Rows[i];

                // ClientID check. dtToolDataClean contains data for all clients even when ClientID > 0 because of transferred duration calculation.
                if (ClientID == 0 || ClientID == dr.Field<int>("ClientID"))
                {
                    //only chargeable activities get written to Tooldata
                    if (IsChargeable(dr))
                    {
                        //this means reservation was started
                        schedDuration = dr.Field<double>("SchedDuration");
                        beginDateTime = dr.Field<DateTime>("BeginDateTime");
                        endDateTime = beginDateTime.AddMinutes(schedDuration); //to prevent auto end's modified end time, which is wrong
                        actualBeginDateTime = dr.Field<DateTime>("ActualBeginDateTime");
                        actualEndDateTime = dr.Field<DateTime>("ActualEndDateTime");

                        dr["IsCancelledBeforeAllowedTime"] = IsCancelledBeforeAllowedTime(dr);

                        DateTime begDate = (beginDateTime > actualBeginDateTime) ? actualBeginDateTime : beginDateTime;
                        DateTime finDate = (endDateTime > actualEndDateTime) ? endDateTime : actualEndDateTime;

                        chargeBeginDateTime = begDate;
                        chargeEndDateTime = finDate;
                        chargeDuration = (chargeEndDateTime - chargeBeginDateTime).TotalMinutes;
                        dr["ChargeBeginDateTime"] = begDate;
                        dr["ChargeEndDateTime"] = finDate;
                        dr["ChargeDuration"] = chargeDuration;

                        begDate = begDate.Date;

                        //if the finDate is right on the edge, eg. the next day at 00:00am, we don't need to create a new row, because it's not crossing day
                        if (finDate == new DateTime(finDate.Year, finDate.Month, finDate.Day, 0, 0, 0))
                            finDate = finDate.AddSeconds(-1);

                        finDate = finDate.Date;

                        if (begDate != finDate)
                        {
                            //handle case where reservation starts and ends on different days
                            double runningOverTime = 0D;
                            int loopDays = finDate.Subtract(begDate).Days;
                            for (int j = 0; j <= loopDays; j++)
                            {
                                DateTime bDate = begDate.AddDays(j);
                                DateTime fDate = begDate.AddDays(j + 1);

                                DataRow adr = dtToolDataClean.NewRow();
                                adr.ItemArray = dr.ItemArray; //start by copying everything

                                adr["ChargeBeginDateTime"] = (bDate > chargeBeginDateTime) ? bDate : chargeBeginDateTime;
                                adr["ChargeEndDateTime"] = (fDate < chargeEndDateTime) ? fDate : chargeEndDateTime;
                                adr["ChargeDuration"] = (adr.Field<DateTime>("ChargeEndDateTime") - adr.Field<DateTime>("ChargeBeginDateTime")).TotalMinutes;
                                adr["Uses"] = adr.Field<double>("ChargeDuration") / chargeDuration;
                                adr["MaxReservedDuration"] = GetMaxReservedDuration(adr);

                                ////2011-11-08 we have to see of MaxReservedDuration is greater than the ChargeDuration - preventing cases when user shortened the reservation after the 2 hours allowed time
                                //if (!Convert.ToBoolean(adr["IsCancelledBeforeAllowedTime"]))
                                //{
                                //    if (Convert.ToDouble(adr["MaxReservedDuration"]) > Convert.ToDouble(adr["ChargeDuration"]))
                                //        adr["ChargeDuration"] = adr["MaxReservedDuration"];
                                //}

                                //For backward compatibilty reason, I have to repopulate the correct ActDuration as well
                                adr["ActualBeginDateTime"] = (bDate > actualBeginDateTime) ? bDate : actualBeginDateTime;
                                adr["ActualEndDateTime"] = (fDate < actualEndDateTime) ? fDate : actualEndDateTime;

                                var fixDate = new DateTime(2012, 12, 1);

                                if (Period >= fixDate)
                                {
                                    /*
                                    [jg 2013-01-24]
                                    Only do this for periods after 2012-12-01. Before that time the next line of code was missing which caused the 2nd part of a split
                                    reservation to not be written to the database. In other words the part that happened on the first day of the period. Note: this is
                                    only for reservations that cross over two periods. For periods before Dec 2012 we are purposefully doing this wrong so that amounts
                                    billed previously aren't changed retroactively.

                                    [jg 2020-02-04]
                                    Note that only ChargeBeginDateTime ChargeEndDateTime, ChargeDuration, ActDuration, and SchedDuration are in ToolData.
                                    Other columns like BeginDateTime, ActualBeginDateTime, etc are set here only because they're used in calculations or
                                    checks elsewhere.
                                    */
                                    adr["BeginDateTime"] = adr["ActualBeginDateTime"]; //for convenience below when writing new table
                                }

                                // ActDuration will only be for this part of a multi-part reservation because the actuals were modified above
                                adr["ActDuration"] = adr.Field<DateTime>("ActualEndDateTime").Subtract(adr.Field<DateTime>("ActualBeginDateTime")).TotalMinutes;

                                // make sure ActDuration is greater than 0, because if a user finish this reservation on the same day (a cross day reservation originally),
                                // it's possible to have negative ActDuration
                                if (adr.Field<double>("ActDuration") < 0D)
                                    adr["ActDuration"] = 0D; //since user finished on the previous day, today's reservation actual duration is zero

                                if (adr.Field<DateTime>("ChargeEndDateTime") > endDateTime)
                                {
                                    adr["OverTime"] = (adr.Field<DateTime>("ChargeEndDateTime") - endDateTime).TotalMinutes - runningOverTime;
                                    runningOverTime += adr.Field<double>("OverTime");
                                }
                                else
                                    adr["OverTime"] = 0D;

                                dtToolDataClean.Rows.Add(adr); //will add to end

                                //make sure next month's data is not included.
                                if (adr.Field<DateTime>("ChargeBeginDateTime") >= Period.AddMonths(1))
                                    adr.Delete();
                            }

                            dr.Delete(); //remove original, multi-day row
                        }
                        else
                        {
                            dr["Uses"] = 1D;
                            if (!dr.Field<bool>("IsStarted"))
                                dr["ActDuration"] = dr["SchedDuration"];

                            //2011-11-08 we have to see of MaxReservedDuration is greater than the ChargeDuration - preventing cases when user shortened the reservation after the 2 hours allowed time
                            if (!dr.Field<bool>("IsCancelledBeforeAllowedTime"))
                            {
                                //this will return a list of reservation changigin history ordered by ModifiedDateTime
                                DataRow[] drsResHistory = dtResHistory.Select($"ReservationID = {dr["ReservationID"]}");

                                //must be at least two rows (becuase I inlcude Insert event as well, when reservation is firstly created)
                                if (drsResHistory.Length > 1)
                                {
                                    DataRow drPrevious = null;
                                    double maxResTimeAfterTwoHoursLimit = 0.0;
                                    double currentResTime;
                                    bool flagtemp = false;
                                    DateTime beginTimeTemp, endTimeTemp;

                                    foreach (DataRow drResHist in drsResHistory)
                                    {
                                        //the idea here is compare every reservation change and find out the max reservation time
                                        if (drResHist.Field<int>("BeforeMinutes") <= 120)
                                        {
                                            flagtemp = true;
                                            beginTimeTemp = drResHist.Field<DateTime>("BeginDateTime");
                                            endTimeTemp = drResHist.Field<DateTime>("EndDateTime");
                                            currentResTime = (endTimeTemp - beginTimeTemp).TotalMinutes;

                                            if (currentResTime > maxResTimeAfterTwoHoursLimit)
                                                maxResTimeAfterTwoHoursLimit = currentResTime;
                                        }

                                        if (!flagtemp)
                                        {
                                            //We need to track the last change of reservation right before the 2 hours limit, this is our true chargeTime 
                                            drPrevious = drResHist;
                                        }
                                    }

                                    if (drPrevious != null)
                                    {
                                        beginTimeTemp = drPrevious.Field<DateTime>("BeginDateTime");
                                        endTimeTemp = drPrevious.Field<DateTime>("EndDateTime");
                                        currentResTime = (endTimeTemp - beginTimeTemp).TotalMinutes;

                                        if (currentResTime > maxResTimeAfterTwoHoursLimit)
                                            maxResTimeAfterTwoHoursLimit = currentResTime;
                                    }

                                    //Lastly, make sure we charge the max time - please note that ChargeDuration could still be higher than maxResTimeAfterTwoHoursLimit because of actual tool usage time
                                    if (maxResTimeAfterTwoHoursLimit > dr.Field<double>("ChargeDuration"))
                                        dr["ChargeDuration"] = maxResTimeAfterTwoHoursLimit;
                                }
                            }
                        }
                    }
                    else
                        dr.Delete(); //not chargeable, so remove
                }
            }
        }

        private double GetMaxReservedDuration(DataRow dr)
        {
            // Is this a good idea? Seems like it would likely be multipled by Uses again later (what's the point of the Uses column).
            var result = dr.Field<double>("MaxReservedDuration") * dr.Field<double>("Uses");
            return result;
        }

        private bool IsChargeable(DataRow dr)
        {
            var activityId = dr.Field<int>("ActivityID");
            var activities = GetActivities();
            var result = activities.Rows.Find(activityId).Field<bool>("Chargeable");
            return result;
        }

        private bool IsCancelledBeforeAllowedTime(DataRow dr)
        {
            var reservationId = dr.Field<int>("ReservationID");
            var beginDateTime = dr.Field<DateTime>("BeginDateTime");
            var cancelledDateTime = dr.Field<DateTime?>("CancelledDateTime");

            if (!cancelledDateTime.HasValue)
            {
                //for accuracy in generating 2011-04-01 tool data, this checking below is necessary
                if (CancelledReservation20110401.FirstOrDefault(x => reservationId == x) > 0 && Period.Year == NewBillingDate.Year && Period.Month == NewBillingDate.Month)
                    return true;
                else
                    return false;
            }
            else
            {
                //determined by 2 hours before reservation time
                if (cancelledDateTime.Value.AddHours(2) < beginDateTime)
                    return true;
                else
                    return false;
            }
        }

        public void CalculateTransferTime(DataTable dtToolDataClean, ReservationDurations durations)
        {
            // [2016-05-17 jg] major refactoring to make things a lot more simple

            DataRow[] rows = dtToolDataClean.AsEnumerable().Where(x => x.RowState != DataRowState.Deleted && (ClientID == 0 || ClientID == x.Field<int>("ClientID"))).ToArray();

            // loop through reservations and get the TransferredDuration quantity for each
            foreach (DataRow dr in rows)
            {
                int reservationId = dr.Field<int>("ReservationID");
                int resourceId = dr.Field<int>("ResourceID");
                DateTime chargeBeginDateTime = dr.Field<DateTime>("ChargeBeginDateTime");
                DateTime chargeEndDateTime = dr.Field<DateTime>("ChargeEndDateTime");

                var item = durations.FirstOrDefault(x => x.Reservation.ReservationID == reservationId);

                var uses = dr.Field<double>("Uses");

                if (item != null)
                {
                    // [2018-08-01 jg] Occasionally item.TransferredDuration.TotalMinutes is a very small negative
                    //      number, never greater than -0.0001. I think we can safely assume these should be zero.
                    dr.SetField("TransferredDuration", Math.Max(item.TransferredDuration.TotalMinutes, 0) * uses);
                }
                else
                    dr.SetField("TransferredDuration", 0.0);
            }
        }

        private void FillOutputTable(DataTable dtOutput, DataTable dtToolDataClean)
        {
            DateTime sd = Period;
            DateTime ed = Period.AddMonths(1);

            foreach (DataRow dr in dtToolDataClean.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    // ClientID check. dtToolDataClean contains data for all clients even when ClientID > 0 because of transferred duration calculation.
                    int clientId = dr.Field<int>("ClientID");
                    if (ClientID == 0 || ClientID == clientId)
                    {
                        DateTime actDate = Convert.ToDateTime(dr["BeginDateTime"]).Date;
                        double schedDuration = dr.Field<double>("SchedDuration");
                        if ((actDate >= sd && actDate < ed) && schedDuration != 0)
                        {
                            DataRow ndr = dtOutput.NewRow();
                            ndr.SetField("Period", Period);
                            ndr.SetField("ClientID", clientId);
                            ndr.SetField("ResourceID", dr.Field<int>("ResourceID"));
                            ndr.SetField("RoomID", dr.Field<int>("RoomID"));
                            ndr.SetField("ActDate", actDate);
                            ndr.SetField("AccountID", dr.Field<int>("AccountID"));
                            ndr.SetField("Uses", dr.Field<double>("Uses"));
                            ndr.SetField("SchedDuration", schedDuration);
                            ndr.SetField("ActDuration", dr.Field<double>("ActDuration"));
                            ndr.SetField("Overtime", dr.Field<double>("Overtime"));
                            ndr.SetField("Days", 0D); //calulated in ToolDataAdjust
                            ndr.SetField("Months", 0D); //calulated in ToolDataAdjust
                            ndr.SetField("IsStarted", dr.Field<bool>("IsStarted"));
                            ndr.SetField("ChargeMultiplier", dr.Field<double>("ChargeMultiplier"));
                            ndr.SetField("ReservationID", dr.Field<int>("ReservationID"));
                            ndr.SetField("ChargeDuration", dr.Field<double>("ChargeDuration"));
                            ndr.SetField("TransferredDuration", dr.Field<double>("TransferredDuration"));
                            ndr.SetField("MaxReservedDuration", dr.Field<double>("MaxReservedDuration"));
                            ndr.SetField("ChargeBeginDateTime", dr.Field<DateTime>("ChargeBeginDateTime"));
                            ndr.SetField("ChargeEndDateTime", dr.Field<DateTime>("ChargeEndDateTime"));
                            ndr.SetField("IsActive", dr.Field<bool>("IsActive"));
                            ndr.SetField("IsCancelledBeforeAllowedTime", dr.Field<bool>("IsCancelledBeforeAllowedTime"));
                            dtOutput.Rows.Add(ndr);
                        }
                    }
                }
            }
        }

        private DataTable GetActivities()
        {
            if (_activities == null)
            {
                /*
                -- this is all that happens in sselScheduler.dbo.SSEL_DataRead @Action = 'ActivityType'
                -- (called by sselData.dbo.sselScheduler_Select)
                SELECT ActivityID, ActivityName, Chargeable
		        FROM dbo.Activity
		        ORDER BY ListOrder 
                */

                using (var cmd = Connection.CreateCommand("SELECT ActivityID, ActivityName, Chargeable FROM sselScheduler.dbo.Activity ORDER BY ListOrder", CommandType.Text))
                using (var adap = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    adap.Fill(dt);
                    _activities = dt;
                    _activities.PrimaryKey = new[] { _activities.Columns["ActivityID"] };
                }
            }

            return _activities;
        }

        private DataTable GetReservationHistory()
        {
            using (var cmd = Connection.CreateCommand("sselScheduler.dbo.procReservationHistorySelect"))
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("Action", "ForToolDataGeneration");
                cmd.Parameters.AddWithValue("Period", Period);
                var dt = new DataTable();
                adap.Fill(dt);
                return dt;
            }
        }

        private int ToolDataAdjust()
        {
            //adjust ToolData to add the days and months data
            using (var cmd = Connection.CreateCommand("dbo.ToolData_Adjust"))
            {
                cmd.Parameters.AddWithValue("Period", Period);
                AddParameterIf(cmd, "ClientID", ClientID > 0, ClientID);

                var result = cmd.ExecuteNonQuery();

                return result;
            }
        }

        private IEnumerable<ReservationDateRangeItem> GetReservationDateRangeItems(DataTable dtToolDataClean)
        {
            var cutoff = Period.AddMonths(1);

            var costs = ServiceProvider.Current.Data.Cost.FindToolCosts(ResourceID, cutoff);

            var result = new List<ReservationDateRangeItem>();

            foreach (DataRow dr in dtToolDataClean.Rows)
            {
                result.Add(ReservationDateRangeItem.Create(
                    dr.Field<int>("ReservationID"),
                    dr.Field<int>("ResourceID"),
                    dr.Field<string>("ResourceName"),
                    dr.Field<int>("ProcessTechID"),
                    dr.Field<string>("ProcessTechName"),
                    dr.Field<int>("ClientID"),
                    dr.Field<string>("UserName"),
                    dr.Field<string>("LName"),
                    dr.Field<string>("FName"),
                    dr.Field<int>("ActivityID"),
                    dr.Field<string>("ActivityName"),
                    dr.Field<int>("AccountID"),
                    dr.Field<string>("AccountName"),
                    dr.Field<string>("ShortCode"),
                    dr.Field<int>("ChargeTypeID"),
                    dr.Field<bool>("IsActive"),
                    dr.Field<bool>("IsStarted"),
                    dr.Field<DateTime>("BeginDateTime"),
                    dr.Field<DateTime>("EndDateTime"),
                    dr.Field<DateTime>("ActualBeginDateTime"),
                    dr.Field<DateTime>("ActualEndDateTime"),
                    dr.Field<DateTime>("LastModifiedOn"),
                    dr.Field<DateTime?>("CancelledDateTime"),
                    dr.Field<double>("ChargeMultiplier"),
                    costs));
            }

            return result;
        }

        public override LNF.DataAccess.IBulkCopy CreateBulkCopy()
        {
            var bcp = new DefaultBulkCopy("dbo.ToolData");
            bcp.AddColumnMapping("Period");
            bcp.AddColumnMapping("ClientID");
            bcp.AddColumnMapping("ResourceID");
            bcp.AddColumnMapping("RoomID");
            bcp.AddColumnMapping("ActDate");
            bcp.AddColumnMapping("AccountID");
            bcp.AddColumnMapping("Uses");
            bcp.AddColumnMapping("SchedDuration");
            bcp.AddColumnMapping("ActDuration");
            bcp.AddColumnMapping("OverTime");
            bcp.AddColumnMapping("Days");
            bcp.AddColumnMapping("Months");
            bcp.AddColumnMapping("IsStarted");
            bcp.AddColumnMapping("ChargeMultiplier");
            bcp.AddColumnMapping("ReservationID");
            bcp.AddColumnMapping("ChargeDuration");
            bcp.AddColumnMapping("TransferredDuration");
            bcp.AddColumnMapping("MaxReservedDuration");
            bcp.AddColumnMapping("ChargeBeginDateTime");
            bcp.AddColumnMapping("ChargeEndDateTime");
            bcp.AddColumnMapping("IsActive");
            bcp.AddColumnMapping("IsCancelledBeforeAllowedTime");
            return bcp;
        }
    }
}
