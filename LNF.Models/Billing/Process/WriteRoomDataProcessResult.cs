namespace LNF.Models.Billing.Process
{
    public class WriteRoomDataProcessResult : ProcessResult
    {
        public WriteRoomDataProcessResult() : base("WriteRoomData") { }

        public int DistinctClientRows { get; set; }
        public int BadEntryRowsDeleted { get; set; }

        /// <summary>
        /// Note: This number will be larger than RowsLoadedInDestination because multiple updates are executed on the same rows.
        /// </summary>
        public int RowsAdjusted { get; set; }

        protected override void WriteLog()
        {
            base.WriteLog();
            AppendLog($"DistinctClientRows: {DistinctClientRows}");
            AppendLog($"BadEntryRowsDeleted: {BadEntryRowsDeleted}");
            AppendLog($"RowsAdjusted: {RowsAdjusted}");
        }
    }
}
