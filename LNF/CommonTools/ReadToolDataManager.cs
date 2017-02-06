using LNF.Billing;
using LNF.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.CommonTools
{
    public class ReadToolDataManager
    {
        public ReadToolDataManager() { }

        public DataSet ds = new DataSet();

        public DataTable ReadToolData(DateTime period, int clientId = 0, int resourceId = 0)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.SelectCommand
                    .AddParameter("@Action", "ByMonthTool")
                    .AddParameter("@Period", period)
                    .AddParameterIf("@ClientID", clientId > 0, clientId)
                    .AddParameterIf("@ResourceID", resourceId > 0, resourceId);

                var dt = dba.FillDataTable("ToolData_Select");
                dt.TableName = "ToolUsage";

                return dt;
            }
        }

        public DataTable DailyToolData(DateTime sDate, DateTime eDate, int clientId = 0, int resourceId = 0)
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

            //the select statement pulls in all reservations that overlap the period
            //only write those whose ActDate is within the period
            DataTable dtToolDataClean = ReadToolDataClean(sDate, eDate, clientId, resourceId);
            if (dtToolDataClean.Rows.Count > 0)
            {
                dtToolDataClean.Columns.Add("Uses", typeof(double));
                DataTable dtClient = ds.Tables[1];
                DataTable dtResource = ds.Tables[2];

                using (var dba = DA.Current.GetAdapter())
                {
                    DataTable dtActivity = dba.ApplyParameters(new { Action = "ActivityType" }).FillDataTable("sselScheduler_Select");
                    dtActivity.PrimaryKey = new DataColumn[] { dtActivity.Columns["ActivityID"] };

                    //handled started separately from unstarted
                    bool Chargeable;
                    int ToolDataCleanRowCount = dtToolDataClean.Rows.Count;
                    for (int i = 0; i < ToolDataCleanRowCount; i++)
                    {
                        DataRow dr = dtToolDataClean.Rows[i];
                        //only chargeable activities get written to Tooldata
                        Chargeable = Convert.ToBoolean(dtActivity.Rows.Find(dr["ActivityID"])["Chargeable"]);
                        if (Chargeable)
                        {
                            //this means reservation was started
                            double SchedDuration = Convert.ToDouble(dr["SchedDuration"]);
                            double ActDuration = Convert.ToDouble(dr["ActDuration"]);
                            DateTime BeginDateTime = Convert.ToDateTime(dr["BeginDateTime"]);
                            DateTime EndDateTime = BeginDateTime.AddMinutes(SchedDuration);
                            DateTime ActualBeginDateTime = Convert.ToDateTime(dr["ActualBeginDateTime"]);
                            DateTime ActualEndDateTime = Convert.ToDateTime(dr["ActualEndDateTime"]);

                            DateTime begDate = (Convert.ToBoolean(dr["IsStarted"])) ? ActualBeginDateTime : BeginDateTime;
                            begDate = begDate.Date;
                            DateTime finDate = (Convert.ToBoolean(dr["IsStarted"])) ? ActualEndDateTime : EndDateTime;
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
                                        adr["ActualBeginDateTime"] = (bDate > ActualBeginDateTime) ? bDate : ActualBeginDateTime;
                                        adr["ActualEndDateTime"] = (fDate < ActualEndDateTime) ? fDate : ActualEndDateTime;
                                        adr["BeginDateTime"] = adr["ActualBeginDateTime"]; //for convenience below when writing new table
                                        adr["ActDuration"] = Convert.ToDateTime(adr["ActualEndDateTime"]).Subtract(Convert.ToDateTime(adr["ActualBeginDateTime"])).TotalMinutes;
                                        adr["Uses"] = Convert.ToDouble(adr["ActDuration"]) / ActDuration;
                                        if (Convert.ToDateTime(adr["ActualEndDateTime"]) > EndDateTime)
                                        {
                                            adr["OverTime"] = Convert.ToDateTime(adr["ActualEndDateTime"]).Subtract(EndDateTime).TotalMinutes - runningOverTime;
                                            runningOverTime += Convert.ToDouble(adr["OverTime"]);
                                        }
                                        else
                                            adr["OverTime"] = 0.0;
                                    }
                                    else
                                    {
                                        adr["BeginDateTime"] = (bDate > BeginDateTime) ? bDate : BeginDateTime;
                                        adr["EndDateTime"] = (fDate < EndDateTime) ? fDate : EndDateTime;
                                        adr["SchedDuration"] = Convert.ToDateTime(adr["EndDateTime"]).Subtract(Convert.ToDateTime(adr["BeginDateTime"])).TotalMinutes;
                                        adr["ActDuration"] = adr["SchedDuration"]; //should be 0, but the adjust SP would be way too complex
                                        adr["Uses"] = Convert.ToDouble(adr["SchedDuration"]) / SchedDuration;
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
                            if ((actDate >= sDate && actDate < eDate) && Convert.ToDouble(dr["SchedDuration"]) != 0.0)
                            {
                                DataRow ndr = dt.NewRow();
                                ndr["Period"] = new DateTime(actDate.Year, actDate.Month, 1);
                                ndr["ReservationID"] = dr["ReservationID"];
                                ndr["ClientID"] = dr["ClientID"];
                                ndr["ResourceID"] = dr["ResourceID"];
                                ndr["RoomID"] = dr["RoomID"];
                                ndr["ActDate"] = actDate;
                                ndr["AccountID"] = dr["AccountID"];
                                ndr["Uses"] = dr["Uses"];
                                ndr["SchedDuration"] = dr["SchedDuration"];
                                ndr["ActDuration"] = dr["ActDuration"];
                                ndr["Overtime"] = dr["Overtime"];
                                ndr["IsStarted"] = dr["IsStarted"];
                                ndr["ChargeMultiplier"] = dr["ChargeMultiplier"];
                                dt.Rows.Add(ndr);
                            }
                        }
                    }
                }
            }
            return dt;
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
            dt.Columns.Add("ChargeDuration", typeof(double));
            dt.Columns.Add("TransferredDuration", typeof(double));
            dt.Columns.Add("MaxReservedDuration", typeof(double));
            dt.Columns.Add("Overtime", typeof(double));
            dt.Columns.Add("IsStarted", typeof(bool));
            dt.Columns.Add("IsActive", typeof(bool));
            dt.Columns.Add("IsCancelledBeforeAllowedTime", typeof(bool));
            dt.Columns.Add("ChargeMultiplier", typeof(double));
            dt.Columns.Add("ChargeBeginDateTime", typeof(DateTime));
            dt.Columns.Add("ChargeEndDateTime", typeof(DateTime));
            return dt;
        }

        //This handles generation of toolData table since 2011-04-01
        public DataTable DailyToolData20110401(DateTime sDate, DateTime eDate, int clientId = 0, int resourceId = 0)
        {
            DataTable dtOutput = CreateToolDataTable();

            //the select statement pulls in all reservations that overlap the period
            //only write those whose ActDate is within the period
            DataTable dtToolDataClean = ReadToolDataClean(sDate, eDate, clientId, resourceId);
            if (dtToolDataClean.Rows.Count == 0)
                return dtOutput;

            ProcessCleanData(dtToolDataClean, sDate, eDate);
            CalculateTransferTime(dtToolDataClean, sDate, eDate);
            //CalculateTransferTimeOld(dtToolDataClean, sDate, clientId);
            FillOutputTable(dtOutput, dtToolDataClean, sDate, eDate);

            return dtOutput;
        }

        public void ProcessCleanData(DataTable dtToolDataClean, DateTime sDate, DateTime eDate)
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

            using (var dba = DA.Current.GetAdapter())
            {
                DataTable dtActivity = dba.ApplyParameters(new { Action = "ActivityType" }).FillDataTable("sselScheduler_Select");
                dtActivity.PrimaryKey = new DataColumn[] { dtActivity.Columns["ActivityID"] };

                //handled started separately from unstarted
                double schedDuration, actDuration, chargeDuration;
                DateTime beginDateTime, endDateTime, actualBeginDateTime, actualEndDateTime, chargeBeginDateTime, chargeEndDateTime;
                int toolDataCleanRowCount = dtToolDataClean.Rows.Count;

                //2011-12-05 Get ReservationHistory so we can calculate the max charge time
                DataTable dtResHistory = GetReservationHistory(sDate);

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
                            if (cancelledReservation20110401.FirstOrDefault(x => reservationId == x) > 0 && sDate.Year == 2011 && sDate.Month == 4)
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
                                if (sDate >= DateTime.Parse("2012-12-01"))
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
                                if (Convert.ToDateTime(adr["ChargeBeginDateTime"]) >= eDate)
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
        }

        public void CalculateTransferTime(DataTable dtToolDataClean, DateTime sd, DateTime ed)
        {
            // [2016-05-17 jg] major refactoring to make things a lot more simple

            DataRow[] rows = dtToolDataClean.AsEnumerable().Where(x => x.RowState != DataRowState.Deleted).ToArray();

            ReservationDateRange range = new ReservationDateRange(sd, ed);
            ReservationDurations durations = new ReservationDurations(range);

            // loop through reservations and get the TransferredDuration quantity for each
            foreach (DataRow dr in rows)
            {
                int reservationId = dr.Field<int>("ReservationID");
                int resourceId = dr.Field<int>("ResourceID");
                DateTime chargeBeginDateTime = dr.Field<DateTime>("ChargeBeginDateTime");
                DateTime chargeEndDateTime = dr.Field<DateTime>("ChargeEndDateTime");

                var item = durations.FirstOrDefault(x => x.Reservation.ReservationID == reservationId);

                double uses = dr.Field<double>("Uses");

                if (item != null)
                    dr.SetField("TransferredDuration", item.TransferredDuration.TotalMinutes * uses);
                else
                    dr.SetField("TransferredDuration", 0.0);
            }
        }

        [Obsolete("Use ReservationsDurations class.")]
        public class TransferredTimeSlot
        {
            public DateTime BeginDateTime;
            public DateTime EndDateTime;
        }

        [Obsolete("Use ReservationsDurations class.")]
        private void CalculateTransferTimeOld(DataTable dtToolDataClean, DateTime sDate, int clientId = 0)
        {
            //2011-03-07 Transferred time calculation
            bool IsStarted;
            double TransferredDuration;
            int myClientID, myResourceID, myReservationID;
            DateTime myChargeBeginDateTime, myChargeEndDateTime;
            DateTime TransferChargeBeginDateTime, TransferChargeEndDateTime;
            string FilterBase, FilterTime;
            DateTime myCreatedOn;
            int tempClientID = clientId;

            double SchedDuration;
            DateTime BeginDateTime;
            DateTime EndDateTime;
            DateTime ActualBeginDateTime;
            DateTime ActualEndDateTime;

            foreach (DataRow dr in dtToolDataClean.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    if (clientId == 0)
                        tempClientID = Convert.ToInt32(dr["ClientID"]);
                    if (tempClientID == Convert.ToInt32(dr["ClientID"]))
                    {
                        if (!Convert.ToBoolean(dr["IsCancelledBeforeAllowedTime"]))
                        {
                            SchedDuration = Convert.ToDouble(dr["SchedDuration"]);
                            BeginDateTime = Convert.ToDateTime(dr["BeginDateTime"]);
                            EndDateTime = BeginDateTime.AddMinutes(SchedDuration); //to prevent auto end's modified end time, which is wrong
                            ActualBeginDateTime = Convert.ToDateTime(dr["ActualBeginDateTime"]);
                            ActualEndDateTime = Convert.ToDateTime(dr["ActualEndDateTime"]);
                            myChargeBeginDateTime = Convert.ToDateTime(dr["ChargeBeginDateTime"]);
                            myChargeEndDateTime = Convert.ToDateTime(dr["ChargeEndDateTime"]);
                            IsStarted = Convert.ToBoolean(dr["IsStarted"]);
                            myResourceID = Convert.ToInt32(dr["ResourceID"]);
                            myClientID = Convert.ToInt32(dr["ClientID"]);
                            myReservationID = Convert.ToInt32(dr["ReservationID"]);
                            myCreatedOn = Convert.ToDateTime(dr["CreatedOn"]); //used to compare who should get the free reservation - the earlier wins

                            //we need to check if there is any transfer time, 
                            FilterBase = string.Format("ResourceID = {0} AND ReservationID <> {1} AND ActivityID in (6, 7, 16) AND ChargeMultiplier <> 0 AND IsCancelledBeforeAllowedTime = 0", myResourceID, myReservationID);
                            FilterTime = string.Format("(ChargeBeginDateTime >= '{0}' AND ChargeBeginDateTime < '{1}' AND CreatedOn > '{2}')", myChargeBeginDateTime, myChargeEndDateTime, myCreatedOn);

                            DataRow[] drwsTranferReservations = dtToolDataClean.Select(string.Format("{0} AND {1}", FilterBase, FilterTime), "ChargeBeginDateTime");

                            TransferredDuration = 0;
                            if (drwsTranferReservations.Length > 0)
                            {
                                //this will store the time slots of transferred time, it's possible to have more than one qualified transfer reservations 
                                List<TransferredTimeSlot> TransferredTimeSlots = new List<TransferredTimeSlot>();

                                //The idea for this for loop is to calculate the un-transferred time - because transfer reservations could have overlapping hours, and we need to make sure
                                //current reservation won't get too much credit because of overlapping transfers
                                foreach (DataRow eachTransfer in drwsTranferReservations)
                                {
                                    TransferChargeBeginDateTime = Convert.ToDateTime(eachTransfer["ChargeBeginDateTime"]);
                                    TransferChargeEndDateTime = Convert.ToDateTime(eachTransfer["ChargeEndDateTime"]);

                                    //The code block below will try to calculate the UnTransferredTime, by default it's 0
                                    double UnTransferredTime = 0; //overlapping reservations time that we must deduct so users won't take advantage of multiple overlapping subsequent reservations UnTransferredTime
                                    //if we have transferred time slots, we must try to compare the current Transfer reservation, so we don't have duplicate free deduciton on multiple overlaping subsequent reservations
                                    if (TransferredTimeSlots.Count > 0)
                                    {
                                        bool flagFound = false;
                                        foreach (TransferredTimeSlot slot in TransferredTimeSlots)
                                        {
                                            //is current transfer reservation have overlapping on previous time slots?
                                            if (TransferChargeBeginDateTime >= slot.BeginDateTime && TransferChargeBeginDateTime < slot.EndDateTime)
                                            {
                                                flagFound = true;
                                                //we have overlaping reservation, so we must find out the un-transferredTime
                                                if (slot.EndDateTime >= TransferChargeEndDateTime)
                                                    UnTransferredTime = (TransferChargeEndDateTime - TransferChargeBeginDateTime).TotalMinutes;
                                                else
                                                    UnTransferredTime = (slot.EndDateTime - TransferChargeBeginDateTime).TotalMinutes;

                                                //We have to see if we need to expand the current time slot
                                                slot.BeginDateTime = (TransferChargeBeginDateTime < slot.BeginDateTime) ? ((TransferChargeBeginDateTime < myChargeBeginDateTime) ? myChargeBeginDateTime : TransferChargeBeginDateTime) : slot.BeginDateTime;
                                                slot.EndDateTime = (TransferChargeEndDateTime > slot.EndDateTime) ? ((TransferChargeEndDateTime > myChargeEndDateTime) ? myChargeEndDateTime : TransferChargeEndDateTime) : slot.EndDateTime;
                                            }
                                        }

                                        if (!flagFound)
                                        {
                                            //no overlapping, then we must add this as a new time slot
                                            TransferredTimeSlots.Add(new TransferredTimeSlot { BeginDateTime = TransferChargeBeginDateTime, EndDateTime = (TransferChargeEndDateTime < myChargeEndDateTime) ? TransferChargeEndDateTime : myChargeEndDateTime });
                                        }
                                    }
                                    else
                                    {
                                        //no overlapping, then we must add this as a new time slot
                                        TransferredTimeSlots.Add(new TransferredTimeSlot { BeginDateTime = TransferChargeBeginDateTime, EndDateTime = (TransferChargeEndDateTime < myChargeEndDateTime) ? TransferChargeEndDateTime : myChargeEndDateTime });
                                    }

                                    if (UnTransferredTime < 0)
                                        UnTransferredTime = 0;

                                    double TransferredTime = 0.0;

                                    if (myChargeEndDateTime == ActualEndDateTime)
                                    {
                                        //When ChargeEndDateTime is equal to ActualEndDateTime, it could be two scenarios
                                        //#1 This means my reservation ends later than originally reserved.  This means it's possible I could affect the later reservations, and we must forgive the later reservations.
                                        //#2 This means I didn't start my reservation at all, so ChargeEndDateTime = ActualEndDateTime = EndDateTime where ActualEndDateTime is populated by Scheduler

                                        if (IsStarted)
                                        {
                                            //#1 Now we have to see if the transfer reservation has started or not.  If started, that means the delay didn't affect next user, so we just calculate transfer time in other place
                                            if (!Convert.ToBoolean(eachTransfer["isStarted"]))
                                            {
                                                //user didn't start the transfer reservation, right now we log only, got to come back and fix this
                                                ExceptionManager exp = new ExceptionManager
                                                {
                                                    TimeStamp = DateTime.Now,
                                                    ExpName = "Possible overtime reservation affecting others",
                                                    AppName = this.GetType().Assembly.GetName().Name,
                                                    FunctionName = "CommonTools-DailyToolData20110401",
                                                    CustomData = string.Format("ClientID = {0}, Period = '{1}', ReservationID = {2}, Transfer ReservationID = {3}", myClientID, sDate, myReservationID, eachTransfer["ReservationID"])
                                                };
                                                exp.LogException();
                                            }
                                        }
                                        else
                                        {
                                            //#2 I didn't start reservation, but I can still calculate transfer time regularly because I won't affect later reservations, and others could still use my time 
                                            TransferredTime = GetTransferTime(myChargeEndDateTime, TransferChargeBeginDateTime, TransferChargeEndDateTime);
                                        }
                                    }
                                    else
                                    {
                                        //This means my reservation ends earlier than originally reserved.  So I calculate transferred time as the most usual way (I ended early, so user can take over my remaing spot)
                                        TransferredTime = GetTransferTime(myChargeEndDateTime, TransferChargeBeginDateTime, TransferChargeEndDateTime);
                                    }

                                    if (TransferredTime < UnTransferredTime)
                                    {
                                        ExceptionManager exp = new ExceptionManager
                                        {
                                            TimeStamp = DateTime.Now,
                                            ExpName = "TransferredTime is less than UnTransferredTime",
                                            AppName = this.GetType().Assembly.GetName().Name,
                                            FunctionName = "CommonTools-DailyToolData20110401",
                                            CustomData = string.Format("ClientID = {0}, Period = '{1}', ReservationID = {2}, Transfer ReservationID = {3}", myClientID, sDate, myReservationID, eachTransfer["ReservationID"])
                                        };
                                        exp.LogException();

                                        TransferredTime = 0.0;
                                        UnTransferredTime = 0.0;
                                    }
                                    TransferredDuration += TransferredTime - UnTransferredTime;
                                }
                            }


                            //Now we have to handle previous reservation that's possibly delayed
                            //we need to check if there is any transfer time because of previous guy ruuning late on his reservation
                            FilterBase = string.Format("ResourceID = {0} AND ReservationID <> {1} AND ActivityID in (6, 7, 16) AND ChargeMultiplier <> 0 AND IsCancelledBeforeAllowedTime = 0 AND isStarted = 1", myResourceID, myReservationID);
                            FilterTime = string.Format("(ActualEndDateTime > '{0}' AND ActualEndDateTime < '{1}' AND ActualEndDateTime > EndDateTime AND ActualBeginDateTime < '{2}')", BeginDateTime, myChargeEndDateTime, myChargeBeginDateTime);

                            DataRow[] drwsPriorTranferReservations = dtToolDataClean.Select(string.Format("{0} AND {1}", FilterBase, FilterTime), "ChargeBeginDateTime");

                            if (drwsPriorTranferReservations.Length > 0)
                            {
                                if (drwsPriorTranferReservations.Length == 1)
                                {
                                    //We should just have one prior reservation that runs over my reservation, if there are two, I need to log (below)
                                    DateTime priorEndTime = Convert.ToDateTime(drwsPriorTranferReservations[0]["ChargeEndDateTime"]);
                                    if (priorEndTime > myChargeBeginDateTime)
                                        TransferredDuration += (priorEndTime - myChargeBeginDateTime).TotalMinutes;
                                    else
                                    {
                                        ExceptionManager exp = new ExceptionManager
                                        {
                                            TimeStamp = DateTime.Now,
                                            ExpName = "prior overtime reservation affecting others - how do I come here?",
                                            AppName = this.GetType().Assembly.GetName().Name,
                                            FunctionName = "CommonTools-DailyToolData20110401",
                                            CustomData = string.Format("ClientID = {0}, Period = '{1}', ReservationID = {2}, Transfer ReservationID = {3}", myClientID, sDate, myReservationID, drwsPriorTranferReservations[0]["ReservationID"])
                                        };
                                        exp.LogException();
                                    }
                                }
                                else
                                {
                                    //I cannot think of possible scenario where there would be more than one prior reservation overrun, so I log now
                                    foreach (DataRow eachTransfer in drwsPriorTranferReservations)
                                    {
                                        if (Convert.ToDateTime(eachTransfer["ChargeEndDateTime"]) > myChargeBeginDateTime)
                                        {
                                            ExceptionManager exp = new ExceptionManager
                                            {
                                                TimeStamp = DateTime.Now,
                                                ExpName = "prior overtime reservation affecting others - more than 1 prior overruns",
                                                AppName = this.GetType().Assembly.GetName().Name,
                                                FunctionName = "CommonTools-DailyToolData20110401",
                                                CustomData = string.Format("ClientID = {0}, Period = '{1}', ReservationID = {2}, Transfer ReservationID = {3}", myClientID, sDate, myReservationID, eachTransfer["ReservationID"])
                                            };
                                            exp.LogException();
                                        }
                                    }
                                }
                            }

                            //2011-10-31 It's possible that transferredDuration is greater than ChargeDuration due to multiple overruns
                            if (Convert.ToDouble(dr["ChargeDuration"]) < TransferredDuration)
                                dr["TransferredDuration"] = dr["ChargeDuration"];
                            else
                                dr["TransferredDuration"] = TransferredDuration;

                        }
                        else
                            dr["TransferredDuration"] = 0; //Cancelled reservations have no TransferredDuration for sure.
                    }
                }
            }
        }

        private void FillOutputTable(DataTable dtOutput, DataTable dtToolDataClean, DateTime sDate, DateTime eDate)
        {
            foreach (DataRow dr in dtToolDataClean.Rows)
            {
                if (dr.RowState != DataRowState.Deleted)
                {
                    DateTime actDate = Convert.ToDateTime(dr["BeginDateTime"]).Date;
                    if ((actDate >= sDate && actDate < eDate) && Convert.ToDouble(dr["SchedDuration"]) != 0)
                    {
                        DataRow ndr = dtOutput.NewRow();
                        ndr["Period"] = new DateTime(actDate.Year, actDate.Month, 1);
                        ndr["ReservationID"] = dr["ReservationID"];
                        ndr["ClientID"] = dr["ClientID"];
                        ndr["ResourceID"] = dr["ResourceID"];
                        ndr["RoomID"] = dr["RoomID"];
                        ndr["ActDate"] = actDate;
                        ndr["AccountID"] = dr["AccountID"];
                        ndr["Uses"] = dr["Uses"];
                        ndr["SchedDuration"] = dr["SchedDuration"];
                        ndr["ActDuration"] = dr["ActDuration"];
                        ndr["ChargeDuration"] = dr["ChargeDuration"];
                        ndr["TransferredDuration"] = dr["TransferredDuration"];
                        ndr["MaxReservedDuration"] = dr["MaxReservedDuration"];
                        ndr["Overtime"] = dr["Overtime"];
                        ndr["IsStarted"] = dr["IsStarted"];
                        ndr["IsActive"] = dr["IsActive"];
                        ndr["IsCancelledBeforeAllowedTime"] = dr["IsCancelledBeforeAllowedTime"];
                        ndr["ChargeMultiplier"] = dr["ChargeMultiplier"];
                        ndr["ChargeBeginDateTime"] = dr["ChargeBeginDateTime"];
                        ndr["ChargeEndDateTime"] = dr["ChargeEndDateTime"];
                        dtOutput.Rows.Add(ndr);
                    }
                }
            }
        }

        private DataTable GetReservationHistory(DateTime period)
        {
            using (var dba = DA.Current.GetAdapter())
                return dba.ApplyParameters(new { Action = "ForToolDataGeneration", Period = period }).FillDataTable("sselScheduler.dbo.procReservationHistorySelect");
        }

        private double GetTransferTime(DateTime myChargeEndDateTime, DateTime TransferChargeBeginDateTime, DateTime TransferChargeEndDateTime)
        {
            //The idea here is it's possible that other people's reservation could end before or after the current reservation, this requires different way of calculating the sum
            if (TransferChargeEndDateTime < myChargeEndDateTime)
            {
                //it's possible that other reservation is fully inside the time range of current reservation
                return (TransferChargeEndDateTime - TransferChargeBeginDateTime).TotalMinutes;
            }
            else
            {
                //The two reservations overlaps, but current reservation ends earlier than the other's reservation
                double temp = (myChargeEndDateTime - TransferChargeBeginDateTime).TotalMinutes;
                if (temp < 0) temp = 0;
                return temp;
            }
        }

        public DataTable ReadToolUtilization(string sumCol, bool includeForgiven, DateTime sDate, DateTime eDate)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                ds = dba.ApplyParameters(new
                {
                    Action = "Utilization",
                    sumCol = sumCol,
                    sDate = sDate,
                    eDate = eDate,
                    IncludeForgiven = includeForgiven
                }).FillDataSet("ToolDataClean_Select");
                return ds.Tables[0];
            }
        }

        public DataTable ReadToolDataClean(DateTime sDate, DateTime eDate, int clientId = 0, int resourceId = 0)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.SelectCommand
                    .AddParameter("@Action", "ByDateRange")
                    .AddParameter("@sDate", sDate)
                    .AddParameter("@eDate", eDate)
                    .AddParameterIf("@ClientID", clientId > 0, clientId)
                    .AddParameterIf("@ResourceID", resourceId > 0, resourceId);

                ds = dba.FillDataSet("ToolDataClean_Select");

                return ds.Tables[0];
            }
        }

        public DataTable ReadToolDataFiltered(DateTime sDate, DateTime eDate, int clientId = 0, int resourceId = 0)
        {
            //if a ActDuration is longer than the max schedulable, chop it
            DataTable dt = ReadToolDataRaw(sDate, eDate, clientId, resourceId);

            // this is no longer needed because the OverTime check is now done in the stored procedure (sselScheduler.dbo.SSEL_DataRead)
            //foreach (DataRow drReserv in dt.Rows)
            //{
            //    //2009-08-02 Sandrine said there is no upper limit on any reservation, so i have to comment out this code

            //    if (drReserv["OverTime"] != DBNull.Value)
            //    {
            //        if (Convert.ToDouble(drReserv["OverTime"]) < 0.0)
            //            drReserv["OverTime"] = 0.0;
            //    }
            //    else
            //        drReserv["OverTime"] = 0.0;
            //}

            return dt;
        }

        //ResourceID currently does nothing
        public DataTable ReadToolDataRaw(DateTime sDate, DateTime eDate, int clientId = 0, int resourceId = 0)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                dba.SelectCommand
                    .AddParameter("@Action", "ToolDataRaw")
                    .AddParameter("@sDate", sDate)
                    .AddParameter("@eDate", eDate)
                    .AddParameterIf("@ClientID", clientId > 0, clientId)
                    .AddParameterIf("@ResourceID", resourceId > 0, resourceId);

                ds = dba.FillDataSet("sselScheduler_Select");

                return ds.Tables[0];
            }
        }
    }
}
