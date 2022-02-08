using System;
using System.Collections.Generic;

namespace LNF.Billing.Reports
{
    public class SendMonthlyCardExpirationEmailsProcessResult : ProcessResult
    {
        protected SendMonthlyCardExpirationEmailsProcessResult() : base() { }

        public SendMonthlyCardExpirationEmailsProcessResult(DateTime startedAt, IEnumerable<string> data) : base(startedAt, data) { }

        public virtual int CardExpirationClientCount { get; set; }
        public virtual int TotalEmailsSent { get; set; }
        public override string ProcessName => "SendMonthlyCardExpirationEmails";

        protected override void WriteLog()
        {
            AppendLog($"CardExpirationClientCount: {CardExpirationClientCount}");
            AppendLog($"TotalEmailsSent: {TotalEmailsSent}");
        }
    }
}
