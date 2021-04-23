using LNF.Billing.Process;
using LNF.CommonTools;
using LNF.DataAccess;
using System;
using System.Data;
using System.Data.SqlClient;

namespace LNF.Impl.Billing
{
    public class WriteToolDataCleanConfig : RangeProcessConfig { }

    /// <summary>
    /// This process will:
    ///     1) Select records from Scheduler in the date range to insert.
    ///     2) Delete records from ToolDataClean in the date range.
    ///     3) Insert records from Scheduler into ToolDataClean.
    /// </summary>
    public class WriteToolDataCleanProcess : ProcessBase<WriteToolDataCleanResult>
    {
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }

        public WriteToolDataCleanProcess(WriteToolDataCleanConfig cfg) : base(cfg)
        {
            StartDate = cfg.StartDate;
            EndDate = cfg.EndDate;
        }

        public override string ProcessName => "ToolDataClean";

        protected override WriteToolDataCleanResult CreateResult()
        {
            return new WriteToolDataCleanResult
            {
                StartDate = StartDate,
                EndDate = EndDate,
                ClientID = ClientID
            };
        }

        public override int DeleteExisting()
        {
            using (var cmd = new SqlCommand("dbo.ToolDataClean_Delete", Connection) { CommandType = CommandType.StoredProcedure })
            {
                AddParameter(cmd, "sDate", StartDate, SqlDbType.DateTime);
                AddParameter(cmd, "eDate", EndDate, SqlDbType.DateTime);
                AddParameterIf(cmd, "ClientID", ClientID > 0, ClientID, SqlDbType.Int);
                AddParameter(cmd, "Context", Context, SqlDbType.NVarChar, 50);
                var result = cmd.ExecuteNonQuery();
                return result;
            }
        }

        public override DataTable Extract()
        {
            var reader = new ToolDataReader(Connection);
            return reader.ReadToolDataRaw(StartDate, EndDate, ClientID);
        }

        public override IBulkCopy CreateBulkCopy()
        {
            IBulkCopy bcp = new DefaultBulkCopy("dbo.ToolDataClean");
            bcp.AddColumnMapping("ReservationID");
            bcp.AddColumnMapping("ClientID");
            bcp.AddColumnMapping("ResourceID");
            bcp.AddColumnMapping("RoomID");
            bcp.AddColumnMapping("BeginDateTime");
            bcp.AddColumnMapping("EndDateTime");
            bcp.AddColumnMapping("ActualBeginDateTime");
            bcp.AddColumnMapping("ActualEndDateTime");
            bcp.AddColumnMapping("AccountID");
            bcp.AddColumnMapping("ActivityID");
            bcp.AddColumnMapping("SchedDuration");
            bcp.AddColumnMapping("ActDuration");
            bcp.AddColumnMapping("OverTime");
            bcp.AddColumnMapping("IsStarted");
            bcp.AddColumnMapping("ChargeMultiplier");
            bcp.AddColumnMapping("IsActive");
            bcp.AddColumnMapping("MaxReservedDuration");
            bcp.AddColumnMapping("CancelledDateTime");
            bcp.AddColumnMapping("OriginalBeginDateTime");
            bcp.AddColumnMapping("OriginalEndDateTime");
            bcp.AddColumnMapping("OriginalModifiedOn");
            bcp.AddColumnMapping("CreatedOn");
            bcp.AddColumnMapping("LastModifiedOn");
            return bcp;
        }
    }
}
