namespace LNF.Models.Billing.Reports
{
    public class SendMonthlyUserUsageEmailsProcessResult : ProcessResult
    {
        public SendMonthlyUserUsageEmailsProcessResult() : base("SendMonthlyUserUsageEmails") { }

        public int QueryCount { get; set; }
        public int TotalEmailsSent { get; set; }

        protected override void WriteLog()
        {
            AppendLog($"QueryCount: {QueryCount}");
            AppendLog($"TotalEmailsSent: {TotalEmailsSent}");
        }
    }
}
