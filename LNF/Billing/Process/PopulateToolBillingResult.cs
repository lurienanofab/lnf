using System;

namespace LNF.Billing.Process
{
    public class PopulateToolBillingResult : DataProcessResult
    {
        public PopulateToolBillingResult(DateTime startedAt) : base(startedAt) { }

        public DateTime Period { get; set; }
        public int ClientID { get; set; }
        public bool IsTemp { get; set; }
        public override string ProcessName => "PopulateToolBilling";

        protected override void WriteLog()
        {
            AppendLog($"Period: {Period:yyyy-MM-dd HH:mm:ss}");
            AppendLog($"ClientID: {ClientID}");
            AppendLog($"IsTemp: {IsTemp}");
            base.WriteLog();
        }
    }
}