using LNF.Billing.Process;
using LNF.CommonTools;
using LNF.DataAccess;
using System;
using System.Data;
using System.Data.SqlClient;

namespace LNF.Impl.Billing
{
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
        public int ClientID { get; }

        public WriteToolDataCleanProcess(SqlConnection conn, DateTime sd, DateTime ed, int clientId = 0) : base(conn)
        {
            StartDate = sd;
            EndDate = ed;
            ClientID = clientId;
        }

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
                cmd.Parameters.AddWithValue("sDate", StartDate);
                cmd.Parameters.AddWithValue("eDate", EndDate);
                AddParameterIf(cmd, "ClientID", ClientID > 0, ClientID);
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
