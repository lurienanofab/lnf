using System;
using System.Collections.Generic;

namespace LNF.Billing
{
    public class CheckPassbackViolationsProcessResult : ProcessResult
    {
        protected CheckPassbackViolationsProcessResult() { }

        public CheckPassbackViolationsProcessResult(DateTime startedAt, IEnumerable<string> data) : base(startedAt, data) { }

        public virtual int TotalPassbackViolations { get; set; }
        public override string ProcessName => "CheckPassbackViolations";

        protected override void WriteLog()
        {
            AppendLog($"TotalPassbackViolations: {TotalPassbackViolations}");
        }
    }
}