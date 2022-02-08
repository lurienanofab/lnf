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

                ProcessToolBillingTable(period);
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

        private void ProcessToolBillingTable(DateTime period)
        {
            var c = Provider.Data.Client.GetClient(3194);

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
                // Some of these are stored in the database as float but the IToolBilling object defines
                // them as decimal. Putting all decimal values in a variable for convenience.
                var item = new
                {
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

                // This is how UsageFeeCharged is calculated:
                //      if IsCancelledBeforeAllowedTime:
                //          result = 0
                //      not IsCancelledBeforeAllowedTime:
                //          result = (PerUseRate + RatePeriodCharge) * ChargeMultiplier
                //
                // PerUseRate is Cost.AddVal
                // RatePeriodCharge is based on baseDuration (see below), Cost.AcctPer, and Cost.MulVal)

                var perUseCharge = item.PerUseRate * item.Uses;

                var schedDurationHours = item.SchedDuration / 60M;
                var schedDurationHoursForgiven = schedDurationHours * item.ChargeMultiplier;

                var chargeDurationHours = item.ChargeDuration / 60M;
                var chargeDurationHoursForgiven = chargeDurationHours * item.ChargeMultiplier;

                // same calculation used in LNF.Impl.Billing.ToolBillingUtility.CalculateUsageFeeCharged
                var baseDuration = item.ChargeDuration - item.OverTime - item.TransferredDuration;
                var baseDurationHours = baseDuration / 60M;
                var baseDurationHoursForgiven = baseDurationHours * item.ChargeMultiplier;
                var ratePeriod = dr.Field<string>("RatePeriod");

                var overTimeHours = item.OverTime / 60M;
                var overTimeHoursForgiven = overTimeHours * item.ChargeMultiplier;

                var overTimePenalty = item.OverTime * item.OverTimePenaltyPercentage;
                var overTimePenaltyHours = overTimePenalty / 60M;
                var overTimePenaltyHoursForgiven = overTimePenaltyHours * item.ChargeMultiplier;

                // this is the same method used to populate LineCost elsewhere
                var resourceId = dr.Field<int>("ResourceID");
                var resourceName = dr.Field<string>("ResourceName");
                var roomId = dr.Field<int>("RoomID");
                var billingTypeId = dr.Field<int>("BillingTypeID");
                var isStarted = dr.Field<bool>("IsStarted");
                var isCancelledBeforeAllowedTime = dr.Field<bool>("IsCancelledBeforeAllowedTime");
                var forgivenPercentage = 1 - item.ChargeMultiplier;
                var hourlyRate = GetHourlyRate(ratePeriod, item.ResourceRate);

                // To be able to calculate the line cost using only the values in each row (this is the goal here) we must set BaseDuration
                // to zero when IsCancelledBeforeAllowedTime is true. This is because for these reservations there is only a booking fee.
                if (isCancelledBeforeAllowedTime)
                {
                    baseDuration = 0;
                    baseDurationHours = 0;
                    baseDurationHoursForgiven = 0;
                }

                // This is a new concept for this report: the sum of base duration plus overtime multiplied by the overtime penalty.
                // The purpose of this is to have one duration that can be multiplied by the hourly rate to get the line cost.
                var billingDuration = baseDuration + overTimePenalty; // zero when cancelled before cutoff
                var billingDurationHours = billingDuration / 60M;
                var billingDurationHoursForgiven = billingDurationHours * item.ChargeMultiplier;

                // includes overtime penalty duration and charge duration
                var billingCharge = hourlyRate * billingDurationHoursForgiven;

                // for a typical reservation:
                //      if IsCancelledBeforeAllowedTime:
                //          result = bookingFee
                //      not IsCancelledBeforeAllowedTime:
                //          result = usageFeeCharged + overTimePenaltyFee + bookingFee (bookingFee will be zero)
                var lineCost = Provider.Billing.Tool.GetLineCost(new ToolLineCostParameters
                {
                    Period = period,
                    ResourceID = resourceId,
                    RoomID = roomId,
                    BillingTypeID = billingTypeId,
                    IsStarted = isStarted,
                    ResourceRate = item.ResourceRate,
                    PerUseRate = item.PerUseRate,
                    UsageFeeCharged = item.UsageFeeCharged,
                    OverTimePenaltyFee = item.OverTimePenaltyFee,
                    UncancelledPenaltyFee = item.UncancelledPenaltyFee,
                    BookingFee = item.BookingFee,
                    ReservationFee2 = item.ReservationFee2,
                    IsCancelledBeforeAllowedTime = isCancelledBeforeAllowedTime
                });

                dr["ResourceDisplayName"] = $"{resourceName} [{resourceId}]";
                dr["PerUseCharge"] = perUseCharge;
                dr["HourlyRate"] = hourlyRate;
                dr["ForgivenPercentage"] = forgivenPercentage;
                dr["SchedDurationHours"] = schedDurationHours;
                dr["SchedDurationHoursForgiven"] = schedDurationHoursForgiven;
                dr["ChargeDurationHours"] = chargeDurationHours;
                dr["ChargeDurationHoursForgiven"] = chargeDurationHoursForgiven;
                dr["BaseDuration"] = baseDuration;
                dr["BaseDurationHours"] = baseDurationHours;
                dr["BaseDurationHoursForgiven"] = baseDurationHoursForgiven;
                dr["OverTimeHours"] = overTimeHours;
                dr["OverTimeHoursForgiven"] = overTimeHoursForgiven;
                dr["OverTimePenalty"] = overTimePenalty;
                dr["OverTimePenaltyHours"] = overTimePenaltyHours;
                dr["OverTimePenaltyHoursForgiven"] = overTimePenaltyHoursForgiven;
                dr["BillingDuration"] = billingDuration;
                dr["BillingDurationHours"] = billingDurationHours;
                dr["BillingDurationHoursForgiven"] = billingDurationHoursForgiven;
                dr["BillingCharge"] = Round(billingCharge);
                dr["LineCost"] = lineCost;
            }
        }

        private decimal Round(decimal d, int decimals = 2) => Math.Round(d, decimals, MidpointRounding.AwayFromZero);

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

        private decimal GetHourlyRate(string ratePeriod, decimal resourceRate)
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
}
