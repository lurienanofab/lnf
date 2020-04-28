using System;

namespace LNF.Reporting.Individual
{
    public class UserUsageSummary
    {
        public int ClientID { get; set; }
        public string UserName { get; set; }
        public string LName { get; set; }
        public string FName { get; set; }
        public DateTime Period { get; set; }
        public DateTime Created { get; set; }
        public string Disclaimer { get; set; }
        public AggregateByOrg AggregateByOrg { get; set; }
    }
}