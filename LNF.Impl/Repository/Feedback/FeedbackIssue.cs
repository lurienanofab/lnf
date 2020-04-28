using LNF.DataAccess;
using System;

namespace LNF.Impl.Repository.Feedback
{
    /// <summary>
    /// A base class for both positive and negative feedback issues
    /// </summary>
    public abstract class FeedbackIssue : IDataItem
    {
        /// <summary>
        /// The unique id of a feedback issue
        /// </summary>
        public virtual int IssueID { get; set; }

        /// <summary>
        /// The unique id of the issuer reporter
        /// </summary>
        public virtual int ReporterID { get; set; }

        /// <summary>
        /// The unique id of the issue reportee
        /// </summary>
        public virtual int ClientID { get; set; }

        /// <summary>
        /// The time the issue occurred
        /// </summary>
        public virtual DateTime Time { get; set; }

        /// <summary>
        /// The time the issue was created
        /// </summary>
        public virtual DateTime CreatedTime { get; set; }

        /// <summary>
        /// The issue status
        /// </summary>
        public virtual string Status { get; set; }

        /// <summary>
        /// The reporter comment
        /// </summary>
        public virtual string Comment { get; set; }

        /// <summary>
        /// Any comment added by an administrator
        /// </summary>
        public virtual string AdminComment { get; set; }
    }
}
