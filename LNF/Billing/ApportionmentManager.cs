using LNF.Cache;
using LNF.CommonTools;
using LNF.Data;
using LNF.Logging;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LNF.Billing
{
    public class ApportionmentManager : ManagerBase, IApportionmentManager
    {
        protected IClientManager ClientManager { get; }

        public ApportionmentManager(ISession session, IClientManager clientManager) : base(session)
        {
            ClientManager = clientManager;
        }

        public void CheckClientIssues()
        {
            DateTime period = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            SendMonthlyApportionmentEmails(period);
        }

        public IList<ApportionmentClient> SelectApportionmentClients(DateTime startDate, DateTime endDate)
        {
            return Session.NamedQuery("SelectApportionmentClients")
                .SetParameters(new { StartDate = startDate, EndDate = endDate })
                .List<ApportionmentClient>();
        }

        public int PopulateRoomApportionData(DateTime period)
        {
            return Session.NamedQuery("PopulateRoomApportionData")
                .SetParameters(new { Period = period })
                .Result<int>();
        }

        /// <summary>
        /// Sends clients alerts at the beginning of the month including 1) anti-passback errors in the data, and 2) need to apportion.
        /// </summary>
        public int SendMonthlyApportionmentEmails(DateTime period, string message = null, string[] recipients = null, bool noEmail = false)
        {
            int apportionmentClientCount = 0;
            int result = 0;

            using (var timer = LogTaskTimer.Start("ApportionmentUtility.SendMonthlyApportionmentEmails", "period = '{0:yyyy-MM-dd}', message = '{1}', noEmail = {2}, apportionmentClientCount = {3}", () => new object[] { period, message, noEmail, apportionmentClientCount }))
            {
                //With noEmail set to true, nothing happens here. The appropriate users are selected and logged
                //but no email is actually sent. This is for testing/debugging purposes.

                StringBuilder bodyHtml;

                IList<ApportionmentClient> query = SelectApportionmentClients(period, period.AddMonths(1));

                apportionmentClientCount = query.Count;

                foreach (ApportionmentClient ac in query)
                {
                    string Subject = string.Format("Please apportion your {0} lab usage time", ServiceProvider.Current.Email.CompanyName);
                    bodyHtml = new StringBuilder();
                    bodyHtml.AppendLine(ac.DisplayName + ":<br /><br />");
                    if (!string.IsNullOrEmpty(message))
                        bodyHtml.AppendLine("<p>" + message + "</p>");
                    bodyHtml.AppendLine("As can best be determined, you need to apportion your " + ServiceProvider.Current.Email.CompanyName + " lab time. This is necessary because you had access to multiple accounts and have entered one or more " + ServiceProvider.Current.Email.CompanyName + " rooms this billing period.<br /><br />");
                    bodyHtml.AppendLine("This matter must be resolved by the close of the third business day of this month.");
                    bodyHtml.AppendLine("For more information about how to apportion your time, please check the “Apportionment Instructions” file in the LNF Online Services > Help > User Fees section.");
                    string[] emails = ac.Emails.Split(',');

                    if (emails.Length > 0)
                    {
                        if (!noEmail)
                        {
                            if (recipients != null)
                                emails = emails.Concat(recipients).ToArray();

                            ServiceProvider.Current.Email.SendMessage(0, "LNF.Billing.ApportionmentUtility.SendMonthlyApportionmentEmails(DateTime period, string message = null, string[] recipients = null, bool noEmail = false)", Subject, bodyHtml.ToString(), "lnf-billing@umich.edu", emails, isHtml: true);
                        }

                        // Always increment result even if noEmail == true so we can at least return how many emails would be sent.
                        // Note this is not incremented unless an email was found for the user, even when there are recipients included.
                        result++;

                        timer.AddData("Needs apportionment: {0}", string.Join(",", ac.Emails));
                    }
                    else
                        timer.AddData("Needs apportionment: no email found for {0}", ac.DisplayName);
                }
            }

            return result;
        }

        public int CheckPassbackViolations()
        {
            DateTime startDate = DateTime.Now.FirstOfMonth().AddMonths(-1);
            DateTime endDate = DateTime.Now.FirstOfMonth();
            int count = 0;

            using (var timer = LogTaskTimer.Start("ApportionmentUtility.CheckPassbackViolations", "startDate = '{0:yyyy-MM-dd}', endDate = '{1:yyyy-MM-dd}', count = {2}", () => new object[] { startDate, endDate, count }))
            {
                int[] clientIds = ServiceProvider.Current.PhysicalAccess.CheckPassbackViolations(startDate, endDate);
                count = clientIds.Length;

                foreach (int id in clientIds)
                {
                    Client client = Session.Single<Client>(id);
                    string recip = ClientManager.PrimaryEmail(client);
                    string subj = "Lab access data anomaly - please check!";
                    string body = client.DisplayName + ":<br /><br />"
                     + "There appears to have been an error with your record of entrances/exists from "
                     + "one or more of the LNF laboratories. Please check the system to ensure that the time "
                     + "recorded is correct.<br /><br />"
                     + "This matter must be resolved by the close of the third business day of this month.";

                    if (recip.Trim().Length > 0)
                        ServiceProvider.Current.Email.SendMessage(0, "LNF.Billing.ApportionmentUtility.CheckPassbackViolations()", subj, body, SendEmail.SystemEmail, new string[] { recip }, isHtml: true);

                    timer.AddData("Has passback violation: ", recip);
                }

                return count;
            }
        }

        public decimal GetApportionment(ClientAccount ca, Room room, DateTime period)
        {
            if (ca == null) return 0;
            if (room == null) return 0;
            RoomApportionment appor = Session.Query<RoomApportionment>().FirstOrDefault(x => x.Client == ca.ClientOrg.Client && x.Account == ca.Account && x.Room == room && x.Period == period);
            if (appor == null) return 0;
            return appor.ChargeDays;
        }

        public int GetPhysicalDays(DateTime period, int clientId, int roomId)
        {
            var roomData = Session.Query<RoomData>().Where(x => x.Period == period && x.Client.ClientID == clientId && (x.Room.RoomID == roomId || x.Room.ParentID == roomId));
            return roomData.Select(x => x.EvtDate).Distinct().Count();
        }

        public int GetMinimumDays(DateTime period, int clientId, int roomId, int orgId)
        {
            var toolData = Session.Query<ToolData>().Where(x => x.Period == period && x.ClientID == clientId && x.IsActive).ToList();
            var actDates = toolData.Where(x => IsInRoom(x, roomId) && IsInOrg(x, orgId)).Select(x => x.ActDate).ToList();
            int result = actDates.Distinct().Count();
            return result;
        }

        public bool IsInOrg(ToolData td, int orgId)
        {
            // get the account for this td
            var a = Session.Single<Account>(td.AccountID);

            if (a == null)
                throw new Exception(string.Format("Cannot find account with AccountID {0}", td.AccountID));

            // check if the orgId is a.OrgID
            return orgId == a.Org.OrgID;
        }

        public bool IsInRoom(ToolData td, int roomId)
        {
            // It is possible that td.RoomID is null, however this hasn't happened since Dec 2007 so it
            // probably won't happen again, but the column does allow nulls.
            if (!td.RoomID.HasValue) return false;

            // Sometimes there is a room (e.g. Conference Room) with a non-null RoomID that does not 
            // have an entry in the Room table. In this case td.RoomID != roomId.
            if (!CacheManager.Current.Rooms().Any(x => x.RoomID == td.RoomID.Value))
                return false;

            // get the room for this td
            var r = CacheManager.Current.GetRoom(td.RoomID.Value);

            // check if roomId is either r.RoomID or r.ParentID
            return roomId == r.RoomID || roomId == r.ParentID;
        }

        public decimal GetTotalEntries(DateTime period, int clientId, int roomId)
        {
            var roomData = Session.Query<RoomData>().Where(x => x.Period == period && x.Client.ClientID == clientId && (x.Room.RoomID == roomId || x.Room.ParentID == roomId));
            if (roomData != null && roomData.Count() > 0)
                return roomData.Sum(x => x.Entries);
            else
                return 0;
        }

        public decimal GetAccountEntries(DateTime period, int clientId, int roomId, int accountId)
        {
            var roomBilling = Session.Query<RoomBilling>().Where(x => x.Period == period && x.ClientID == clientId && x.RoomID == roomId && x.AccountID == accountId).ToArray();
            if (roomBilling != null && roomBilling.Length > 0)
                return roomBilling.Sum(x => x.Entries);
            else
                return 0;
        }

        public void UpdateRoomBillingEntries(DateTime period, int clientId, int roomId, int accountId, decimal entries)
        {
            RoomBilling rb = Session.Query<RoomBilling>().First(x => x.Period == period && x.ClientID == clientId && x.RoomID == roomId && x.AccountID == accountId);
            rb.Entries = entries;

            RoomBillingUserApportionData appData = Session.Query<RoomBillingUserApportionData>().FirstOrDefault(x => x.Period == period && x.Client.ClientID == clientId && x.Room.RoomID == roomId && x.Account.AccountID == accountId);

            if (appData == null)
            {
                appData = new RoomBillingUserApportionData()
                {
                    Account = Session.Single<Account>(rb.AccountID),
                    ChargeDays = 0,
                    Client = Session.Single<Client>(rb.ClientID),
                    Period = period,
                    Room = Session.Single<Room>(rb.RoomID),
                    Entries = 0
                };

                Session.Insert(appData);
            }

            appData.Entries = entries;
        }

        public decimal GetDefaultApportionmentPercentage(int clientId, int roomId, int accountId)
        {
            ApportionmentDefault item = Session.Query<ApportionmentDefault>()
                .FirstOrDefault(x => x.Client.ClientID == clientId && x.Room.RoomID == roomId && x.Account.AccountID == accountId);

            if (item == null)
                return 0;
            else
                return item.Percentage;
        }

        public int UpdateChildRoomEntryApportionment(DateTime period, int clientId, int parentRoomId)
        {
            int result = 0;

            Room parentRoom = Session.Single<Room>(parentRoomId);

            //make sure this really is a parent room
            if (parentRoom.ParentID == null)
            {
                //need to know the child rooms
                Room[] children = Session.Query<Room>().Where(x => x.ParentID == parentRoomId).ToArray();

                //continue only if there are children rooms
                if (children.Length > 0)
                {
                    //entries and physical days for each child room
                    var entries = Session.Query<RoomData>()
                        .Where(x => x.Period == period && x.Client.ClientID == clientId && x.Room.ParentID == parentRoomId)
                        .GroupBy(x => x.Room.RoomID)
                        .Select(g => new { RoomID = g.Key, TotalEntries = g.Sum(n => n.Entries), PhysicalDays = (decimal)g.Select(n => n.EvtDate).Distinct().Count() })
                        .ToArray();

                    //child room apportionment records, this is what we'll be updating
                    RoomApportionment[] childAppor = Session.Query<RoomApportionment>().Where(x => x.Room.ParentID == parentRoomId && x.Period == period && x.Client.ClientID == clientId).ToArray();

                    //also update the child user apportionment data
                    RoomBillingUserApportionData[] childUserAppor = Session.Query<RoomBillingUserApportionData>().Where(x => x.Room.ParentID == parentRoomId && x.Period == period && x.Client.ClientID == clientId).ToArray();

                    //parent room apportionment records
                    RoomApportionment[] parentAppor = Session.Query<RoomApportionment>().Where(x => x.Period == period && x.Client.ClientID == clientId && x.Room == parentRoom).ToArray();

                    //get the pct for each acct based on parent room day apporiontment
                    foreach (RoomApportionment pa in parentAppor)
                    {
                        decimal pct = Convert.ToDecimal(Math.Round(pa.ChargeDays / pa.PhysicalDays, 3, MidpointRounding.AwayFromZero));

                        //get the total for each child room
                        foreach (Room child in children)
                        {
                            var e = entries.FirstOrDefault(x => x.RoomID == child.RoomID);

                            //e will be null if they didn't go into a room (e.g. wet chem)
                            if (e != null)
                            {
                                //the pct for this acct
                                decimal chargeDays = e.PhysicalDays * pct;
                                decimal chargeEntries = e.TotalEntries * pct;

                                //get the child RoomApportionment record
                                RoomApportionment ca = childAppor.FirstOrDefault(x => x.Room == child && x.Account == pa.Account);

                                if (ca != null)
                                {
                                    //update the record
                                    ca.Entries = chargeEntries;
                                    ca.ChargeDays = chargeDays;
                                    result++;
                                }

                                //update/insert the user apportionment data
                                RoomBillingUserApportionData uad = childUserAppor.FirstOrDefault(x => x.Room == child && x.Account == pa.Account);
                                if (uad != null)
                                {
                                    uad.Entries = chargeEntries;
                                    uad.ChargeDays = chargeDays;
                                }
                                else
                                {
                                    //create the record if it does not exist
                                    uad = new RoomBillingUserApportionData()
                                    {
                                        Client = pa.Client,
                                        Account = pa.Account,
                                        Room = child,
                                        Period = period,
                                        ChargeDays = chargeDays,
                                        Entries = chargeEntries
                                    };
                                    Session.Insert(uad);
                                }
                            }
                        }
                    }
                }
            }

            return result;
        }
    }
}
