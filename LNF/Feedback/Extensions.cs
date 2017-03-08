﻿using LNF.Cache;
using LNF.Data;
using LNF.Repository.Feedback;
using LNF.Scheduler;
using System.Linq;

namespace LNF.Feedback
{
    public static class FeedbackIssueExtensions
    {
        public static FeedbackIssueItem CreateFeedbackIssueItem(this FeedbackIssue issue)
        {
            return new FeedbackIssueItem()
            {
                IssueID = issue.IssueID,
                Time = issue.Time,
                ClientName = issue.GetClientName(),
                ReporterName = issue.GetReporterName(),
                Comment = issue.Comment
            };
        }

        public static string GetReporterName(this FeedbackIssue issue)
        {
            if (issue.ReporterID > 0)
            {
                var client = CacheManager.Current.GetClient(issue.ClientID);
                if (client != null)
                    return client.DisplayName;
            }

            return null;
        }

        public static string GetReporterEmail(this FeedbackIssue issue)
        {
            if (issue.ReporterID > 0)
            {
                var client = CacheManager.Current.GetClient(issue.ClientID);
                if (client != null)
                {
                    var primary = ClientOrgUtility.GetPrimary(client.ClientID);
                    if (primary != null)
                        return primary.Email;
                    else
                        return string.Empty;
                }
            }

            return null;
        }

        public static string GetClientName(this FeedbackIssue issue)
        {
            if (issue.ClientID > 0)
            {
                var client = CacheManager.Current.GetClient(issue.ClientID);
                if (client != null)
                    return client.DisplayName;
            }

            return null;
        }
    }

    public static class NegativeIssueExtensions
    {
        public static string GetResourceName(this NegativeIssue issue)
        {
            var res = CacheManager.Current.Resources(x => x.ResourceID == issue.ResourceID).FirstOrDefault();

            if (res != null)
                return res.ResourceName;
            else
                return "unspecified";
        }
    }
}