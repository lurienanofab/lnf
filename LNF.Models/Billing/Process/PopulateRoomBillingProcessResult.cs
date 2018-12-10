namespace LNF.Models.Billing.Process
{
    public class PopulateRoomBillingProcessResult : ProcessResult
    {
        public PopulateRoomBillingProcessResult() : base("PopulateRoomBilling") { }

        public bool UseParentRooms { get; set; }
        public int RowsExtractedForParentRooms { get; set; }
        public int RowsLoadedForParentRooms { get; set; }

        protected override void WriteLog()
        {
            base.WriteLog();
            AppendLog($"UseParentRooms: {UseParentRooms}");
            AppendLog($"RowsExtractedForParentRooms: {RowsExtractedForParentRooms}");
            AppendLog($"RowsLoadedForParentRooms: {RowsLoadedForParentRooms}");
        }
    }
}
