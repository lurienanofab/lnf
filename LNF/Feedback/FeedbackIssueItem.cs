using System;

namespace LNF.Feedback
{
    public class FeedbackIssueItem
    {
        public int IssueID { get; set; }
        public DateTime Time { get; set; }
        public string ClientName { get; set; }
        public string ReporterName { get; set; }
        public string Comment { get; set; }
    }
}
