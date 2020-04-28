using LNF.Data;
using System;
using System.Collections.Generic;

namespace LNF.Feedback
{
    public interface IFeedbackService
    {
        IEnumerable<INegativeIssue> GetNegativeIssues(DateTime sd, DateTime ed, string status, int clientId = 0);
        IEnumerable<IPositiveIssue> GetPositiveIssues(DateTime sd, DateTime ed, string status, int clientId = 0);
        IEnumerable<ClientListItem> GetClientsByGroup(ClientGroup group, DateTime sd, DateTime ed);
    }
}
