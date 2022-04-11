using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace LNF.Billing
{
    public class UserUsageAudit
    {
        private DataSet _ds;

        public IProvider Provider { get; }

        public UserUsageAudit(IProvider provider)
        {
            Provider = provider;
        }

        public void GetAuditData(DateTime period, int clientId)
        {
            /*
                ChargeDuration is originally calculated in LNF.Impl.Billing.WriteToolDataProcess.ProcessCleanData
                The actual calculation is:
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

                Notes:
                    * ToolDataClean.SchedDuration is copied directly from Reservation.Duration (not computed when ToolDataClean is populated)
                    * EndDateTime adds SchedDuration to BeginDateTime instead of using ToolDataClean.EndDateTime or Reservation.EndDateTime,
                        and ActualEndDateTime is subsequently determined by this instead of using value in ToolDataClean or Reservation
                    * chargeDuration is TotalMinutes from a TimeSpan object

                UncancelledPenaltyPercentage is calculated when rows are selected from ToolData for populating ToolBilling using:
                    (SELECT c2.MulVal FROM @costs c2 WHERE c2.TableNameOrDescript = 'ToolMissedReservCost' AND c2.ChargeTypeID = ot.ChargeTypeID)
                    [zero since April 2011]

                UncancelledPenaltyFee is a computed column in ToolBilling using:
                    (CONVERT([decimal](19,4),((([ChargeMultiplier]*([SchedDuration]/(60)))*[ResourceRate])*[UncancelledPenaltyPercentage])*((1)-[IsStarted]),(0)))
                    [zero since April 2011]

                OvertimePenaltyPercentage is calculated when rows are selected from ToolData for populating ToolBilling using:
                    (SELECT c2.MulVal + 1.0 FROM @costs c2 WHERE c2.TableNameOrDescript = 'ToolOvertimeCost' AND c2.ChargeTypeID = ot.ChargeTypeID)
                    [this is currently 1.25]

                OverTimePenaltyFee is a computed column in ToolBilling using:
                    (CONVERT([decimal](19,4),(([ChargeMultiplier]*([OverTime]/(60)))*[ResourceRate])*[OverTimePenaltyPercentage],(0)))
            */

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["cnSselData"].ConnectionString))
            using (var cmd = new SqlCommand("dbo.Report_AuditUserUsageSummary", conn) { CommandType = CommandType.StoredProcedure })
            using (var adap = new SqlDataAdapter(cmd))
            {
                cmd.Parameters.AddWithValue("ClientID", clientId);
                cmd.Parameters.AddWithValue("Period", period);

                _ds = new DataSet();
                adap.Fill(_ds);

                _ds.Tables[0].TableName = "ToolBilling";
                _ds.Tables[1].TableName = "Client";

                ProcessToolBillingTable();
            }
        }

        public DataTable GetClientTable()
        {
            return _ds.Tables["Client"];
        }

        public DataTable GetDetailTable(int resourceId)
        {
            var dtToolBilling = _ds.Tables["ToolBilling"];
            var result = CopyTable(dtToolBilling, $"ResourceID = {resourceId}", "ReservationID ASC");
            return result;
        }

        public DataTable GetAggregateTable()
        {
            var dtAgg = AggregateByResource();
            var result = CopyTable(dtAgg, null, "LabDisplayName, ResourceDisplayName, AccountName");
            return result;
        }

        private DataTable CopyTable(DataTable dt, string filter, string sort)
        {
            var result = dt.Clone();
            var rows = dt.Select(filter, sort);
            foreach (DataRow dr in rows)
                result.Rows.Add(dr.ItemArray);
            return result;
        }

        private void ProcessToolBillingTable()
        {
            var dtToolBilling = _ds.Tables["ToolBilling"];

            dtToolBilling.Columns.Add("ResourceDisplayName", typeof(string));
            dtToolBilling.Columns.Add("PerUseCharge", typeof(decimal));
            dtToolBilling.Columns.Add("HourlyRate", typeof(decimal));
            dtToolBilling.Columns.Add("ForgivenPercentage", typeof(decimal));
            dtToolBilling.Columns.Add("SchedDurationHours", typeof(decimal));
            dtToolBilling.Columns.Add("SchedDurationHoursForgiven", typeof(decimal));
            dtToolBilling.Columns.Add("ChargeDurationHours", typeof(decimal));
            dtToolBilling.Columns.Add("ChargeDurationHoursForgiven", typeof(decimal));
            dtToolBilling.Columns.Add("BaseDuration", typeof(decimal));
            dtToolBilling.Columns.Add("BaseDurationHours", typeof(decimal));
            dtToolBilling.Columns.Add("BaseDurationHoursForgiven", typeof(decimal));
            dtToolBilling.Columns.Add("OverTimeHours", typeof(decimal));
            dtToolBilling.Columns.Add("OverTimeHoursForgiven", typeof(decimal));
            dtToolBilling.Columns.Add("OverTimePenalty", typeof(decimal));
            dtToolBilling.Columns.Add("OverTimePenaltyHours", typeof(decimal));
            dtToolBilling.Columns.Add("OverTimePenaltyHoursForgiven", typeof(decimal));
            dtToolBilling.Columns.Add("BillingDuration", typeof(decimal));
            dtToolBilling.Columns.Add("BillingDurationHours", typeof(decimal));
            dtToolBilling.Columns.Add("BillingDurationHoursForgiven", typeof(decimal));
            dtToolBilling.Columns.Add("BillingCharge", typeof(decimal));
            dtToolBilling.Columns.Add("LineCost", typeof(decimal));

            foreach (DataRow dr in dtToolBilling.Rows)
            {
                SetDataRow(dr);
            }
        }

        private void SetDataRow(DataRow dr)
        {
            // Some of these are stored in the database as float but the IToolBilling object defines
            // them as decimal. Putting all decimal values in a variable for convenience.
            var item = new UserUsageAuditItem
            {
                ResourceID = dr.Field<int>("ResourceID"),
                ResourceName = dr.Field<string>("ResourceName"),
                RoomID = dr.Field<int>("RoomID"),
                BillingTypeID = dr.Field<int>("BillingTypeID"),
                IsStarted = dr.Field<bool>("IsStarted"),
                IsCancelledBeforeAllowedTime = dr.Field<bool>("IsCancelledBeforeAllowedTime"),
                RatePeriod = dr.Field<string>("RatePeriod"),
                Uses = Convert.ToDecimal(dr.Field<double>("Uses")),
                PerUseRate = dr.Field<decimal>("PerUseRate"), // stored as decimal in database
                ResourceRate = dr.Field<decimal>("ResourceRate"), // stored as decimal in database
                ReservationRate = dr.Field<decimal>("ReservationRate"), // stored as decimal in database
                OverTimePenaltyPercentage = Convert.ToDecimal(dr.Field<double>("OverTimePenaltyPercentage")),
                OverTimePenaltyFee = dr.Field<decimal>("OverTimePenaltyFee"), // stored as decimal in the database
                UncancelledPenaltyPercentage = Convert.ToDecimal(dr.Field<double>("UncancelledPenaltyPercentage")),
                UncancelledPenaltyFee = dr.Field<decimal>("UncancelledPenaltyFee"), // stored as decimal in the database
                BookingFee = dr.Field<decimal>("BookingFee"), // stored as decimal in the database
                ReservationFee2 = dr.Field<decimal>("ReservationFee2"), // stored as decimal in the database
                SchedDuration = Convert.ToDecimal(dr.Field<double>("SchedDuration")),
                ChargeDuration = Convert.ToDecimal(dr.Field<double>("ChargeDuration")),
                OverTime = dr.Field<decimal>("OverTime"), // stored as decimal in database
                TransferredDuration = Convert.ToDecimal(dr.Field<double>("TransferredDuration")),
                ChargeMultiplier = Convert.ToDecimal(dr.Field<double>("ChargeMultiplier")),
                UsageFeeCharged = dr.Field<decimal>("UsageFeeCharged"), // stored as decimal in database
            };

            var auditResult = GetUserUsageAuditResult(item);

            dr["ResourceDisplayName"] = auditResult.ResourceDisplayName;
            dr["PerUseCharge"] = auditResult.PerUseCharge;
            dr["HourlyRate"] = auditResult.HourlyRate;
            dr["ForgivenPercentage"] = auditResult.ForgivenPercentage;
            dr["SchedDurationHours"] = auditResult.SchedDurationHours;
            dr["SchedDurationHoursForgiven"] = auditResult.SchedDurationHoursForgiven;
            dr["ChargeDurationHours"] = auditResult.ChargeDurationHours;
            dr["ChargeDurationHoursForgiven"] = auditResult.ChargeDurationHoursForgiven;
            dr["BaseDuration"] = auditResult.BaseDuration;
            dr["BaseDurationHours"] = auditResult.BaseDurationHours;
            dr["BaseDurationHoursForgiven"] = auditResult.BaseDurationHoursForgiven;
            dr["OverTimeHours"] = auditResult.OverTimeHours;
            dr["OverTimeHoursForgiven"] = auditResult.OverTimeHoursForgiven;
            dr["OverTimePenalty"] = auditResult.OverTimePenalty;
            dr["OverTimePenaltyHours"] = auditResult.OverTimePenaltyHours;
            dr["OverTimePenaltyHoursForgiven"] = auditResult.OverTimePenaltyHoursForgiven;
            dr["BillingDuration"] = auditResult.BillingDuration;
            dr["BillingDurationHours"] = auditResult.BillingDurationHours;
            dr["BillingDurationHoursForgiven"] = auditResult.BillingDurationHoursForgiven;
            dr["BillingCharge"] = auditResult.BillingCharge;
            dr["LineCost"] = auditResult.LineCost;

            // for a typical reservation:
            //      if IsCancelledBeforeAllowedTime:
            //          result = bookingFee
            //      not IsCancelledBeforeAllowedTime:
            //          result = usageFeeCharged + overTimePenaltyFee + bookingFee (bookingFee will be zero)
            //var lineCost = Provider.Billing.Tool.GetLineCost(new ToolLineCostParameters
            //{
            //    Period = period,
            //    ResourceID = resourceId,
            //    RoomID = roomId,
            //    BillingTypeID = billingTypeId,
            //    IsStarted = isStarted,
            //    ResourceRate = item.ResourceRate,
            //    PerUseRate = item.PerUseRate,
            //    UsageFeeCharged = item.UsageFeeCharged,
            //    OverTimePenaltyFee = item.OverTimePenaltyFee,
            //    UncancelledPenaltyFee = item.UncancelledPenaltyFee,
            //    BookingFee = item.BookingFee,
            //    ReservationFee2 = item.ReservationFee2,
            //    IsCancelledBeforeAllowedTime = isCancelledBeforeAllowedTime
            //});
        }

        public static UserUsageAuditResult GetUserUsageAuditResult(UserUsageAuditItem item)
        {
            // This is how UsageFeeCharged is calculated:
            //      if IsCancelledBeforeAllowedTime:
            //          result = 0
            //      not IsCancelledBeforeAllowedTime:
            //          result = (PerUseRate + RatePeriodCharge) * ChargeMultiplier
            //
            // PerUseRate is Cost.AddVal
            // RatePeriodCharge is based on baseDuration (see below), Cost.AcctPer, and Cost.MulVal

            // unrounded decimal value
            decimal hourlyDuration;

            var perUseCharge = Round(item.PerUseRate * item.Uses * item.ChargeMultiplier);

            hourlyDuration = item.SchedDuration / 60M;
            var schedDurationHours = Round(hourlyDuration);
            var schedDurationHoursForgiven = Round(hourlyDuration * item.ChargeMultiplier);

            hourlyDuration = item.ChargeDuration / 60M;
            var chargeDurationHours = Round(hourlyDuration);
            var chargeDurationHoursForgiven = Round(hourlyDuration * item.ChargeMultiplier);

            // same calculation used in LNF.Impl.Billing.ToolBillingUtility.CalculateUsageFeeCharged
            var baseDuration = item.ChargeDuration - item.OverTime - item.TransferredDuration;

            hourlyDuration = baseDuration / 60M;
            var baseDurationHours = Round(hourlyDuration);
            var baseDurationHoursForgiven = Round(hourlyDuration * item.ChargeMultiplier);

            hourlyDuration = item.OverTime / 60M;
            var overTimeHours = Round(hourlyDuration);
            var overTimeHoursForgiven = Round(hourlyDuration * item.ChargeMultiplier);

            var overTimePenalty = item.OverTime * item.OverTimePenaltyPercentage;

            hourlyDuration = overTimePenalty / 60M;
            var overTimePenaltyHours = Round(hourlyDuration);
            var overTimePenaltyHoursForgiven = Round(hourlyDuration * item.ChargeMultiplier);

            // this is the same method used to populate LineCost elsewhere
            var forgivenPercentage = 1 - item.ChargeMultiplier;
            var hourlyRate = GetHourlyRate(item.RatePeriod, item.ResourceRate);

            // To be able to calculate the line cost using only the values in each row (this is the goal here) we must set BaseDuration
            // to zero when IsCancelledBeforeAllowedTime is true. This is because for these reservations there is only a booking fee.
            if (item.IsCancelledBeforeAllowedTime)
            {
                baseDuration = 0;
                baseDurationHours = 0;
                baseDurationHoursForgiven = 0;
            }

            // This is a new concept for this report: the sum of base duration plus overtime multiplied by the overtime penalty.
            // The purpose of this is to have one duration that can be multiplied by the hourly rate to get the line cost.
            var billingDuration = baseDuration + overTimePenalty; // zero when cancelled before cutoff

            hourlyDuration = billingDuration / 60M;
            var billingDurationHours = Round(hourlyDuration);
            var billingDurationHoursForgiven = Round(hourlyDuration * item.ChargeMultiplier);

            // includes overtime penalty duration and charge duration
            var billingCharge = Round(hourlyRate * billingDurationHoursForgiven);

            // for a typical reservation:
            //      if IsCancelledBeforeAllowedTime:
            //          result = bookingFee
            //      not IsCancelledBeforeAllowedTime:
            //          result = usageFeeCharged + overTimePenaltyFee + bookingFee (bookingFee will be zero)
            //var lineCost = Provider.Billing.Tool.GetLineCost(new ToolLineCostParameters
            //{
            //    Period = period,
            //    ResourceID = resourceId,
            //    RoomID = roomId,
            //    BillingTypeID = billingTypeId,
            //    IsStarted = isStarted,
            //    ResourceRate = item.ResourceRate,
            //    PerUseRate = item.PerUseRate,
            //    UsageFeeCharged = item.UsageFeeCharged,
            //    OverTimePenaltyFee = item.OverTimePenaltyFee,
            //    UncancelledPenaltyFee = item.UncancelledPenaltyFee,
            //    BookingFee = item.BookingFee,
            //    ReservationFee2 = item.ReservationFee2,
            //    IsCancelledBeforeAllowedTime = isCancelledBeforeAllowedTime
            //});

            var result = new UserUsageAuditResult
            {
                ResourceID = item.ResourceID,
                ResourceDisplayName = $"{item.ResourceName} [{item.ResourceID}]",
                BookingFee = item.BookingFee, //aready forgiven via ToolBilling import process
                PerUseCharge = perUseCharge,
                HourlyRate = hourlyRate,
                ForgivenPercentage = forgivenPercentage,
                SchedDurationHours = schedDurationHours,
                SchedDurationHoursForgiven = schedDurationHoursForgiven,
                ChargeDurationHours = chargeDurationHours,
                ChargeDurationHoursForgiven = chargeDurationHoursForgiven,
                BaseDuration = baseDuration,
                BaseDurationHours = baseDurationHours,
                BaseDurationHoursForgiven = baseDurationHoursForgiven,
                OverTimeHours = overTimeHours,
                OverTimeHoursForgiven = overTimeHoursForgiven,
                OverTimePenalty = overTimePenalty,
                OverTimePenaltyHours = overTimePenaltyHours,
                OverTimePenaltyHoursForgiven = overTimePenaltyHoursForgiven,
                BillingDuration = billingDuration,
                BillingDurationHours = billingDurationHours,
                BillingDurationHoursForgiven = billingDurationHoursForgiven,
                BillingCharge = billingCharge
            };

            return result;
        }

        private static decimal Round(decimal d, int decimals = 2)
        {
            //return d;
            return Math.Round(d, decimals);
            //return Math.Round(d, decimals, MidpointRounding.AwayFromZero);
        }

        private DataTable AggregateByResource()
        {
            var dtToolBilling = _ds.Tables["ToolBilling"];

            //return dtToolBilling;
            var dt = dtToolBilling.Clone();

            string[] sumColumns = new[]
            {
                "Uses",
                "PerUseCharge",
                "BaseDuration",
                "BaseDurationHours",
                "BaseDurationHoursForgiven",
                "OverTime",
                "OverTimeHours",
                "OverTimeHoursForgiven",
                "OverTimePenalty",
                "OverTimePenaltyHours",
                "OverTimePenaltyHoursForgiven",
                "BillingDuration",
                "BillingDurationHours",
                "BillingDurationHoursForgiven",
                "BillingCharge",
                "UsageFeeCharged",
                "OverTimePenaltyFee",
                "BookingFee",
                "LineCost"
            };

            foreach (DataRow dr in dtToolBilling.Rows)
            {
                var existing = dt.Select($"ResourceID = {dr["ResourceID"]} AND AccountID = {dr["AccountID"]}");
                DataRow drExisting;

                if (existing.Length == 0)
                {
                    drExisting = dt.Rows.Add(dr.ItemArray);
                    drExisting["ReservationID"] = 0;
                    drExisting["ChargeMultiplier"] = 0;
                    drExisting["ForgivenPercentage"] = 0;

                    foreach (var col in sumColumns)
                        drExisting[col] = 0;
                }
            }

            // now we have a table with distinct resource/account rows

            foreach (DataRow dr in dt.Rows)
            {
                foreach (var col in sumColumns)
                {
                    var sum = dtToolBilling.Compute($"SUM({col})", $"ResourceID = {dr["ResourceID"]} AND AccountID = {dr["AccountID"]}");

                    var dc = dt.Columns[col];

                    if (dc.DataType == typeof(double))
                        dr[col] = Convert.ToDouble(sum);
                    else if (dc.DataType == typeof(decimal))
                        dr[col] = Convert.ToDecimal(sum);
                    else if (dc.DataType == typeof(int))
                        dr[col] = Convert.ToInt32(sum);
                    else
                        throw new Exception($"Cannot sum type: {dc.DataType}");
                }
            }

            return dt;
        }

        private static decimal GetHourlyRate(string ratePeriod, decimal resourceRate)
        {
            switch (ratePeriod)
            {
                case "Hourly":
                    return resourceRate;
                case "None":
                    return 0;
                default:
                    throw new NotSupportedException("unhandled ratePeriod: " + ratePeriod);
            }
        }
    }

    public class UserUsageAuditItem
    {
        public int ResourceID { get; set; }
        public string ResourceName { get; set; }
        public int RoomID { get; set; }
        public int BillingTypeID { get; set; }
        public bool IsStarted { get; set; }
        public bool IsCancelledBeforeAllowedTime { get; set; }
        public string RatePeriod { get; set; }
        public decimal Uses { get; set; }
        public decimal PerUseRate { get; set; }
        public decimal ResourceRate { get; set; }
        public decimal ReservationRate { get; set; }
        public decimal OverTimePenaltyPercentage { get; set; }
        public decimal OverTimePenaltyFee { get; set; }
        public decimal UncancelledPenaltyPercentage { get; set; }
        public decimal UncancelledPenaltyFee { get; set; }
        public decimal BookingFee { get; set; }
        public decimal ReservationFee2 { get; set; }
        public decimal SchedDuration { get; set; }
        public decimal ChargeDuration { get; set; }
        public decimal OverTime { get; set; }
        public decimal TransferredDuration { get; set; }
        public decimal ChargeMultiplier { get; set; }
        public decimal UsageFeeCharged { get; set; }
    }

    public class UserUsageAuditResult
    {
        public int ResourceID { get; set; }
        public string ResourceDisplayName { get; set; }
        public decimal BookingFee { get; set; }
        public decimal PerUseCharge { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal ForgivenPercentage { get; set; }
        public decimal SchedDurationHours { get; set; }
        public decimal SchedDurationHoursForgiven { get; set; }
        public decimal ChargeDurationHours { get; set; }
        public decimal ChargeDurationHoursForgiven { get; set; }
        public decimal BaseDuration { get; set; }
        public decimal BaseDurationHours { get; set; }
        public decimal BaseDurationHoursForgiven { get; set; }
        public decimal OverTimeHours { get; set; }
        public decimal OverTimeHoursForgiven { get; set; }
        public decimal OverTimePenalty { get; set; }
        public decimal OverTimePenaltyHours { get; set; }
        public decimal OverTimePenaltyHoursForgiven { get; set; }
        public decimal BillingDuration { get; set; }
        public decimal BillingDurationHours { get; set; }
        public decimal BillingDurationHoursForgiven { get; set; }
        public decimal BillingCharge { get; set; }

        // these values have already been adjusted for cancelled before cutoff, forgiveness, and rounded to two decimals
        public decimal LineCost => BookingFee + PerUseCharge + BillingCharge;
    }
}
