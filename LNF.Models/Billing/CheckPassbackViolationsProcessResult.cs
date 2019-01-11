using System;

namespace LNF.Models.Billing
{
    public class CheckPassbackViolationsProcessResult : ProcessResult
    {
        public int TotalPassbackViolations { get; set; }
        public override string ProcessName => "CheckPassbackViolations";

        protected override void WriteLog()
        {
            AppendLog($"TotalPassbackViolations: {TotalPassbackViolations}");
        }
    }
}
