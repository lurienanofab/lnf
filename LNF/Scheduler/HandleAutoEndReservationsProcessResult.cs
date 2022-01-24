using System;
using System.Collections.Generic;

namespace LNF.Scheduler
{
    public class HandleAutoEndReservationsProcessResult : ProcessResult
    {
        protected HandleAutoEndReservationsProcessResult() { }

        public HandleAutoEndReservationsProcessResult(DateTime startedAt, IEnumerable<string> data) : base(startedAt, data) { }

        public virtual int ReservationsCount { get; set; }
        public override string ProcessName => "HandleAutoEndReservations";

        protected override void WriteLog()
        {
            AppendLog($"ReservationsCount: {ReservationsCount}");
        }
    }
}
