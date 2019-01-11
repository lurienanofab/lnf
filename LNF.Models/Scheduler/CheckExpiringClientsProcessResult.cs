using System;

namespace LNF.Models.Scheduler
{
    public class CheckExpiringClientsProcessResult : ProcessResult
    {
        public int ExpiringClientsCount { get; set; }
        public int ExpiringClientsEmailsSent { get; set; }
        public int ExpiringEveryoneCount { get; set; }
        public int ExpiringEveryoneEmailsSent { get; set; }
        public override string ProcessName => "CheckExpiringClients";

        protected override void WriteLog()
        {
            AppendLog($"ExpiringClientsCount: {ExpiringClientsCount}");
            AppendLog($"ExpiringClientsEmailsSent: {ExpiringClientsEmailsSent}");
            AppendLog($"ExpiringEveryoneCount: {ExpiringEveryoneCount}");
            AppendLog($"ExpiringEveryoneEmailsSent: {ExpiringEveryoneEmailsSent}");
        }
    }
}
