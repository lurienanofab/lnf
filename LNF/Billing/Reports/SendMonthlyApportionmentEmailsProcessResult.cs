namespace LNF.Billing.Reports
{
    public class SendMonthlyApportionmentEmailsProcessResult : ProcessResult
    {
        public int ApportionmentClientCount { get; set; }
        public int TotalEmailsSent { get; set; }
        public override string ProcessName => "SendMonthlyApportionmentEmails";

        protected override void WriteLog()
        {
            AppendLog($"ApportionmentClientCount: {ApportionmentClientCount}");
            AppendLog($"TotalEmailsSent: {TotalEmailsSent}");
        }
    }
}
