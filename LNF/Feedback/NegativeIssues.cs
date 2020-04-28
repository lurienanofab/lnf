using System;
using System.Collections.Generic;

namespace LNF.Feedback
{
    public static class NegativeIssues
    {
        public static IEnumerable<INegativeIssue> GetIssues(DateTime sd, DateTime ed, string status, int clientId = 0)
        {
            return ServiceProvider.Current.Feedback.GetNegativeIssues(sd, ed, status, clientId);
        }
    }
}
