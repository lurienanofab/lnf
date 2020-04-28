using LNF.Impl.Repository.Feedback;

namespace LNF.Impl.Mappings.Feedback
{
    internal class NegativeIssueMap : FeedbackIssueMap<NegativeIssue>
    {
        internal NegativeIssueMap()
        {
            Table("NegativeIssue");
            Map(x => x.ClientID, "ViolatorID");
            Map(x => x.ResourceID);
            Map(x => x.Rule1);
            Map(x => x.Rule2);
            Map(x => x.Rule3);
            Map(x => x.Rule4);
            Map(x => x.Rule5);
            Map(x => x.Rule6);
            Map(x => x.Rule7);
            Map(x => x.Rule8);
            Map(x => x.Rule9);
            Map(x => x.Rule10);
            Map(x => x.Rule11);
            Map(x => x.Rule12);
            Map(x => x.Rule13);
        }
    }
}
