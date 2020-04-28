namespace LNF.Scheduler
{
    public class HandleRepairReservationsProcessResult : ProcessResult
    {
        public int ReservationsCount { get; set; }
        public override string ProcessName => "HandleRepairReservations";

        protected override void WriteLog()
        {
            AppendLog($"ReservationsCount: {ReservationsCount}");
        }
    }
}