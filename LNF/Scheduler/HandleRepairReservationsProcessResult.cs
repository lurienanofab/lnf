using System;
using System.Collections.Generic;

namespace LNF.Scheduler
{
    public class HandleRepairReservationsProcessResult : ProcessResult
    {
        protected HandleRepairReservationsProcessResult() { }

        public HandleRepairReservationsProcessResult(DateTime startedAt, IEnumerable<string> data) : base(startedAt, data) { }

        public virtual int ReservationsCount { get; set; }
        public override string ProcessName => "HandleRepairReservations";

        protected override void WriteLog()
        {
            AppendLog($"ReservationsCount: {ReservationsCount}");
        }
    }
}