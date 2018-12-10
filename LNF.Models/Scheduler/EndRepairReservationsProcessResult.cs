namespace LNF.Models.Scheduler
{
    public class EndRepairReservationsProcessResult : ProcessResult
    {
        public EndRepairReservationsProcessResult() : base("EndRepairReservations") { }

        public int ReservationsCount { get; set; }

        protected override void WriteLog()
        {
            AppendLog($"ReservationsCount: {ReservationsCount}");
        }
    }
}