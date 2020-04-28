using System;
using System.Collections.Generic;

namespace LNF.Feedback
{
    public static class PositiveIssues
    {
        public static IEnumerable<IPositiveIssue> GetIssues(DateTime sd, DateTime ed, string status, int clientId)
        {
            return ServiceProvider.Current.Feedback.GetPositiveIssues(sd, ed, status, clientId);
        }
    }
}
