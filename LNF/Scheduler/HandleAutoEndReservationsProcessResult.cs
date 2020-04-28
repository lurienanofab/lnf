namespace LNF.Scheduler
{
    public class HandleAutoEndReservationsProcessResult : ProcessResult
    {
        public int ReservationsCount { get; set; }
        public override string ProcessName => "HandleAutoEndReservations";

        protected override void WriteLog()
        {
            AppendLog($"ReservationsCount: {ReservationsCount}");
        }
    }
}
