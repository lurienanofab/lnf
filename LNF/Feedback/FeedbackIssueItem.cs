using System;

namespace LNF.Feedback
{
    public abstract class FeedbackIssueItem : IFeedbackIssue
    {
        public int IssueID { get; set; }
        public int ReporterID { get; set; }
        public int ClientID { get; set; }
        public DateTime Time { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
        public string AdminComment { get; set; }
    }
}
