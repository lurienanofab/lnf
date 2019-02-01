using System;

namespace LNF.Models.Billing.Process
{
    public class WriteStoreDataCleanProcessResult : DataProcessResult
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ClientID { get; set; }
        public int DryBoxRows { get; set; }
        public override string ProcessName => "WriteStoreDataClean";

        protected override void WriteLog()
        {
            AppendLog($"StartDate: {StartDate:yyyy-MM-dd HH:mm:ss}");
            AppendLog($"EndDate: {EndDate:yyyy-MM-dd HH:mm:ss}");
            AppendLog($"ClientID: {ClientID}");
            base.WriteLog();
            AppendLog($"DryBoxRows: {DryBoxRows}");
        }
    }
}
