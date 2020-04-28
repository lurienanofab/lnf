using LNF.Impl.Repository.Feedback;

namespace LNF.Impl.Mappings.Feedback
{
    internal class PositiveIssueMap : FeedbackIssueMap<PositiveIssue>
    {
        internal PositiveIssueMap()
        {
            Table("PositiveIssue");
            Map(x => x.ClientID, "ClientID");
        }
    }
}
