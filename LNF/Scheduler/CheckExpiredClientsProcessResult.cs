using System;
using System.Collections.Generic;

namespace LNF.Scheduler
{
    public class CheckExpiredClientsProcessResult : ProcessResult
    {
        protected CheckExpiredClientsProcessResult() { }

        public CheckExpiredClientsProcessResult(DateTime startedAt, IEnumerable<string> data) : base(startedAt, data) { }

        public virtual int ExpiredClientsCount { get; set; }
        public virtual int ExpiredClientsEmailsSent { get; set; }
        public virtual int ExpiredEveryoneCount { get; set; }
        public virtual int ExpiredEveryoneEmailsSent { get; set; }
        public virtual int DeleteExpiredClientsCount { get; set; }
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
