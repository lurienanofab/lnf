namespace LNF.Models.Scheduler
{
    public class CheckExpiringClientsProcessResult : ProcessResult
    {
        public CheckExpiringClientsProcessResult() : base("CheckExpiringClients") { }

        public int ExpiringClientsCount { get; set; }
        public int ExpiringClientsEmailsSent { get; set; }
        public int ExpiringEveryoneCount { get; set; }
        public int ExpiringEveryoneEmailsSent { get; set; }

        protected override void WriteLog()
        {
            AppendLog($"ExpiringClientsCount: {ExpiringClientsCount}");
            AppendLog($"ExpiringClientsEmailsSent: {ExpiringClientsEmailsSent}");
            AppendLog($"ExpiringEveryoneCount: {ExpiringEveryoneCount}");
            AppendLog($"ExpiringEveryoneEmailsSent: {ExpiringEveryoneEmailsSent}");
        }
    }
}
