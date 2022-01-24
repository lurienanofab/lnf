using System;

namespace LNF.Billing.Process
{
    public class PopulateSubsidyBillingResult : DataProcessResult
    {
        public PopulateSubsidyBillingResult(DateTime startedAt) : base(startedAt) { }

        public DateTime Period { get; set; }
        public int ClientID { get; set; }
        public string Command { get; set; }
        public override string ProcessName => "PopulateSubsidyBilling";

        protected override void WriteLog()
        {
            AppendLog($"Period: {Period:yyyy-MM-dd HH:mm:ss}");
            AppendLog($"ClientID: {ClientID}");
            AppendLog($"Command: {Command}");
            base.WriteLog();
        }
    }
}