using LNF.Repository;
using LNF.Repository.Feedback;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Feedback
{
    public static class NegativeIssueUtility
    {
        public static IList<FeedbackIssueItem> GetIssues(DateTime sd, DateTime ed, string status, int clientId)
        {
            var query = DA.Current.Query<NegativeIssue>().Where(x => x.Time >= sd && x.Time < ed.AddDays(1) && x.ClientID == clientId);

            if (status.ToLower() != "all")
                query = query.Where(x => x.Status == status);

            var result = query.Select(x => x.CreateFeedbackIssueItem()).ToList();

            return result;
        }

        public static IList<FeedbackIssueItem> GetIssuesByStatus(DateTime sd, DateTime ed, string status)
        {
            var query = DA.Current.Query<NegativeIssue>().Where(x => x.Time >= sd && x.Time < ed.AddDays(1) && x.Status == status).ToList();

            var result = query.Select(x => x.CreateFeedbackIssueItem()).ToList();

            return result;
        }
    }
}
