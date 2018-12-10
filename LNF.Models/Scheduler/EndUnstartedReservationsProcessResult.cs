namespace LNF.Models.Scheduler
{
    public class EndUnstartedReservationsProcessResult : ProcessResult
    {
        public EndUnstartedReservationsProcessResult() : base("EndUnstartedReservations") { }

        public int ReservationsCount { get; set; }

        protected override void WriteLog()
        {
            AppendLog($"ReservationsCount: {ReservationsCount}");
        }
    }
}