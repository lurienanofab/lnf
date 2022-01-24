using System;
using System.Collections.Generic;

namespace LNF.Scheduler
{
    public class CheckExpiringClientsProcessResult : ProcessResult
    {
        protected CheckExpiringClientsProcessResult() { }

        public CheckExpiringClientsProcessResult(DateTime startedAt, IEnumerable<string> data) : base(startedAt, data) { }

        public virtual int ExpiringClientsCount { get; set; }
        public virtual int ExpiringClientsEmailsSent { get; set; }
        public virtual int ExpiringEveryoneCount { get; set; }
        public virtual int ExpiringEveryoneEmailsSent { get; set; }
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
