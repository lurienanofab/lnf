﻿using System;

namespace LNF.Models.Billing.Process
{
    public class PopulateRoomBillingProcessResult : ProcessResult
    {
        public DateTime Period { get; set; }
        public int ClientID { get; set; }
        public bool IsTemp { get; set; }
        public bool UseParentRooms { get; set; }
        public int RowsExtractedForParentRooms { get; set; }
        public int RowsLoadedForParentRooms { get; set; }
        public override string ProcessName => "PopulateRoomBilling";

        protected override void WriteLog()
        {
            AppendLog($"Period: {Period:yyyy-MM-dd HH:mm:ss}");
            AppendLog($"ClientID: {ClientID}");
            AppendLog($"IsTemp: {IsTemp}");
            AppendLog($"UseParentRooms: {UseParentRooms}");
            base.WriteLog();
            AppendLog($"RowsExtractedForParentRooms: {RowsExtractedForParentRooms}");
            AppendLog($"RowsLoadedForParentRooms: {RowsLoadedForParentRooms}");
        }
    }
}
