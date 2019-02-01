using System;

namespace LNF.Models.Billing.Process
{
    public class DataUpdateProcessResult : ProcessResult
    {
        public DateTime Now { get; set; }
        public DateTime Period { get; set; }
        public bool IsFirstBusinessDay { get; set; }
        public UpdateTablesResult UpdateTablesResult { get; set; }
        public DataFinalizeProcessResult FinalizeResult { get; set; }
        public override string ProcessName => "Update";

        protected override void WriteLog()
        {
            AppendLog($"Now: {Now:yyyy-MM-dd HH:mm:ss}");
            AppendLog($"Period: {Period:yyyy-MM-dd HH:mm:ss}");
            AppendLog($"IsFirstBusinessDay: {IsFirstBusinessDay}");
            AppendResult(UpdateTablesResult);
            AppendResult(FinalizeResult);
        }
    }
}
