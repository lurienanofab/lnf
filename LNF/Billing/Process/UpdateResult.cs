using System;

namespace LNF.Billing.Process
{
    public class UpdateTablesResult : ProcessResult
    {
        public DateTime Now { get; set; }
        public DateTime Period { get; set; }
        public bool IsFirstBusinessDay { get; set; }
        public UpdateResult UpdateResult { get; set; }
        public FinalizeResult FinalizeResult { get; set; }
        public override string ProcessName => "Update";

        protected override void WriteLog()
        {
            AppendLog($"Now: {Now:yyyy-MM-dd HH:mm:ss}");
            AppendLog($"Period: {Period:yyyy-MM-dd HH:mm:ss}");
            AppendLog($"IsFirstBusinessDay: {IsFirstBusinessDay}");
            AppendResult(UpdateResult);
            AppendResult(FinalizeResult);
        }
    }
}
