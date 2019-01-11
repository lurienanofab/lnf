namespace LNF.Models.Scheduler
{
    public class EmailOnOpenReservationsProcessResult : ProcessResult
    {
        public int TotalEmailsSent { get; set; }
        public override string ProcessName => "EmailOnOpenReservations";

        protected override void WriteLog()
        {
            AppendLog($"TotalEmailsSent: {TotalEmailsSent}");
        }
    }
}
