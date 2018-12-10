namespace LNF.Models.Scheduler
{
    public class HandleAutoEndReservationsProcessResult : ProcessResult
    {
        public HandleAutoEndReservationsProcessResult() : base("HandleAutoEndReservations") { }

        public int ReservationsCount { get; set; }

        protected override void WriteLog()
        {
            AppendLog($"ReservationsCount: {ReservationsCount}");
        }
    }
}
