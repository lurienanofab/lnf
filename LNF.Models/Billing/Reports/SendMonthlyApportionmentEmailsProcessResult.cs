namespace LNF.Models.Billing.Reports
{
    public class SendMonthlyApportionmentEmailsProcessResult : ProcessResult
    {
        public SendMonthlyApportionmentEmailsProcessResult() : base("SendMonthlyApportionmentEmails") { }

        public int ApportionmentClientCount { get; set; }
        public int TotalEmailsSent { get; set; }

        protected override void WriteLog()
        {
            AppendLog($"ApportionmentClientCount: {ApportionmentClientCount}");
            AppendLog($"TotalEmailsSent: {TotalEmailsSent}");
        }
    }
}
