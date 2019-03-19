using LNF.Cache;
using LNF.Data;
using LNF.Mail.Criteria;
using LNF.Models.Data;
using LNF.Models.Mail;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Mail;
using LNF.Scheduler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace LNF.Mail
{
    public class GroupEmailManager
    {
        #region "Helper functions to populate email groups available"
        public static DataTable GetAllPrivileges()
        {
            return DA.Command().FillDataTable("dbo.Priv_Select");
        }

        public static DataTable GetAllCommunities()
        {
            return DA.Command().FillDataTable("dbo.Community_Select");
        }

        public static DataTable GetAllActiveManagers()
        {
            return DA.Command().Param("Action", "GetAllActive").FillDataTable("dbo.ClientManager_Select");
        }

        public static DataTable GetAllActiveAreas()
        {
            return DA.Command(CommandType.Text).FillDataTable("SELECT AreaID, AreaName FROM v_Area");
        }

        public static DataView GetAllActiveTools()
        {
            DataView dv = DA.Command().Param("Action", "SelectAll").FillDataTable("sselScheduler.dbo.procResourceSelect").DefaultView;
            dv.Sort = "ResourceName";
            return dv;
        }
        #endregion

        #region "Helper functions that get all email addresses based on group type"
        public static IList<MassEmailRecipient> GetEmailListByPrivilege(int privs)
        {
            if (privs == 0)
                return new List<MassEmailRecipient>();

            var dt = DA.Command()
                .Param("Action", "GetEmailsByPrivilege")
                .Param("Privs", privs)
                .FillDataTable("dbo.ClientOrg_Select");

            return FilterEmails(dt);
        }

        public static IList<MassEmailRecipient> GetEmailListByCommunity(int flag)
        {
            if (flag == 0)
                return new List<MassEmailRecipient>();

            var dt = DA.Command()
                .Param("Action", "GetEmailsByCommunity")
                .Param("Communities", flag)
                .FillDataTable("dbo.ClientOrg_Select");

            return FilterEmails(dt);
        }

        public static IList<MassEmailRecipient> GetEmailListByManagerID(int managerId)
        {
            var dt = DA.Command()
                .Param("Action", "GetEmailsByManager")
                .Param("ClientID", managerId)
                .FillDataTable("dbo.ClientOrg_Select");

            if (dt.Select($"ClientID = {managerId}").Length == 0)
            {
                DataRow emailRow = GetEmailAddrByClientID(managerId);

                if (emailRow != null)
                {
                    var ndr = dt.NewRow();
                    ndr["ClientOrgID"] = emailRow["ClientOrgID"];
                    ndr["DisplayName"] = emailRow["DisplayName"];
                    ndr["Email"] = emailRow["Email"];
                    ndr["IsStaff"] = 0; //isStaff column is needed because we have to check it in the last stage right before sending out email
                    dt.Rows.Add(ndr);
                }
            }

            return FilterEmails(dt);
        }

        public static IList<MassEmailRecipient> GetEmailListByTools(int[] multipleResourceIds)
        {
            var dt = DA.Command()
                .Param("Action", "SelectByMultipleResourceIDs")
                .Param("MultipleResourceIDs", string.Join(",", multipleResourceIds))
                .FillDataTable("sselScheduler.dbo.procResourceClientSelect");

            return FilterEmails(dt);
        }

        public static IList<MassEmailRecipient> GetEmailListByInLab(int[] areas)
        {
            var dt = DA.Command()
                .Param("Action", "GetEmailsByInLab")
                .Param("Areas", GetAreasXml(areas))
                .FillDataTable("dbo.ClientOrg_Select");

            return FilterEmails(dt);
        }

        private static string GetAreasXml(int[] areas)
        {
            string root = "<areas></areas>";

            if (areas == null || areas.Length == 0)
                return root;

            XElement xel = XElement.Parse(root);

            foreach (int a in areas)
                xel.Add(XElement.Parse($"<id>{a}</id>"));

            return xel.ToString();
        }
        #endregion

        private ResourceTreeItemCollection ResourceTree { get; }

        public GroupEmailManager(ResourceTreeItemCollection resourceTree)
        {
            ResourceTree = resourceTree;
        }


        /// <summary>
        /// Get a particular person's email address based on his ClientID
        /// </summary>
        public static DataRow GetEmailAddrByClientID(int clientId)
        {
            var dt = DA.Command()
                .Param("Action", "GetEmails")
                .Param("ClientID", clientId)
                .FillDataTable("dbo.Client_Select");

            if (dt.Rows.Count > 0)
                return dt.Rows[0];

            return null;
        }

        /// <summary>
        /// All email addresses filtering shoud be done here.  One thing need more attention is that if the number of emails in filters is too big, performance might be an issue
        /// </summary>
        private static IList<MassEmailRecipient> FilterEmails(DataTable dtEmails)
        {
            if (dtEmails == null) return null;

            // we have to filter out the invalid email addresses or addresses we should not send email to due to user's request
            DataTable dtInvalidEmails = InvalidEmailManager.GetInvalidEmailListFiltering();
            DataTable dtClean = dtEmails.Clone();

            foreach (DataRow dr in dtEmails.Rows)
            {
                DataRow[] badEmail = dtInvalidEmails.Select("EmailAddress = '" + dr["Email"] + "'");
                if (badEmail.Length == 0)
                {
                    // get rid of repeated emails
                    DataRow[] repeatmail = dtClean.Select("Email = '" + dr["Email"] + "'");
                    if (repeatmail.Length == 0)
                    {
                        // Filter out the empty email addresses - it's possible, because some users are inactive but still shown on Scheduler
                        if (dr["Email"] != DBNull.Value)
                        {
                            // filter out all those nonsense emails that has the format of none@none.xxx, or nobody@nowhere.xxx
                            string bademail1 = dr["Email"].ToString().Substring(0, 4);
                            string bademail2 = dr["Email"].ToString().Substring(0, 6);
                            if (bademail1 != "none" && bademail2 != "nobody")
                            {
                                DataRow newrow = dtClean.NewRow();
                                newrow["ClientOrgID"] = dr["ClientOrgID"];
                                newrow["Email"] = dr["Email"];
                                newrow["DisplayName"] = dr["DisplayName"];
                                newrow["IsStaff"] = dr["IsStaff"];
                                dtClean.Rows.Add(newrow);
                            }
                        }
                    }
                }
            }

            return RecipientsFromDataTable(dtClean);
        }

        public string SendEmail(MassEmail email, int clientId)
        {
            try
            {
                IList<string> to = null;
                IList<string> cc = null;
                IList<string> bcc = null;

                SendMessageArgs args = new SendMessageArgs()
                {
                    ClientID = clientId,
                    From = email.FromAddress,
                    DisplayName = email.DisplayName,
                    Subject = email.Subject.Trim(),
                    IsHtml = false
                };

                string returnMessage = string.Empty;

                int staffCount = 0;
                int totalCount = 0;

                IEnumerable<MassEmailRecipient> recipients = email.GetCriteria().GetRecipients();

                // loop through the table and create the BCC recipients
                foreach (MassEmailRecipient recip in recipients)
                {
                    try
                    {
                        if (recip.IsStaff)
                        {
                            // we have to expose staff member's emails, so adding to To (not Bcc)
                            NullSafeAddToList(ref to, recip.Email);
                            staffCount++;
                        }
                        else
                        {
                            NullSafeAddToList(ref bcc, recip.Email);
                        }

                        totalCount++;
                    }
                    catch (Exception ex)
                    {
                        returnMessage += $"<div>The email address {recip.Email} doesn't have correct format, please contact the administrator: {ex.Message}</div>";
                    }
                }

                string[] ccAddr = email.GetCC();

                if (ccAddr != null && ccAddr.Length > 0)
                {
                    try
                    {
                        foreach (string addr in ccAddr)
                            NullSafeAddToList(ref cc, addr);
                    }
                    catch (Exception ex)
                    {
                        return $"The CC email addresses string has wrong format, please make sure addresses are separated by comma: {ex.Message}";
                    }
                }

                bool isStaff = email.Client.HasPriv(ClientPrivilege.Staff);
                bool recipientsIncludeNonStaff = staffCount != totalCount;

                if (isStaff && recipientsIncludeNonStaff)
                    // messages only sent here if staff is sending email to non-staff
                    NullSafeAddToList(ref bcc, "announcements@lnf.umich.edu");

                // all messages are sent here
                NullSafeAddToList(ref bcc, "messages@lnf.umich.edu");

                // apparently we always send to the from address also
                NullSafeAddToList(ref to, email.FromAddress);

                string footer = "\r\n\r\n\r\nThis email has been sent to the following group(s) : " + GetGroup(email) + ".";
                footer += "\r\nYou are receiving this email message because you are associated with the LNF.\r\nTo unsubscribe, please go to:\r\nhttp://ssel-sched.eecs.umich.edu/sselOnLine/Unsubscribe.aspx";

                args.Body = email.Body + footer;

                // handle the email attachments
                // [2016-06-09 jg] now handled by inserting links into the body (outside this method)

                args.To = to;
                args.Cc = cc;
                args.Bcc = bcc;

                ServiceProvider.Current.Mail.SendMessage(args);

                return returnMessage;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private string GetGroup(MassEmail email)
        {
            string result = string.Empty;

            switch (email.RecipientGroup)
            {
                case "community":
                    result = string.Join(", ", CommunityUtility.GetCommunityNames(email.GetCriteria<ByCommunity>().SelectedCommunities));
                    break;
                case "manager":
                    result = DA.Current.Single<Client>(email.GetCriteria<ByManager>().SelectedManagerClientID).DisplayName;
                    break;
                case "tools":
                    result = string.Join(", ", ResourceTree.Resources().Where(x => email.GetCriteria<ByTool>().SelectedResourceIDs.Contains(x.ResourceID)).ToList().Select(x => x.ResourceName));
                    break;
                case "lab":
                    result = string.Join(", ", DA.Current.Query<PhysicalAccessArea>().Where(x => email.GetCriteria<ByLab>().SelectedLabs.Contains(x.AreaID)).ToList().Select(x => x.AreaName));
                    break;
                default: //privilege
                    result = string.Join(", ", PrivUtility.GetPrivTypes((ClientPrivilege)email.GetCriteria<ByPrivilege>().SelectedPrivileges));
                    break;
            }

            return result;
        }

        private static void NullSafeAddToList<T>(ref IList<T> list, T item)
        {
            if (list == null) list = new List<T>();
            if (!list.Contains(item))
                list.Add(item);
        }

        private static IList<MassEmailRecipient> RecipientsFromDataTable(DataTable dt)
        {
            var result = dt.AsEnumerable().Select(RecipientFromDataRow).ToList();
            return result;
        }

        private static MassEmailRecipient RecipientFromDataRow(DataRow dr)
        {
            return new MassEmailRecipient()
            {
                Name = dr.Field<string>("DisplayName"),
                Email = dr.Field<string>("Email"),
                IsStaff = dr.Field<int>("IsStaff") == 1
            };
        }
    }
}