using System;

namespace LNF.Billing.Process
{
    public class WriteToolDataResult : DataProcessResult
    {
        public WriteToolDataResult(DateTime startedAt) : base(startedAt) { }

        public DateTime Period { get; set; }
        public int ClientID { get; set; }
        public int ResourceID { get; set; }
        public int RowsAdjusted { get; set; }
        public override string ProcessName => "WriteToolData";

        protected override void WriteLog()
        {
            AppendLog($"Period: {Period:yyyy-mm-dd HH:mm:ss}");
            AppendLog($"ClientID: {ClientID}");
            AppendLog($"ResourceID: {ResourceID}");
            base.WriteLog();
            AppendLog($"RowsAdjusted: {RowsAdjusted}");
        }
    }
}
