using System;

namespace LNF.Models.Billing.Process
{
    public class WriteRoomDataProcessResult : DataProcessResult
    {
        public DateTime Period { get; set; }
        public int ClientID { get; set; }
        public int RoomID { get; set; }
        public int DistinctClientRows { get; set; }
        public int BadEntryRowsDeleted { get; set; }
        /// <summary>
        /// Note: This number will be larger than RowsLoadedInDestination because multiple updates are executed on the same rows.
        /// </summary>
        public int RowsAdjusted { get; set; }
        public override string ProcessName => "WriteRoomData";

        protected override void WriteLog()
        {
            AppendLog($"Period: {Period:yyyy-MM-dd HH:mm:ss}");
            AppendLog($"ClientID: {ClientID}");
            AppendLog($"RoomID: {RoomID}");
            base.WriteLog();
            AppendLog($"DistinctClientRows: {DistinctClientRows}");
            AppendLog($"BadEntryRowsDeleted: {BadEntryRowsDeleted}");
            AppendLog($"RowsAdjusted: {RowsAdjusted}");
        }
    }
}
