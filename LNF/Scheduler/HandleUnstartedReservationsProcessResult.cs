using System;
using System.Collections.Generic;

namespace LNF.Scheduler
{
    public class HandleUnstartedReservationsProcessResult : ProcessResult
    {
        protected HandleUnstartedReservationsProcessResult() { }

        public HandleUnstartedReservationsProcessResult(DateTime startedAt, IEnumerable<string> data) : base(startedAt, data) { }

        public virtual int ReservationsCount { get; set; }
        public override string ProcessName => "HandleUnstartedReservations";

        protected override void WriteLog()
        {
            AppendLog($"ReservationsCount: {ReservationsCount}");
        }
    }
}