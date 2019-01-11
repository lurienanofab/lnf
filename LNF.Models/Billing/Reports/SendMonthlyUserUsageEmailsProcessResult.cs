using System;

namespace LNF.Models.Billing.Reports
{
    public class SendMonthlyUserUsageEmailsProcessResult : ProcessResult
    {
        public int QueryCount { get; set; }
        public int TotalEmailsSent { get; set; }
        public override string ProcessName => "SendMonthlyUserUsageEmails";

        protected override void WriteLog()
        {
            AppendLog($"QueryCount: {QueryCount}");
            AppendLog($"TotalEmailsSent: {TotalEmailsSent}");
        }
    }
}
