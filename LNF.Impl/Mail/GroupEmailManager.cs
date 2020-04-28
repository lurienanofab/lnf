using LNF.Data;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Data;
using LNF.Mail;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace LNF.Impl.Mail
{
    public class GroupEmailManager
    {
        protected ISession Session { get; }

        public GroupEmailManager(ISession session)
        {
            Session = session;
        }

        #region "Helper functions to populate email groups available"
        public IEnumerable<IPriv> GetAllPrivileges()
        {
            return Session.CreateSQLQuery("dbo.Priv_Select").List<Priv>();
        }

        public DataTable GetAllCommunities()
        {
            return Session.Command().FillDataTable("dbo.Community_Select");
        }

        public DataTable GetAllActiveManagers()
        {
            return Session.Command().Param("Action", "GetAllActive").FillDataTable("dbo.ClientManager_Select");
        }

        public DataTable GetAllActiveAreas()
        {
            return Session.Command(CommandType.Text).FillDataTable("SELECT AreaID, AreaName FROM v_Area");
        }

        public DataView GetAllActiveTools()
        {
            DataView dv = Session.Command().Param("Action", "SelectAll").FillDataTable("sselScheduler.dbo.procResourceSelect").DefaultView;
            dv.Sort = "ResourceName";
            return dv;
        }
        #endregion

        #region "Helper functions that get all email addresses based on group type"
        public IList<MassEmailRecipient> GetEmailListByPrivilege(int privs)
        {
            if (privs == 0)
                return new List<MassEmailRecipient>();

            var dt = Session.Command()
                .Param("Action", "GetEmailsByPrivilege")
                .Param("Privs", privs)
                .FillDataTable("dbo.ClientOrg_Select");

            return FilterEmails(dt);
        }

        public IList<MassEmailRecipient> GetEmailListByCommunity(int flag)
        {
            if (flag == 0)
                return new List<MassEmailRecipient>();

            var dt = Session.Command()
                .Param("Action", "GetEmailsByCommunity")
                .Param("Communities", flag)
                .FillDataTable("dbo.ClientOrg_Select");

            return FilterEmails(dt);
        }

        public IList<MassEmailRecipient> GetEmailListByManagerID(int managerId)
        {
            var dt = Session.Command()
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

        public IList<MassEmailRecipient> GetEmailListByTools(int[] multipleResourceIds)
        {
            var dt = Session.Command()
                .Param("Action", "SelectByMultipleResourceIDs")
                .Param("MultipleResourceIDs", string.Join(",", multipleResourceIds))
                .FillDataTable("sselScheduler.dbo.procResourceClientSelect");

            return FilterEmails(dt);
        }

        public IList<MassEmailRecipient> GetEmailListByInLab(int[] areas)
        {
            var dt = Session.Command()
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

        /// <summary>
        /// Get a particular person's email address based on his ClientID
        /// </summary>
        public DataRow GetEmailAddrByClientID(int clientId)
        {
            var dt = Session.Command()
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
        private IList<MassEmailRecipient> FilterEmails(DataTable dtEmails)
        {
            if (dtEmails == null) return null;

            // we have to filter out the invalid email addresses or addresses we should not send email to due to user's request
            var invalidEmailManager = new InvalidEmailManager(Session);
            DataTable dtInvalidEmails = invalidEmailManager.GetInvalidEmailListFiltering();
            DataTable dtClean = dtEmails.Clone();

            if (!dtClean.Columns.Contains("ClientID"))
                dtClean.Columns.Add("ClientID", typeof(int));

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
                                DataRow ndr = dtClean.NewRow();
                                ndr["ClientID"] = GetClientID(dr);
                                ndr["ClientOrgID"] = dr["ClientOrgID"];
                                ndr["Email"] = dr["Email"];
                                ndr["DisplayName"] = dr["DisplayName"];
                                ndr["IsStaff"] = dr["IsStaff"];
                                dtClean.Rows.Add(ndr);
                            }
                        }
                    }
                }
            }

            return RecipientsFromDataTable(dtClean);
        }

        private static int GetClientID(DataRow dr)
        {
            if (dr.Table.Columns.Contains("ClientID"))
            { 
                if (dr["ClientID"] != DBNull.Value)
                    return dr.Field<int>("ClientID");
            }

            return 0;
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
                ClientID = dr.Field<int>("ClientID"),
                Name = dr.Field<string>("DisplayName"),
                Email = dr.Field<string>("Email"),
                IsStaff = dr.Field<int>("IsStaff") == 1
            };
        }
    }
}