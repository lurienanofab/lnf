using LNF.Data;
using LNF.Feedback;
using RestSharp;
using System;
using System.Collections.Generic;

namespace OnlineServices.Api.Feedback
{
    public class FeedbackService : ApiClient, IFeedbackService
    {
        internal FeedbackService(IRestClient rc) : base(rc) { }

        public IEnumerable<ClientListItem> GetClientsByGroup(ClientGroup group, DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<INegativeIssue> GetNegativeIssues(DateTime sd, DateTime ed, string status, int clientId = 0)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IPositiveIssue> GetPositiveIssues(DateTime sd, DateTime ed, string status, int clientId = 0)
        {
            throw new NotImplementedException();
        }
    }
}
