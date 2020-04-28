using FluentNHibernate.Mapping;
using LNF.Impl.Repository.Feedback;

namespace LNF.Impl.Mappings.Feedback
{
    internal abstract class FeedbackIssueMap<T> : ClassMap<T> where T : FeedbackIssue
    {
        internal FeedbackIssueMap()
        {
            Schema("Feedback.dbo");
            Id(x => x.IssueID);
            Map(x => x.ReporterID);
            Map(x => x.Time);
            Map(x => x.CreatedTime);
            Map(x => x.Status);
            Map(x => x.Comment);
            Map(x => x.AdminComment);
        }
    }
}
