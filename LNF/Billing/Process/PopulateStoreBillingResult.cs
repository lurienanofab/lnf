﻿using System;

namespace LNF.Billing.Process
{
    public class PopulateStoreBillingResult : DataProcessResult
    {
        public PopulateStoreBillingResult(DateTime startedAt) : base(startedAt) { }

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