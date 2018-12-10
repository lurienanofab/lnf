namespace LNF.Models.Scheduler.Service
{
    public class FiveMinuteTaskResult
    {
        public EndRepairReservationsProcessResult EndRepairReservationsProcessResult { get; set; }
        public EndUnstartedReservationsProcessResult EndUnstartedReservationsProcessResult { get; set; }
        public HandleAutoEndReservationsProcessResult HandleAutoEndReservationsProcessResult { get; set; }
    }
}