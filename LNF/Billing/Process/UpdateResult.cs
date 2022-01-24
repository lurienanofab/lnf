using System;

namespace LNF.Billing.Process
{
    public class UpdateTablesResult : ProcessResult
    {
        protected UpdateTablesResult() { }

        public UpdateTablesResult(DateTime startedAt) : base(startedAt, null) { }

        public virtual DateTime Now { get; set; }
        public virtual DateTime Period { get; set; }
        public virtual bool IsFirstBusinessDay { get; set; }
        public virtual UpdateResult UpdateResult { get; set; }
        public virtual FinalizeResult FinalizeResult { get; set; }
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