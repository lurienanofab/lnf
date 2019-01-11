namespace LNF.Models.Scheduler.Service
{
    public class FiveMinuteTaskResult
    {
        public HandleRepairReservationsProcessResult EndRepairReservationsProcessResult { get; set; }
        public HandleUnstartedReservationsProcessResult EndUnstartedReservationsProcessResult { get; set; }
        public HandleAutoEndReservationsProcessResult HandleAutoEndReservationsProcessResult { get; set; }
    }
}