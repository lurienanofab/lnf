using System;

namespace LNF.Models.Billing.Process
{
    public class PopulateStoreBillingProcessResult : DataProcessResult
    {
        public DateTime Period { get; set; }
        public bool IsTemp { get; set; }
        public override string ProcessName => "PopulateStoreBilling";

        protected override void WriteLog()
        {
            AppendLog($"Period: {Period:yyyy-MM-dd HH:mm:ss}");
            AppendLog($"IsTemp: {IsTemp}");
            base.WriteLog();
        }
    }
}
