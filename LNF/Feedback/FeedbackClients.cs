using LNF.Data;
using System;
using System.Collections.Generic;

namespace LNF.Feedback
{
    public static class FeedbackClients
    {
        public static IEnumerable<ClientListItem> SelectByGroup(ClientGroup group, DateTime sd, DateTime ed)
        {
            return ServiceProvider.Current.Feedback.GetClientsByGroup(group, sd, ed);
        }
    }
}
