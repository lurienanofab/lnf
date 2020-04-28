using LNF.Data;
using LNF.Scheduler;
using System.Linq;

namespace LNF.Feedback
{
    public static class FeedbackIssueExtensions
    {
        public static string GetReporterName(this IFeedbackIssue issue)
        {
            if (issue.ReporterID > 0)
            {
                var client = ServiceProvider.Current.Data.Client.GetClient(issue.ClientID);
                if (client != null)
                    return Clients.GetDisplayName(client.LName, client.FName);
            }

            return null;
        }

        public static string GetReporterEmail(this IFeedbackIssue issue)
        {
            if (issue.ReporterID > 0)
            {
                var client = ServiceProvider.Current.Data.Client.GetClient(issue.ClientID);
                if (client != null)
                {
                    if (!client.PrimaryOrg)
                    {
                        var primary = ServiceProvider.Current.Data.Client.GetPrimary(client.ClientID);
                        if (primary != null)
                            return primary.Email;
                        else
                            return string.Empty;
                    }
                    else
                    {
                        return client.Email;
                    }
                }
            }

            return null;
        }

        public static string GetClientName(this IFeedbackIssue issue)
        {
            if (issue.ClientID > 0)
            {
                var client = ServiceProvider.Current.Data.Client.GetClient(issue.ClientID);
                if (client != null)
                    return client.DisplayName;
            }

            return null;
        }
    }

    public static class NegativeIssueExtensions
    {
        public static string GetResourceName(this INegativeIssue issue, ResourceTreeItemCollection tree)
        {
            var res = tree.Resources().Where(x => x.ResourceID == issue.ResourceID).FirstOrDefault();

            if (res != null)
                return res.ResourceName;
            else
                return "unspecified";
        }
    }
}
