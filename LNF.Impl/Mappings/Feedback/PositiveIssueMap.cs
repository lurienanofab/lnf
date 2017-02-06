using LNF.Repository.Feedback;

namespace LNF.Impl.Mappings.Feedback
{
    internal class PositiveIssueMap : FeedbackIssueClassMap<PositiveIssue>
    {
        internal PositiveIssueMap()
        {
            Table("PositiveIssue");
            Map(x => x.ClientID, "ClientID");
        }
    }
}
