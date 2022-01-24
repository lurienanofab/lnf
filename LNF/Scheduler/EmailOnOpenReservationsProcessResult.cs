using System;

namespace LNF.Scheduler
{
    public class EmailOnOpenReservationsProcessResult : ProcessResult
    {
        protected EmailOnOpenReservationsProcessResult() { }

        public EmailOnOpenReservationsProcessResult(DateTime startedAt) : base(startedAt, null) { }

        public virtual int TotalEmailsSent { get; set; }
        public override string ProcessName => "EmailOnOpenReservations";

        protected override void WriteLog()
        {
            AppendLog($"TotalEmailsSent: {TotalEmailsSent}");
        }
    }
}
