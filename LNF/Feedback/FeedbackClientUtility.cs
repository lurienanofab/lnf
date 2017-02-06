using LNF.PhysicalAccess;
using LNF.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace LNF.Feedback
{
    public enum ClientGroup
    {
        InLab = 0,
        Internal = 1,
        External = 2,
        Staff = 3,
        All = 4
    }

    public static class FeedbackClientUtility
    {
        public static IList<FeedbackClientItem> SelectByGroup(ClientGroup group, DateTime sd, DateTime ed)
        {
            IEnumerable<FeedbackClientItem> query = null;

            if (group == ClientGroup.InLab)
            {
                IEnumerable<Badge> inlab = Providers.PhysicalAccess.CurrentlyInArea();
                query = inlab.Select(x => new FeedbackClientItem()
                {
                    ClientID = x.ClientID,
                    DisplayName = x.LastName + ", " + x.FirstName
                });
            }
            else
            {
                using (var dba = DA.Current.GetAdapter())
                {
                    DataTable dt = null;
                    switch (group)
                    {
                        case ClientGroup.All:
                            dt = dba.ApplyParameters(new { Action = "ByAll", sdate = sd, edate = ed }).FillDataTable("Feedback.dbo.FeedbackClient_Select");
                            break;
                        case ClientGroup.External:
                            dt = dba.ApplyParameters(new { Action = "ByExternal", sdate = sd, edate = ed }).FillDataTable("Feedback.dbo.FeedbackClient_Select");
                            break;
                        case ClientGroup.Internal:
                            dt = dba.ApplyParameters(new { Action = "ByInternal", sdate = sd, edate = ed }).FillDataTable("Feedback.dbo.FeedbackClient_Select");
                            break;
                        case ClientGroup.Staff:
                            dt = dba.ApplyParameters(new { Action = "ByStaff", sdate = sd, edate = ed }).FillDataTable("Feedback.dbo.FeedbackClient_Select");
                            break;
                    }

                    if (dt != null)
                    {
                        query = dt.AsEnumerable().Select(x => new FeedbackClientItem()
                        {
                            ClientID = x.Field<int>("ClientID"),
                            DisplayName = x.Field<string>("DisplayName")
                        });
                    }
                }
            }

            List<FeedbackClientItem> result;

            if (query == null)
                result = new List<FeedbackClientItem>();
            else
                result = query.OrderBy(x => x.DisplayName).ToList();


            result.Insert(0, new FeedbackClientItem() { ClientID = -99, DisplayName = "Unknown User" });

            return result;
        }
    }
}
