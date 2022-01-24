﻿using System;

namespace LNF.Billing.Process
{
    public class WriteRoomDataCleanResult : DataProcessResult
    {
        protected WriteRoomDataCleanResult() { }

        public WriteRoomDataCleanResult(DateTime startedAt) : base(startedAt) { }

        public virtual DateTime StartDate { get; set; }
        public virtual DateTime EndDate { get; set; }
        public virtual int ClientID { get; set; }
        public virtual int RoomID { get; set; }
        public override string ProcessName => "WriteRoomDataClean";

        protected override void WriteLog()
        {
            AppendLog($"StartDate: {StartDate:yyyy-MM-dd HH:mm:ss}");
            AppendLog($"EndDate: {EndDate:yyyy-MM-dd HH:mm:ss}");
            AppendLog($"ClientID: {ClientID}");
            base.WriteLog();
        }
    }
}