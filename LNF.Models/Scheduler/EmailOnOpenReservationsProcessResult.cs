namespace LNF.Models.Scheduler
{
    public class EmailOnOpenReservationsProcessResult : ProcessResult
    {
        public EmailOnOpenReservationsProcessResult() : base("EmailOnOpenReservations") { }

        public int TotalEmailsSent { get; set; }

        protected override void WriteLog()
        {
            AppendLog($"TotalEmailsSent: {TotalEmailsSent}");
        }
    }
}
