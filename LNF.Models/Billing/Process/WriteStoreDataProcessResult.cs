﻿using System;

namespace LNF.Models.Billing.Process
{
    public class WriteStoreDataProcessResult : DataProcessResult
    {
        public DateTime Period { get; set; }
        public int ClientID { get; set; }
        public int ItemID { get; set; }
        public override string ProcessName => "WriteStoreData";

        protected override void WriteLog()
        {
            AppendLog($"Period: {Period:yyyy-MM-dd HH:mm:ss}");
            AppendLog($"ClientID: {ClientID}");
            AppendLog($"ItemID: {ItemID}");
            base.WriteLog();
        }

    }
}
