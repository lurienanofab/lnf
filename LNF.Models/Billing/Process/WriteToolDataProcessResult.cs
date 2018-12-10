namespace LNF.Models.Billing.Process
{
    public class WriteToolDataProcessResult : ProcessResult
    {
        public WriteToolDataProcessResult() : base("WriteToolData") { }

        public int RowsAdjusted { get; set; }

        protected override void WriteLog()
        {
            base.WriteLog();
            AppendLog($"RowsAdjusted: {RowsAdjusted}");
        }
    }
}
