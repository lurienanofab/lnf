using LNF.Billing;
using LNF.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Impl.Billing
{
    public class ToolDataTransformer20110401 : ToolDataTransformer
    {
        public static readonly DateTime NewBillingDate = new DateTime(2011, 4, 1);

        private readonly IEnumerable<ICost> _costs;
        private ReservationDurations _durations;

        public DataTable ReservationHistoryTable { get; }

        public ToolDataTransformer20110401(DateTime period, int clientId, int resourceId, DataTable dtReservationHistory, DataTable dtActivities, IEnumerable<ICost> costs) : base(period, clientId, resourceId, dtActivities)
        {
            ReservationHistoryTable = dtReservationHistory;
            _costs = costs;
        }

        //This list of reservations were cancelled before 2011-04-01 so they should only be charged with booking fees
        //Delete this ONLY when there is no need to regenerate the April 2011 data
        public override int[] CancelledReservations() => new int[] { 286867, 302663, 302703, 303807, 303818, 303856, 302876, 303041, 303155, 303156, 303168, 303288, 303444, 303534, 303556, 303697, 303735, 303787 };

        //This handles generation of toolData table since 2011-04-01
        public override void ProcessCleanData(DataTable dtToolDataClean)
        {
            // must be done before any changes are made to the table, this will be used later to calculate transfer durations
            _durations = GetReservationDurations(dtToolDataClean);

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
            //DataTable dtResHistory = GetReservationHistory();

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

                                // [2022-03-25 jg] Rounding to two digits now
                                RoundDouble(adr, "ActDuration");
                                RoundDouble(adr, "ChargeDuration");
                                RoundDouble(adr, "OverTime");
                                RoundDouble(adr, "MaxReservedDuration");

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
                                DataRow[] drsResHistory = ReservationHistoryTable.Select($"ReservationID = {dr["ReservationID"]}");

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

                            // [2022-03-25 jg] Rounding to two digits now
                            RoundDouble(dr, "ActDuration");
                            RoundDouble(dr, "ChargeDuration");
                            RoundDouble(dr, "OverTime");
                            RoundDouble(dr, "MaxReservedDuration");
                        }
                    }
                    else
                        dr.Delete(); //not chargeable, so remove
                }
            }
        }

        public override void CalculateTransferTime(DataTable dtToolDataClean)
        {
            if (_durations == null)
                throw new Exception("_durations is null");

            // [2016-05-17 jg] major refactoring to make things a lot more simple

            DataRow[] rows = dtToolDataClean.AsEnumerable().Where(x => x.RowState != DataRowState.Deleted && (ClientID == 0 || ClientID == x.Field<int>("ClientID"))).ToArray();

            // loop through reservations and get the TransferredDuration quantity for each
            foreach (DataRow dr in rows)
            {
                int reservationId = dr.Field<int>("ReservationID");
                int resourceId = dr.Field<int>("ResourceID");
                DateTime chargeBeginDateTime = dr.Field<DateTime>("ChargeBeginDateTime");
                DateTime chargeEndDateTime = dr.Field<DateTime>("ChargeEndDateTime");

                var item = _durations.FirstOrDefault(x => x.Reservation.ReservationID == reservationId);

                var uses = dr.Field<double>("Uses");

                if (item != null)
                {
                    // [2018-08-01 jg] Occasionally item.TransferredDuration.TotalMinutes is a very small negative
                    //      number, never greater than -0.0001. I think we can safely assume these should be zero.
                    dr.SetField("TransferredDuration", Math.Max(item.TransferredDuration.TotalMinutes, 0) * uses);
                }
                else
                    dr.SetField("TransferredDuration", 0.0);

                // [2022-03-25 jg] Rounding to two digits now
                RoundDouble(dr, "TransferredDuration");
            }
        }

        public bool IsCancelledBeforeAllowedTime(DataRow dr)
        {
            var reservationId = dr.Field<int>("ReservationID");
            var beginDateTime = dr.Field<DateTime>("BeginDateTime");
            var cancelledDateTime = dr.Field<DateTime?>("CancelledDateTime");

            if (!cancelledDateTime.HasValue)
            {
                //for accuracy in generating 2011-04-01 tool data, this checking below is necessary
                if (CancelledReservations().FirstOrDefault(x => reservationId == x) > 0 && Period.Year == NewBillingDate.Year && Period.Month == NewBillingDate.Month)
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

        public ReservationDurations GetReservationDurations(DataTable dtToolDataClean)
        {
            var reservations = GetReservationDateRangeItems(dtToolDataClean);
            var range = new ReservationDateRange(reservations);
            var result = new ReservationDurations(range);
            return result;
        }

        private void RoundDouble(DataRow dr, string col, int decimals = 2)
        {
            var d = 0D;
            if (dr[col] != DBNull.Value)
                d = Convert.ToDouble(dr[col]);
            var rounded = Math.Round(d, decimals);
            dr[col] = rounded;
        }

        private IEnumerable<ReservationDateRangeItem> GetReservationDateRangeItems(DataTable dtToolDataClean)
        {
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
                    _costs));
            }

            return result;
        }
    }
}
