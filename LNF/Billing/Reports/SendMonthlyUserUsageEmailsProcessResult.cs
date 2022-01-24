using System;
using System.Collections.Generic;

namespace LNF.Billing.Reports
{
    public class SendMonthlyUserUsageEmailsProcessResult : ProcessResult
    {
        protected SendMonthlyUserUsageEmailsProcessResult() { }

        public SendMonthlyUserUsageEmailsProcessResult(DateTime startedAt, IEnumerable<string> data) : base(startedAt, data) { }

        public virtual int QueryCount { get; set; }
        public virtual int TotalEmailsSent { get; set; }
        public override string ProcessName => "SendMonthlyUserUsageEmails";

        protected override void WriteLog()
        {
            AppendLog($"QueryCount: {QueryCount}");
            AppendLog($"TotalEmailsSent: {TotalEmailsSent}");
        }
    }
}