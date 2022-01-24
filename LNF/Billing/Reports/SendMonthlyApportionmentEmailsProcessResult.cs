using System;
using System.Collections.Generic;

namespace LNF.Billing.Reports
{
    public class SendMonthlyApportionmentEmailsProcessResult : ProcessResult
    {
        protected SendMonthlyApportionmentEmailsProcessResult() { }

        public SendMonthlyApportionmentEmailsProcessResult(DateTime startedAt, IEnumerable<string> data) : base(startedAt, data) { }

        public virtual int ApportionmentClientCount { get; set; }
        public virtual int TotalEmailsSent { get; set; }
        public override string ProcessName => "SendMonthlyApportionmentEmails";

        protected override void WriteLog()
        {
            AppendLog($"ApportionmentClientCount: {ApportionmentClientCount}");
            AppendLog($"TotalEmailsSent: {TotalEmailsSent}");
        }
    }
}