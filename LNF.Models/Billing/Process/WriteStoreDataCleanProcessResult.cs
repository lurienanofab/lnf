namespace LNF.Models.Billing.Process
{
    public class WriteStoreDataCleanProcessResult : ProcessResult
    {
        public WriteStoreDataCleanProcessResult() : base("WriteStoreDataClean") { }

        public int DryBoxRows { get; set; }

        protected override void WriteLog()
        {
            base.WriteLog();
            AppendLog($"DryBoxRows: {DryBoxRows}");
        }
    }
}
