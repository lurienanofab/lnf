using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using LNF.Repository;
using LNF.Logging;

namespace LNF.CommonTools
{
    public class WriteToolDataManager
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ClientID { get; set; }
        public int ResourceID { get; set; }

        // force using the static constructor
        private WriteToolDataManager() { }

        /// <summary>
        /// Creates a new instance of WriteToolDataManager with the given parameters.
        /// </summary>
        public static WriteToolDataManager Create(DateTime startDate, DateTime endDate, int clientId = 0, int resourceId = 0)
        {
            return new WriteToolDataManager()
            {
                StartDate = startDate,
                EndDate = endDate,
                ClientID = clientId,
                ResourceID = resourceId
            };
        }

        /// <summary>
        /// This method will:
        ///     1) Select records from Scheduler in the date range to insert.
        ///     2) Delete records from ToolDataClean in the date range.
        ///     3) Insert records from Scheduler into ToolDataClean.
        /// </summary>
        public void WriteToolDataClean()
        {
            int rowsSelectedFromScheduler = 0;
            int rowsDeletedFromToolDataClean = 0;
            int rowsInsertedIntoToolDataClean = 0;

            using (LogTaskTimer.Start("WriteToolDataManager.WriteToolDataClean", "ClientID = {0}, ResourceID = {1}, StartDate= '{2:yyyy-MM-dd}', EndDate = '{3:yyyy-MM-dd}', RowsSelectedFromScheduler = {4}, RowsDeletedFromToolDataClean = {5}, RowsInsertedIntoToolDataClean = {6}", () => new object[] { ClientID, ResourceID, StartDate, EndDate, rowsSelectedFromScheduler, rowsDeletedFromToolDataClean, rowsInsertedIntoToolDataClean }))
            {
                //write data for all resources at the same time
                ReadToolDataManager reader = new ReadToolDataManager();
                DataTable dtSource = reader.ReadToolDataFiltered(StartDate, EndDate, ClientID, ResourceID);

                rowsSelectedFromScheduler = dtSource.Rows.Count;

                if (rowsSelectedFromScheduler > 0)
                {
                    using (var dba = DA.Current.GetAdapter())
                    {
                        rowsDeletedFromToolDataClean = dba.SelectCommand
                            .AddParameter("@sDate", StartDate)
                            .AddParameter("@eDate", EndDate)
                            .AddParameterIf("@ClientID", ClientID > 0, ClientID)
                            .AddParameterIf("@ResourceID", ResourceID > 0, ResourceID)
                            .ExecuteNonQuery("ToolDataClean_Delete");
                    }

                    DataTable dtClean = Utility.CopyDT(dtSource);

                    //insert the table into the DB
                    using (var bcp = CreateToolDataCleanBulkCopy())
                        bcp.WriteToServer(dtClean);

                    rowsInsertedIntoToolDataClean = dtClean.Rows.Count;
                }
            }
        }

        private IBulkCopy CreateToolDataCleanBulkCopy()
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

        /// <summary>
        /// This method will:
        ///     1) Delete records from ToolData in the date range.
        ///     2) Select records from ToolDataClean in the date range to insert.
        ///     3) Insert records from ToolDataClean records into ToolData.
        ///     4) Adjust records in ToolData.
        /// </summary>
        public void WriteToolData()
        {
            int rowsDeletedFromToolData = 0;
            int rowsSelectedFromToolDataClean = 0;
            int rowsInsertedIntoToolData = 0;
            int rowsAdjustedInToolData = 0;

            using (LogTaskTimer.Start("WriteToolDataManager.WriteToolData", "ClientID = {0}, ResourceID = {1}, StartDate= '{2:yyyy-MM-dd}', EndDate = '{3:yyyy-MM-dd}', RowsDeletedFromToolData = {4}, RowsSelectedFromToolDataClean = {5}, RowsInsertedIntoToolData = {6}, rowsAdjustedInToolData = {7}", () => new object[] { ClientID, ResourceID, StartDate, EndDate, rowsDeletedFromToolData, rowsSelectedFromToolDataClean, rowsInsertedIntoToolData, rowsAdjustedInToolData }))
            {

                //get rid of any non-user entered entries
                rowsDeletedFromToolData = ToolDataDelete();

                //get all tool data for period
                var reader = new ReadToolDataManager();
                DataTable dtToolDataClean;

                if (StartDate < new DateTime(2011, 4, 1))
                    dtToolDataClean = reader.DailyToolData(StartDate, EndDate, ClientID, ResourceID);
                else
                    dtToolDataClean = reader.DailyToolData20110401(StartDate, EndDate, ClientID, ResourceID);

                rowsSelectedFromToolDataClean = dtToolDataClean.Rows.Count;

                using (var bcp = CreateToolDataBulkCopy())
                    bcp.WriteToServer(dtToolDataClean);

                //adjust ToolData to add the days and months data
                using (var dba = DA.Current.GetAdapter())
                    rowsAdjustedInToolData = dba.SelectCommand.ApplyParameters(new { Period = StartDate }).ExecuteNonQuery("ToolData_Adjust");
            }
        }

        private int ToolDataDelete()
        {
            DateTime period = StartDate;
            int? clientId = null;
            int? resourceId = null;

            if (ClientID > 0)
                clientId = ClientID;

            if (ResourceID > 0)
                resourceId = ResourceID;

            using (var dba = DA.Current.GetAdapter())
            {
                int result = dba.CommandTypeText()
                    .ApplyParameters(new { period, clientId, resourceId })
                    .ExecuteScalar<int>("DELETE ToolData WHERE Period = @period AND ClientID = ISNULL(@clientId, ClientID) AND ResourceID = ISNULL(@resourceId, ResourceID); SELECT @@ROWCOUNT;");
                return result;
            }
        }

        private IBulkCopy CreateToolDataBulkCopy()
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
