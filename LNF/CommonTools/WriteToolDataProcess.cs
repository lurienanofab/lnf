using LNF.Billing;
using LNF.Models.Billing.Process;
using LNF.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.CommonTools
{
    /// <summary>
    /// This process will:
    ///     1) Delete records from ToolData in the date range.
    ///     2) Select records from ToolDataClean in the date range to insert.
    ///     3) Insert records from ToolDataClean records into ToolData.
    ///     4) Adjust records in ToolData.
    /// </summary>
    public class WriteToolDataProcess : ProcessBase<WriteToolDataProcessResult>
    {
        public static readonly DateTime NewBillingDate = new DateTime(2011, 4, 1);

        public DateTime Period { get; set; }
        public int ClientID { get; set; }
        public int ResourceID { get; set; }

        private DataSet _ds;

        protected override WriteToolDataProcessResult CreateResult()
        {
            return new WriteToolDataProcessResult
            {
                Period = Period,
                ClientID = ClientID,
                ResourceID = ResourceID
            };
        }

        // force using the static constructor
        public WriteToolDataProcess(DateTime period, int clientId = 0, int resourceId = 0)
        {
            Period = period;
            ClientID = clientId;
            ResourceID = resourceId;
        }

        public override int DeleteExisting()
        {
            return DA.Command()
                .Param("Period", Period)
                .Param("ClientID", ClientID > 0, ClientID)
                .Param("ResourceID", ResourceID > 0, ResourceID)
                .ExecuteNonQuery("dbo.ToolData_Delete").Value;
        }

        public override DataTable Extract()
        {
            DateTime sd = Period;
            DateTime ed = Period.AddMonths(1);
            _ds = ReadData.Tool.ReadToolDataClean(sd, ed, ClientID, ResourceID);
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
            int result = base.Load(dtTransform);
            _result.RowsAdjusted = ToolDataAdjust();
            return result;
        }

        private DataTable DailyToolDataOld(DataTable dtToolDataClean)
        {
            DataTable dtToolData = CreateToolDataTable();

            //the select statement pulls in all reservations that overlap the period
            //only write those whose ActDate is within the period

            if (dtToolDataClean.Rows.Count > 0)
            {
                dtToolDataClean.Columns.Add("Uses", typeof(double));

                DataTable dtClient = _ds.Tables["Client"];
                DataTable dtResource = _ds.Tables["Resource"];
                DataTable dtActivity = GetActivities();

                //handled started separately from unstarted
                bool chargeable;
                int toolDataCleanRowCount = dtToolDataClean.Rows.Count;
                for (int i = 0; i < toolDataCleanRowCount; i++)
                {
                    DataRow dr = dtToolDataClean.Rows[i];
                    //only chargeable activities get written to Tooldata
                    chargeable = Convert.ToBoolean(dtActivity.Rows.Find(dr["ActivityID"])["Chargeable"]);
                    if (chargeable)
                    {
                        //this means reservation was started
                        double schedDuration = Convert.ToDouble(dr["SchedDuration"]);
                        double actDuration = Convert.ToDouble(dr["ActDuration"]);
                        DateTime beginDateTime = Convert.ToDateTime(dr["BeginDateTime"]);
                        DateTime endDateTime = beginDateTime.AddMinutes(schedDuration);
                        DateTime actualBeginDateTime = Convert.ToDateTime(dr["ActualBeginDateTime"]);
                        DateTime actualEndDateTime = Convert.ToDateTime(dr["ActualEndDateTime"]);

                        DateTime begDate = (Convert.ToBoolean(dr["IsStarted"])) ? actualBeginDateTime : beginDateTime;
                        begDate = begDate.Date;
                        DateTime finDate = (Convert.ToBoolean(dr["IsStarted"])) ? actualEndDateTime : endDateTime;
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

                                dtToolDataClean.Rows.Add(adr); //will add to end
                            }
                            dr.Delete(); //remove original, multi-day row
                        }
                        else
                        {
                            dr["Uses"] = 1.0;
                            if (!Convert.ToBoolean(dr["IsStarted"]))
                                dr["ActDuration"] = dr["SchedDuration"];
                        }
                    }
                    else
                        dr.Delete(); //not chargeable, so remove
                }

                foreach (DataRow dr in dtToolDataClean.Rows)
                {
                    if (dr.RowState != DataRowState.Deleted)
                    {
                        DateTime actDate = Convert.ToDateTime(dr["BeginDateTime"]).Date;
                        if ((actDate >= Period && actDate < Period.AddMonths(1)) && Convert.ToDouble(dr["SchedDuration"]) != 0.0)
                        {
                            DataRow ndr = dtToolData.NewRow();
                            ndr.SetField("Period", new DateTime(actDate.Year, actDate.Month, 1));
                            ndr.SetField("ReservationID", dr.Field<int>("ReservationID"));
                            ndr.SetField("ClientID", dr.Field<int>("ClientID"));
                            ndr.SetField("ResourceID", dr.Field<int>("ResourceID"));
                            ndr.SetField("RoomID", dr.Field<int>("RoomID"));
                            ndr.SetField("ActDate", actDate);
                            ndr.SetField("AccountID", dr.Field<int>("AccountID"));
                            ndr.SetField("Uses", dr.Field<double>("Uses"));
                            ndr.SetField("SchedDuration", dr.Field<double>("SchedDuration"));
                            ndr.SetField("ActDuration", dr.Field<double>("ActDuration"));
                            ndr.SetField("Overtime", dr.Field<double>("Overtime"));
                            ndr.SetField("IsStarted", dr.Field<bool>("IsStarted"));
                            ndr.SetField("ChargeMultiplier", dr.Field<double>("ChargeMultiplier"));
                            dtToolData.Rows.Add(ndr);
                        }
                    }
                }
            }
            return dtToolData;
        }

        //This handles generation of toolData table since 2011-04-01
        private DataTable DailyToolData20110401(DataTable dtToolDataClean)
        {
            DataTable dtOutput = CreateToolDataTable();

            // new columns
            dtOutput.Columns.Add("ChargeDuration", typeof(double));
            dtOutput.Columns.Add("TransferredDuration", typeof(double));
            dtOutput.Columns.Add("MaxReservedDuration", typeof(double));
            dtOutput.Columns.Add("IsActive", typeof(bool));
            dtOutput.Columns.Add("IsCancelledBeforeAllowedTime", typeof(bool));
            dtOutput.Columns.Add("ChargeBeginDateTime", typeof(DateTime));
            dtOutput.Columns.Add("ChargeEndDateTime", typeof(DateTime));

            //the select statement pulls in all reservations that overlap the period
            //only write those whose ActDate is within the period

            if (dtToolDataClean.Rows.Count == 0)
                return dtOutput;

            ProcessCleanData(dtToolDataClean);
            CalculateTransferTime(dtToolDataClean);
            FillOutputTable(dtOutput, dtToolDataClean);

            return dtOutput;
        }

        private DataTable CreateToolDataTable()
        {
            DataTable dt = new DataTable();
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
            return dt;
        }

        private void ProcessCleanData(DataTable dtToolDataClean)
        {
            //This list of reservations were cancelled before 2011-04-01 so they should only be charged with booking fees
            //Delete this ONLY when there is no need to regenerate the April 2011 data
            List<int> cancelledReservation20110401 = new List<int>(new int[] { 286867, 302663, 302703, 303807, 303818, 303856, 302876, 303041, 303155, 303156, 303168, 303288, 303444, 303534, 303556, 303697, 303735, 303787 });

            dtToolDataClean.Columns.Add("Uses", typeof(double));
            dtToolDataClean.Columns.Add("TransferredDuration", typeof(double));
            dtToolDataClean.Columns.Add("IsCancelledBeforeAllowedTime", typeof(bool));
            dtToolDataClean.Columns.Add("ChargeBeginDateTime", typeof(DateTime));
            dtToolDataClean.Columns.Add("ChargeEndDateTime", typeof(DateTime));
            dtToolDataClean.Columns.Add("ChargeDuration", typeof(double));

            DataTable dtActivity = DA.Command().Param("Action", "ActivityType").FillDataTable("dbo.sselScheduler_Select");
            dtActivity.PrimaryKey = new DataColumn[] { dtActivity.Columns["ActivityID"] };

            //handled started separately from unstarted
            double schedDuration, actDuration, chargeDuration;
            DateTime beginDateTime, endDateTime, actualBeginDateTime, actualEndDateTime, chargeBeginDateTime, chargeEndDateTime;
            int toolDataCleanRowCount = dtToolDataClean.Rows.Count;

            //2011-12-05 Get ReservationHistory so we can calculate the max charge time
            DataTable dtResHistory = GetReservationHistory();

            for (int i = 0; i < toolDataCleanRowCount; i++)
            {
                DataRow dr = dtToolDataClean.Rows[i];

                //only chargeable activities get written to Tooldata
                if (Convert.ToBoolean(dtActivity.Rows.Find(dr["ActivityID"])["Chargeable"]))
                {
                    int reservationId = dr.Field<int>("ReservationID");
                    int resourceId = dr.Field<int>("ResourceID");

                    //this means reservation was started
                    schedDuration = Convert.ToDouble(dr["SchedDuration"]);
                    actDuration = Convert.ToDouble(dr["ActDuration"]);
                    beginDateTime = Convert.ToDateTime(dr["BeginDateTime"]);
                    endDateTime = beginDateTime.AddMinutes(schedDuration); //to prevent auto end's modified end time, which is wrong
                    actualBeginDateTime = Convert.ToDateTime(dr["ActualBeginDateTime"]);
                    actualEndDateTime = Convert.ToDateTime(dr["ActualEndDateTime"]);

                    DateTime? cancelledDateTime = dr.Field<DateTime?>("CancelledDateTime");

                    if (!cancelledDateTime.HasValue)
                    {
                        //for accuracy in generating 2011-04-01 tool data, this checking below is necessary
                        if (cancelledReservation20110401.FirstOrDefault(x => reservationId == x) > 0 && Period.Year == NewBillingDate.Year && Period.Month == NewBillingDate.Month)
                            dr.SetField("IsCancelledBeforeAllowedTime", true);
                        else
                            dr.SetField("IsCancelledBeforeAllowedTime", false);
                    }
                    else
                    {
                        //determined by 2 hours before reservation time
                        if (cancelledDateTime.Value.AddHours(2) < beginDateTime)
                            dr["IsCancelledBeforeAllowedTime"] = true;
                        else
                            dr["IsCancelledBeforeAllowedTime"] = false;
                    }

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
                            adr["ChargeDuration"] = (Convert.ToDateTime(adr["ChargeEndDateTime"]) - Convert.ToDateTime(adr["ChargeBeginDateTime"])).TotalMinutes;
                            adr["Uses"] = Convert.ToDouble(adr["ChargeDuration"]) / chargeDuration;
                            adr["MaxReservedDuration"] = Convert.ToDouble(adr["MaxReservedDuration"]) * Convert.ToDouble(adr["Uses"]);

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
                                */
                                adr["BeginDateTime"] = adr["ActualBeginDateTime"]; //for convenience below when writing new table
                            }
                            adr["ActDuration"] = Convert.ToDateTime(adr["ActualEndDateTime"]).Subtract(Convert.ToDateTime(adr["ActualBeginDateTime"])).TotalMinutes;

                            //make sure ActDuration is greater than 0, because if a user finish this reservatio on the same day (a cross day reservation originally), it's possible to have negative ActDuration
                            if (Convert.ToDouble(adr["ActDuration"]) < 0)
                                adr["ActDuration"] = 0; //since user finished on the previous day, today's reservation actual duration is zero

                            if (Convert.ToDateTime(adr["ChargeEndDateTime"]) > endDateTime)
                            {
                                adr["OverTime"] = (Convert.ToDateTime(adr["ChargeEndDateTime"]) - endDateTime).TotalMinutes - runningOverTime;
                                runningOverTime += Convert.ToDouble(adr["OverTime"]);
                            }
                            else
                                adr["OverTime"] = 0.0;

                            dtToolDataClean.Rows.Add(adr); //will add to end

                            //make sure next month's data is not included.
                            if (Convert.ToDateTime(adr["ChargeBeginDateTime"]) >= Period.AddMonths(1))
                                adr.Delete();
                        }
                        dr.Delete(); //remove original, multi-day row
                    }
                    else
                    {
                        dr["Uses"] = 1.0;
                        if (!Convert.ToBoolean(dr["IsStarted"]))
                            dr["ActDuration"] = dr["SchedDuration"];

                        //2011-11-08 we have to see of MaxReservedDuration is greater than the ChargeDuration - preventing cases when user shortened the reservation after the 2 hours allowed time
                        if (!Convert.ToBoolean(dr["IsCancelledBeforeAllowedTime"]))
                        {
                            //this will return a list of reservation changigin history ordered by ModifiedDateTime
                            DataRow[] drsResHistory = dtResHistory.Select(string.Format("ReservationID = {0}", dr["ReservationID"]));

                            //must be at least two rows (becuase I inlcude Insert event as well, when reservation is firstly created)
                            if (drsResHistory.Length > 1)
                            {
                                DataRow drPrevious = null;
                                double maxResTimeAfterTwoHoursLimit = 0.0;
                                double currentResTime = 0.0;
                                bool flagtemp = false;
                                DateTime biginTimeTemp, endTimeTemp;

                                foreach (DataRow reshisrow in drsResHistory)
                                {
                                    //the idea here is compare every reservation change and find out the max reservation time
                                    if (Convert.ToInt32(reshisrow["BeforeMinutes"]) <= 120)
                                    {
                                        flagtemp = true;
                                        biginTimeTemp = Convert.ToDateTime(reshisrow["BeginDateTime"]);
                                        endTimeTemp = Convert.ToDateTime(reshisrow["EndDateTime"]);
                                        currentResTime = (endTimeTemp - biginTimeTemp).TotalMinutes;

                                        if (currentResTime > maxResTimeAfterTwoHoursLimit)
                                            maxResTimeAfterTwoHoursLimit = currentResTime;
                                    }

                                    if (!flagtemp)
                                    {
                                        //We need to track the last change of reservation right before the 2 hours limit, this is our true chargeTime 
                                        drPrevious = reshisrow;
                                    }
                                }

                                if (drPrevious != null)
                                {
                                    biginTimeTemp = Convert.ToDateTime(drPrevious["BeginDateTime"]);
                                    endTimeTemp = Convert.ToDateTime(drPrevious["EndDateTime"]);
                                    currentResTime = (endTimeTemp - biginTimeTemp).TotalMinutes;

                                    if (currentResTime > maxResTimeAfterTwoHoursLimit)
                                        maxResTimeAfterTwoHoursLimit = currentResTime;
                                }

                                //Lastly, make sure we charge the max time - please note that ChargeDuration could still be higher than maxResTimeAfterTwoHoursLimit because of actual tool usage time
                                if (maxResTimeAfterTwoHoursLimit > Convert.ToDouble(dr["ChargeDuration"]))
                                    dr["ChargeDuration"] = maxResTimeAfterTwoHoursLimit;
                            }
                        }
                    }
                }
                else
                    dr.Delete(); //not chargeable, so remove
            }
        }

        private void CalculateTransferTime(DataTable dtToolDataClean)
        {
            // [2016-05-17 jg] major refactoring to make things a lot more simple

            DataRow[] rows = dtToolDataClean.AsEnumerable().Where(x => x.RowState != DataRowState.Deleted).ToArray();

            ReservationDateRange range = new ReservationDateRange(Period);
            ReservationDurations durations = range.CreateReservationDurations();

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
                    DateTime actDate = Convert.ToDateTime(dr["BeginDateTime"]).Date;
                    if ((actDate >= sd && actDate < ed) && Convert.ToDouble(dr["SchedDuration"]) != 0)
                    {
                        DataRow ndr = dtOutput.NewRow();
                        ndr.SetField("Period", Period);
                        ndr.SetField("ReservationID", dr.Field<int>("ReservationID"));
                        ndr.SetField("ClientID", dr.Field<int>("ClientID"));
                        ndr.SetField("ResourceID", dr.Field<int>("ResourceID"));
                        ndr.SetField("RoomID", dr.Field<int>("RoomID"));
                        ndr.SetField("ActDate", actDate);
                        ndr.SetField("AccountID", dr.Field<int>("AccountID"));
                        ndr.SetField("Uses", dr.Field<double>("Uses"));
                        ndr.SetField("SchedDuration", dr.Field<double>("SchedDuration"));
                        ndr.SetField("ActDuration", dr.Field<double>("ActDuration"));
                        ndr.SetField("ChargeDuration", dr.Field<double>("ChargeDuration"));
                        ndr.SetField("TransferredDuration", dr.Field<double>("TransferredDuration"));
                        ndr.SetField("MaxReservedDuration", dr.Field<double>("MaxReservedDuration"));
                        ndr.SetField("Overtime", dr.Field<double>("Overtime"));
                        ndr.SetField("IsStarted", dr.Field<bool>("IsStarted"));
                        ndr.SetField("IsActive", dr.Field<bool>("IsActive"));
                        ndr.SetField("IsCancelledBeforeAllowedTime", dr.Field<bool>("IsCancelledBeforeAllowedTime"));
                        ndr.SetField("ChargeMultiplier", dr.Field<double>("ChargeMultiplier"));
                        ndr.SetField("ChargeBeginDateTime", dr.Field<DateTime>("ChargeBeginDateTime"));
                        ndr.SetField("ChargeEndDateTime", dr.Field<DateTime>("ChargeEndDateTime"));
                        dtOutput.Rows.Add(ndr);
                    }
                }
            }
        }

        private DataTable GetActivities()
        {
            var dt = DA.Command()
                .Param("Action", "ActivityType")
                .FillDataTable("dbo.sselScheduler_Select");

            dt.PrimaryKey = new[] { dt.Columns["ActivityID"] };

            return dt;
        }

        private DataTable GetReservationHistory()
        {
            return DA.Command()
                .Param("Action", "ForToolDataGeneration")
                .Param("Period", Period)
                .FillDataTable("sselScheduler.dbo.procReservationHistorySelect");
        }

        private int ToolDataAdjust()
        {
            //adjust ToolData to add the days and months data
            return DA.Command()
                .Param("Period", Period)
                .ExecuteNonQuery("dbo.ToolData_Adjust").Value;
        }

        public override IBulkCopy CreateBulkCopy()
        {
            IBulkCopy bcp = DA.Current.GetBulkCopy("dbo.ToolData");
            bcp.AddColumnMapping("Period");
            bcp.AddColumnMapping("ReservationID");
            bcp.AddColumnMapping("ClientID");
            bcp.AddColumnMapping("ResourceID");
            bcp.AddColumnMapping("RoomID");
            bcp.AddColumnMapping("ActDate");
            bcp.AddColumnMapping("AccountID");
            bcp.AddColumnMapping("Uses");
            bcp.AddColumnMapping("SchedDuration");
            bcp.AddColumnMapping("ActDuration");
            bcp.AddColumnMapping("ChargeDuration");
            bcp.AddColumnMapping("TransferredDuration");
            bcp.AddColumnMapping("MaxReservedDuration");
            bcp.AddColumnMapping("OverTime");
            bcp.AddColumnMapping("IsStarted");
            bcp.AddColumnMapping("IsActive");
            bcp.AddColumnMapping("ChargeMultiplier");
            bcp.AddColumnMapping("IsCancelledBeforeAllowedTime");
            bcp.AddColumnMapping("ChargeBeginDateTime");
            bcp.AddColumnMapping("ChargeEndDateTime");
            return bcp;
        }
    }
}
