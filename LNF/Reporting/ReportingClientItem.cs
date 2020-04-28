namespace LNF.Reporting
{
    public class ReportingClientItem : IReportingClient
    {
        public int ClientID { get; set; }
        public string UserName { get; set; }
        public string LName { get; set; }
        public string FName { get; set; }
        public string Email { get; set; }
        public bool IsManager { get; set; }
        public bool IsFinManager { get; set; }
        public bool IsInternal { get; set; }
    }
}