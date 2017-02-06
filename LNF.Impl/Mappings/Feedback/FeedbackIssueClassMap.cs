using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LNF.Repository.Feedback;
using FluentNHibernate.Mapping;

namespace LNF.Impl.Mappings.Feedback
{
    internal abstract class FeedbackIssueClassMap<T> : ClassMap<T> where T : FeedbackIssue
    {
        internal FeedbackIssueClassMap()
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
