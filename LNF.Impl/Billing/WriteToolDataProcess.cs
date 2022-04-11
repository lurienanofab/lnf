using LNF.Billing;
using LNF.Billing.Process;
using LNF.CommonTools;
using LNF.Data;
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
        public IEnumerable<ICost> Costs { get; set; }

        public static WriteToolDataConfig Create(SqlConnection conn, string context, DateTime period, int clientId, int resourceId, IEnumerable<ICost> costs)
        {
            return new WriteToolDataConfig
            {
                Connection = conn,
                Context = context,
                Period = period,
                ClientID = clientId,
                ResourceID = resourceId,
                Costs = costs
            };
        }
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
        private readonly WriteToolDataConfig _config;

        public int ResourceID => _config.ResourceID;
        public IEnumerable<ICost> Costs => _config.Costs;

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
            var transformer = GetTransformer();
            var dtTransform = transformer.DailyToolData(dtExtract);
            return dtTransform;
        }

        public ToolDataTransformer GetTransformer()
        {
            var dtReservationHistory = GetReservationHistory();
            var dtActivities = GetActivities();

            if (Period < ToolDataTransformer20110401.NewBillingDate)
                return new ToolDataTransformerOld(Period, ClientID, ResourceID, dtActivities);
            else
                return new ToolDataTransformer20110401(Period, ClientID, ResourceID, dtReservationHistory, dtActivities, Costs);
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

        public DataTable GetActivities()
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

        public DataTable GetReservationHistory()
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
