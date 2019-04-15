using System;

namespace LNF.Models.Billing.Process
{
    public class WriteRoomDataCleanProcessResult : DataProcessResult
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int ClientID { get; set; }
        public int RoomID { get; set; }
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
