﻿using System;

namespace LNF.Billing.Process
{
    public class WriteToolDataCleanResult : DataProcessResult
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ClientID { get; set; }
        public override string ProcessName => "WriteToolDataClean";

        protected override void WriteLog()
        {
            AppendLog($"StartDate: {StartDate:yyyy-MM-dd HH:mm:ss}");
            AppendLog($"EndDate: {EndDate:yyyy-MM-dd HH:mm:ss}");
            AppendLog($"ClientID: {ClientID}");
            base.WriteLog();
        }
    }
}
