using System;

namespace LNF.Feedback
{
    public interface IFeedbackIssue
    {
        int IssueID { get; set; }
        int ReporterID { get; set; }
        int ClientID { get; set; }
        DateTime Time { get; set; }
        DateTime CreatedTime { get; set; }
        string Status { get; set; }
        string Comment { get; set; }
        string AdminComment { get; set; }
    }
}
