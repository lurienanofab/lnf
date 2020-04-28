namespace LNF.Scheduler
{
    public class HandleUnstartedReservationsProcessResult : ProcessResult
    {
        public int ReservationsCount { get; set; }
        public override string ProcessName => "HandleUnstartedReservations";

        protected override void WriteLog()
        {
            AppendLog($"ReservationsCount: {ReservationsCount}");
        }
    }
}