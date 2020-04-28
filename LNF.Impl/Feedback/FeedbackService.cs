using LNF.Data;
using LNF.Feedback;
using LNF.Impl.DataAccess;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Feedback;
using LNF.PhysicalAccess;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Impl.Feedback
{
    public class FeedbackService : RepositoryBase, IFeedbackService
    {
        public IPhysicalAccessService PhysicalAccess { get; }

        public FeedbackService(ISessionManager mgr, IPhysicalAccessService physicalAccess) : base(mgr)
        {
            PhysicalAccess = physicalAccess;
        }

        public IEnumerable<INegativeIssue> GetNegativeIssues(DateTime sd, DateTime ed, string status, int clientId = 0)
        {
            var query = Session.Query<NegativeIssue>().Where(x => x.Time >= sd && x.Time < ed.AddDays(1));

            if (clientId > 0)
                query = query.Where(x => x.ClientID == clientId);

            if (status.ToLower() != "all")
                query = query.Where(x => x.Status == status);

            var result = query.CreateModels<INegativeIssue>();

            return result;
        }

        public IEnumerable<IPositiveIssue> GetPositiveIssues(DateTime sd, DateTime ed, string status, int clientId = 0)
        {
            var query = Session.Query<PositiveIssue>().Where(x => x.Time >= sd && x.Time < ed.AddDays(1));

            if (clientId > 0)
                query = query.Where(x => x.ClientID == clientId);

            if (status.ToLower() != "all")
                query = query.Where(x => x.Status == status);

            var result = query.CreateModels<IPositiveIssue>();

            return result;
        }

        public IEnumerable<ClientListItem> GetClientsByGroup(ClientGroup group, DateTime sd, DateTime ed)
        {
            IEnumerable<ClientListItem> query = null;

            if (group == ClientGroup.InLab)
            {
                IEnumerable<Badge> inlab = PhysicalAccess.GetCurrentlyInArea("all");
                query = inlab.Select(x => new ClientListItem
                {
                    ClientID = x.ClientID,
                    DisplayName = x.LastName + ", " + x.FirstName
                });
            }
            else
            {
                DataTable dt = null;
                switch (group)
                {
                    case ClientGroup.All:
                        dt = Session.Command().Param(new { Action = "ByAll", sdate = sd, edate = ed }).FillDataTable("Feedback.dbo.FeedbackClient_Select");
                        break;
                    case ClientGroup.External:
                        dt = Session.Command().Param(new { Action = "ByExternal", sdate = sd, edate = ed }).FillDataTable("Feedback.dbo.FeedbackClient_Select");
                        break;
                    case ClientGroup.Internal:
                        dt = Session.Command().Param(new { Action = "ByInternal", sdate = sd, edate = ed }).FillDataTable("Feedback.dbo.FeedbackClient_Select");
                        break;
                    case ClientGroup.Staff:
                        dt = Session.Command().Param(new { Action = "ByStaff", sdate = sd, edate = ed }).FillDataTable("Feedback.dbo.FeedbackClient_Select");
                        break;
                }

                if (dt != null)
                {
                    query = dt.AsEnumerable().Select(x => new ClientListItem
                    {
                        ClientID = x.Field<int>("ClientID"),
                        DisplayName = x.Field<string>("DisplayName")
                    });
                }
            }

            List<ClientListItem> result;

            if (query == null)
                result = new List<ClientListItem>();
            else
                result = query.OrderBy(x => x.DisplayName).ToList();


            result.Insert(0, new ClientListItem { ClientID = -99, DisplayName = "Unknown User" });

            return result;
        }
    }
}
