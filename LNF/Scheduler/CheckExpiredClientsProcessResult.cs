namespace LNF.Scheduler
{
    public class CheckExpiredClientsProcessResult : ProcessResult
    {
        public int ExpiredClientsCount { get; set; }
        public int ExpiredClientsEmailsSent { get; set; }
        public int ExpiredEveryoneCount { get; set; }
        public int ExpiredEveryoneEmailsSent { get; set; }
        public int DeleteExpiredClientsCount { get; set; }
        public override string ProcessName => "CheckExpiredClients";

        protected override void WriteLog()
        {
            AppendLog($"ExpiredClientsCount: {ExpiredClientsCount}");
            AppendLog($"ExpiredClientsEmailsSent: {ExpiredClientsEmailsSent}");
            AppendLog($"ExpiredEveryoneCount: {ExpiredEveryoneCount}");
            AppendLog($"ExpiredEveryoneEmailsSent: {ExpiredEveryoneEmailsSent}");
            AppendLog($"DeleteExpiredClientsCount: {DeleteExpiredClientsCount}");
        }
    }
}
