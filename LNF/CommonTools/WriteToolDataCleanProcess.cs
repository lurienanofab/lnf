using LNF.Models.Billing.Process;
using LNF.Repository;
using System;
using System.Data;

namespace LNF.CommonTools
{
    /// <summary>
    /// This process will:
    ///     1) Select records from Scheduler in the date range to insert.
    ///     2) Delete records from ToolDataClean in the date range.
    ///     3) Insert records from Scheduler into ToolDataClean.
    /// </summary>
    public class WriteToolDataCleanProcess : ProcessBase<WriteToolDataCleanProcessResult>
    {
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public int ClientID { get; }

        public WriteToolDataCleanProcess(DateTime sd, DateTime ed, int clientId = 0)
        {
            StartDate = sd;
            EndDate = ed;
            ClientID = clientId;
        }

        public override int DeleteExisting()
        {
            return DA.Command()
                .Param("sDate", StartDate)
                .Param("eDate", EndDate)
                .Param("ClientID", ClientID > 0, ClientID)
                .ExecuteNonQuery("dbo.ToolDataClean_Delete").Value;
        }

        public override DataTable Extract()
        {
            return ReadData.Tool.ReadToolDataRaw(StartDate, EndDate, ClientID);
        }

        public override IBulkCopy CreateBulkCopy()
        {
            IBulkCopy bcp = DA.Current.GetBulkCopy("dbo.ToolDataClean");
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
            return bcp;
        }
    }
}
