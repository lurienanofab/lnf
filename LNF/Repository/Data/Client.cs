using LNF.Data;
using LNF.Models.Data;
using LNF.Repository.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Repository.Data
{
    /// <summary>
    /// Represents a user of the system.
    /// </summary>
    public class Client : ActiveDataItem, IPrivileged
    {
        /// <summary>
        /// The unique id of a Client
        /// </summary>
        public virtual int ClientID { get; set; }

        /// <summary>
        /// The first name of a Client
        /// </summary>
        public virtual string FName { get; set; }

        /// <summary>
        /// The middle name of a Client
        /// </summary>
        public virtual string MName { get; set; }

        /// <summary>
        /// The last name of a Client
        /// </summary>
        public virtual string LName { get; set; }

        /// <summary>
        /// The unique username of a Client used to log in
        /// </summary>
        public virtual string UserName { get; set; }

        /// <summary>
        /// Id used to indicate citizenship demographic data
        /// </summary>
        public virtual int DemCitizenID { get; set; }

        /// <summary>
        /// Id used to indicate gender demographic data
        /// </summary>
        public virtual int DemGenderID { get; set; }

        /// <summary>
        /// Id used to indicate racial demographic data
        /// </summary>
        public virtual int DemRaceID { get; set; }

        /// <summary>
        /// Id used to indicate ethnicity demographic data
        /// </summary>
        public virtual int DemEthnicID { get; set; }

        /// <summary>
        /// Id used to indicate disability demographic data
        /// </summary>
        public virtual int DemDisabilityID { get; set; }

        /// <summary>
        /// The privilges granted to a Client
        /// </summary>
        public virtual ClientPrivilege Privs { get; set; }

        /// <summary>
        /// The communities assigned to a Client
        /// </summary>
        public virtual int Communities { get; set; }

        /// <summary>
        /// Id used to indicate a technical field to which a Client is associated
        /// </summary>
        public virtual int TechnicalFieldID { get; set; }

        /// <summary>
        /// Indicates if a Client has viewed the new user ethics video
        /// </summary>
        public virtual bool? IsChecked { get; set; }

        /// <summary>
        /// Indicates if a Client has passed the new user safety test
        /// </summary>
        public virtual bool? IsSafetyTest { get; set; }

        /// <summary>
        /// Indicates if a Client is currently active
        /// </summary>
        public override bool Active { get; set; }

        /// <summary>
        /// The display name of a Client
        /// </summary>
        public virtual string DisplayName
        {
            get { return ClientItem.GetDisplayName(LName, FName); }
        }

        /// <summary>
        /// Gets the record id used in the ActiveLog
        /// </summary>
        /// <returns>A ClientID integer value</returns>
        public override int Record() { return ClientID; }

        /// <summary>
        /// The table name used in the ActiveLog
        /// </summary>
        /// <returns>A table name string value</returns>
        public override string TableName() { return "Client"; }

        /// <summary>
        /// Gets the non null value of IsChecked
        /// </summary>
        /// <returns>True if a Client has watched the ethical video, otherwise false</returns>
        public virtual bool HasWatchedEthicalVideo()
        {
            return IsChecked.GetValueOrDefault();
        }

        /// <summary>
        /// Gets the non null value of IsSafetyTest
        /// </summary>
        /// <returns>True if a Client has completed the safety test, otherwise false</returns>
        public virtual bool HasTakenSafetyTest()
        {
            return IsSafetyTest.GetValueOrDefault();
        }

        public virtual ClientInfo GetClientInfo()
        {
            ClientInfo result = DA.Current.Single<ClientInfo>(ClientID);
            return result;
        }

        public virtual string PrimaryEmail()
        {
            ClientInfo c = GetClientInfo();
            if (c == null) return string.Empty;
            return c.Email;
        }

        public virtual string PrimaryPhone()
        {
            ClientInfo c = GetClientInfo();
            if (c == null) return string.Empty;
            return c.Phone;
        }

        public virtual Org PrimaryOrg()
        {
            ClientInfo c = GetClientInfo();
            if (c == null) return null;
            return DA.Current.Single<Org>(c.OrgID);
        }

        public virtual ClientOrg PrimaryClientOrg()
        {
            ClientInfo c = GetClientInfo();
            if (c == null) return null;
            return DA.Current.Single<ClientOrg>(c.ClientOrgID);
        }

        public virtual ChargeType MaxChargeType()
        {
            var result = ActiveClientOrgs()
                .Select(x => x.Org.OrgType.ChargeType)
                .OrderBy(x => x.ChargeTypeID)
                .LastOrDefault();

            return result;
        }

        /// <summary>
        /// Gets an unflitered list of ClientOrgs for this Client
        /// </summary>
        /// <param name="item">The Client item</param>
        /// <returns>A list of ClientOrg items</returns>
        public virtual IQueryable<ClientOrg> ClientOrgs()
        {
            return DA.Current.Query<ClientOrg>().Where(x => x.Client.ClientID == ClientID);
        }

        /// <summary>
        /// Gets ClientOrgs for this Client that are currently active
        /// </summary>
        /// <param name="item">The Client item</param>
        /// <returns>A list of ClientOrg items</returns>
        public virtual IQueryable<ClientOrg> ActiveClientOrgs()
        {
            return DA.Current.Query<ClientOrg>().Where(x => x.Client.ClientID == ClientID && x.Active);
        }

        /// <summary>
        /// Gets ClientOrgs for this Client that were active during the specified date range
        /// </summary>
        /// <param name="item">The Client item</param>
        /// <param name="sd">The start of the date range</param>
        /// <param name="ed">The end of the date range</param>
        /// <returns>A list of ClientOrg items</returns>
        public virtual IQueryable<ClientOrg> ActiveClientOrgs(DateTime sd, DateTime ed)
        {
            var query = DA.Current.Query<ActiveLog>().Where(x => x.TableName == "ClientOrg" && (x.EnableDate < ed && (x.DisableDate == null || x.DisableDate > sd)));
            var join = query.Join(DA.Current.Query<ClientOrg>(), o => o.Record, i => i.ClientOrgID, (outer, inner) => inner);
            return join.Where(x => x.Client.ClientID == ClientID);
        }

        /// <summary>
        /// Gets an unflitered list of ClientAccounts for this Client
        /// </summary>
        /// <param name="item">The Client item</param>
        /// <returns>A list of ClientAccount items</returns>
        public virtual IQueryable<ClientAccount> ClientAccounts()
        {
            return DA.Current.Query<ClientAccount>().Where(x => x.ClientOrg.Client.ClientID == ClientID);
        }

        /// <summary>
        /// Gets ClientAccounts for this Client that are currently active
        /// </summary>
        /// <param name="item">The Client item</param>
        /// <returns>A list of ClientAccount items</returns>
        public virtual IQueryable<ClientAccount> ActiveClientAccounts()
        {
            return DA.Current.Query<ClientAccount>().Where(x => x.ClientOrg.Client.ClientID == ClientID && x.Active && x.ClientOrg.Active);
        }

        /// <summary>
        /// Gets ClientAccounts for this Client that were active during the specified date range
        /// </summary>
        /// <param name="item">The Client item</param>
        /// <param name="sd">The start of the date range</param>
        /// <param name="ed">The end of the date range</param>
        /// <returns>A list of ClientAccount items</returns>
        public virtual IQueryable<ClientAccount> ActiveClientAccounts(DateTime sd, DateTime ed)
        {
            var query = DA.Current.Query<ActiveLog>().Where(x => x.TableName == "ClientAccount" && (x.EnableDate < ed && (x.DisableDate == null || x.DisableDate > sd)));
            var join = query.Join(DA.Current.Query<ClientAccount>(), o => o.Record, i => i.ClientAccountID, (outer, inner) => inner);
            return join.Where(x => x.ClientOrg.Client.ClientID == ClientID);
        }

        public virtual IList<Account> ActiveAccounts()
        {
            var result = ClientAccounts().Where(x => x.Active && x.ClientOrg.Active && x.Account.Active).Select(x => x.Account).ToList();
            return result;
        }

        public virtual IQueryable<Account> ActiveAccounts(DateTime sd, DateTime ed)
        {
            var result = ActiveClientAccounts(sd, ed).Select(x => x.Account).Distinct();
            return result;
        }

        public virtual TechnicalField TechnicalField()
        {
            return DA.Current.Single<TechnicalField>(TechnicalFieldID);
        }

        public virtual string TechnicalFieldName()
        {
            var tf = TechnicalField();

            if (tf != null)
                return tf.TechnicalFieldName;
            else
                return string.Empty;
        }

        public virtual DateTime? LastReservation()
        {
            return DA.Scheduler.Reservation.Query()
                .Where(x => x.Client == this && x.IsActive && x.IsStarted)
                .Max(x => x.ActualBeginDateTime);
        }

        public virtual string[] ActiveEmails()
        {
            //this function returns the same result as sselData.dbo.udf_ClientEmails()
            return DA.Current.Query<ClientOrg>().Where(x => x.Client == this && x.Active).Select(x => x.Email).Distinct().ToArray();
        }

        public virtual string AccountEmail(int accountId)
        {
            var co = ClientOrgUtility.SelectByClientAccount(this, DA.Current.Single<Account>(accountId)).FirstOrDefault();

            if (co != null)
                return co.Email;
            else
                return string.Empty;
        }

        public virtual string AccountPhone(int accountId)
        {
            var query = ClientOrgUtility.SelectByClientAccount(this, DA.Current.Single<Account>(accountId));

            if (query.Count() > 0)
                return query.First().Phone;
            else
                return string.Empty;
        }

        public virtual bool IsAdmin()
        {
            return this.HasPriv(ClientPrivilege.Administrator);
        }

        public virtual bool IsStaff()
        {
            return this.HasPriv(ClientPrivilege.Staff);
        }

        public virtual DateTime? LastRoomEntry()
        {
            DateTime? result = DA.Current.Query<RoomDataClean>().Where(x => x.Client == this).Max(x => x.EntryDT);
            return result;
        }

        public virtual ClientOrgInfo GetClientOrgInfo(int rank)
        {
            ClientOrgInfo result = DA.Current.Query<ClientOrgInfo>().Where(x => x.ClientID == ClientID && x.EmailRank == rank).FirstOrDefault();
            return result;
        }

        public virtual IQueryable<ClientAccountInfo> ActiveClientAccountInfos()
        {
            var result = DA.Current.Query<ClientAccountInfo>().Where(x => x.ClientID == ClientID && x.ClientAccountActive && x.ClientOrgActive);
            return result;
        }

        public virtual int TotalDaysInLab(Room r, DateTime period)
        {
            IList<RoomData> query = DA.Current.Query<RoomData>().Where(x => x.Client.ClientID == ClientID && x.Room == r && x.Period == period).ToList();
            IEnumerable<DateTime> days = query.Select(x => x.EvtDate);
            IEnumerable<int> distinctDays = days.Select(x => x.Day).Distinct();
            int result = distinctDays.Count();
            return result;
        }

        /// <summary>
        /// Checks to see if the given password is correct.
        /// </summary>
        /// <param name="item">The client for which this action is performed.</param>
        /// <param name="password">An unencrypted password.</param>
        /// <returns>True if the password is correct, otherwise false.</returns>
        public virtual bool CheckPassword(string password)
        {
            return ClientUtility.CheckPassword(ClientID, password);
        }

        /// <summary>
        /// Sets the client password to the given value.
        /// </summary>
        /// <param name="item">The client for which this action is performed.</param>
        /// <param name="password">An unencrypted password.</param>
        /// <returns>The number of rows updated.</returns>
        public virtual int SetPassword(string password)
        {
            return ClientUtility.SetPassword(ClientID, password);
        }

        /// <summary>
        /// Sets the client password to the UserName.
        /// </summary>
        public virtual void ResetPassword()
        {
            SetPassword(UserName);
        }

        public virtual void Update(ClientItem model)
        {
            FName = model.FName;
            MName = model.MName;
            LName = model.LName;
            UserName = model.UserName;
            DemCitizenID = model.DemCitizenID;
            DemGenderID = model.DemGenderID;
            DemRaceID = model.DemRaceID;
            DemEthnicID = model.DemEthnicID;
            DemDisabilityID = model.DemDisabilityID;
            Privs = model.Privs;
            Communities = model.Communities;
            TechnicalFieldID = model.TechnicalFieldID;
            IsChecked = model.IsChecked;
            IsSafetyTest = model.IsSafetyTest;

            if (model.ClientActive)
                this.Enable();
            else
                this.Disable();
        }

        public static Client Find(string username)
        {
            return DA.Current.Query<Client>().FirstOrDefault(x => x.UserName == username);
        }

        public static Client FindByDisplayName(string displayName)
        {
            string[] splitter = displayName.Split(',');

            if (splitter.Length < 2)
                return null;

            string lname = splitter[0].Trim();
            string fname = splitter[1].Trim();

            Client result = DA.Current.Query<Client>().FirstOrDefault(x => x.LName == lname && x.FName == fname);

            return result;
        }

        /// <summary>
        /// Find Clients that were active during the period, filtered on the current user status. Administrators see all users, Staff see themselves only unless displayAllUsersToStaff is true, Managers see themselves and any users they manage, Normal users see only themselves.
        /// </summary>
        /// <param name="client">The current user</param>
        /// <param name="period">The period during which clients must be active</param>
        /// <param name="displayAllUsersToStaff">When true all users are displayed if the current user is staff</param>
        /// <returns>A list of active clients during the period, filtered on the current user</returns>
        public static IEnumerable<Client> FindByPeriod(Client client, DateTime period, bool displayAllUsersToStaff = false)
        {
            //populate the user dropdown list

            DateTime sd = period;
            DateTime ed = sd.AddMonths(1);

            IList<ActiveLogClientAccount> allClientAccountsActiveInPeriod = DA.Current.Query<ActiveLogClientAccount>()
                .Where(x => x.EnableDate < ed && (x.DisableDate == null || x.DisableDate > sd))
                .ToList();

            // check for administrator, manager, or normal user

            List<Client> result = new List<Client>();

            bool isUserStaffAndAllowedAllUserInfo = client.HasPriv(ClientPrivilege.Staff) && displayAllUsersToStaff;

            int[] clientIds = null;

            if (client.HasPriv(ClientPrivilege.Administrator | ClientPrivilege.Developer) || isUserStaffAndAllowedAllUserInfo)
            {
                //administrators see everyone
                clientIds = allClientAccountsActiveInPeriod.Select(x => x.ClientID).Distinct().ToArray();
            }
            else
            {
                //not an admin - get all of this user's ClientAccounts
                var clientAccts = allClientAccountsActiveInPeriod.Where(x => x.ClientID == client.ClientID).ToArray();

                var temp = new List<int>();

                foreach (ActiveLogClientAccount ca in clientAccts)
                {
                    //check to see if they are a manager
                    if (ca.Manager)
                    {
                        var otherClientAccts = allClientAccountsActiveInPeriod.Where(x => x.AccountID == ca.AccountID);
                        temp.AddRange(otherClientAccts.Select(x => x.ClientID).ToArray());
                    }
                    else
                    {
                        temp.Add(client.ClientID);
                    }
                }

                clientIds = temp.Distinct().ToArray();
            }

            result = DA.Current.Query<Client>().Where(x => clientIds.Contains(x.ClientID)).ToList();

            return result;
        }

        public static IEnumerable<Client> FindByCommunity(int flag, bool? active = true)
        {
            IQueryable<Client> query;

            if (active.HasValue)
                query = DA.Current.Query<Client>().Where(x => x.Active == active.Value);
            else
                query = DA.Current.Query<Client>();

            var result = query.Where(x => (x.Communities & flag) > 0);

            return result.ToList();
        }

        public static IEnumerable<Client> FindByTools(IEnumerable<int> resourceIds, bool? active = true)
        {
            List<Client> result = new List<Client>();
            List<ResourceClient> rclist = new List<ResourceClient>();

            foreach (int id in resourceIds)
                rclist.AddRange(DA.Current.Query<ResourceClient>().Where(x => x.ClientID > 0 && x.Resource.ResourceID == id && (x.Expiration == null || x.Expiration.Value < DateTime.Now)));

            if (active.HasValue)
                result.AddRange(rclist.Select(x => DA.Current.Single<Client>(x.ClientID)).Where(x => x.Active == active.Value));
            else
                result.AddRange(rclist.Select(x => DA.Current.Single<Client>(x.ClientID)));

            return result;
        }

        public static IEnumerable<Client> FindByManager(int managerClientId, bool? active = true)
        {
            List<Client> result = new List<Client>();
            List<ClientManager> cmlist = new List<ClientManager>();
            IList<ClientOrg> mgrClientOrgs;

            if (active.HasValue)
            {
                mgrClientOrgs = DA.Current.Query<ClientOrg>().Where(x => x.Client.ClientID == managerClientId && (x.IsFinManager || x.IsManager)).ToList();

                foreach (ClientOrg co in mgrClientOrgs)
                    cmlist.AddRange(DA.Current.Query<ClientManager>().Where(x => x.ManagerOrg == co && x.Active == active.Value));

                result.AddRange(cmlist.Select(x => x.ClientOrg.Client));
            }
            else
            {
                mgrClientOrgs = DA.Current.Query<ClientOrg>().Where(x => x.Client.ClientID == managerClientId && (x.IsFinManager || x.IsManager)).ToList();
                foreach (ClientOrg co in mgrClientOrgs)
                    cmlist.AddRange(DA.Current.Query<ClientManager>().Where(x => x.ManagerOrg == co));
                result.AddRange(cmlist.Select(x => x.ClientOrg.Client));
            }

            return result;
        }

        /// <summary>
        /// Gets a Client display name in the standard format
        /// </summary>
        public static string GetDisplayName(string lname, string fname)
        {
            return ClientItem.GetDisplayName(lname, fname);
        }

        public static IEnumerable<Client> FindByPrivilege(ClientPrivilege priv, bool? active = true)
        {
            IQueryable<Client> query;

            if (active.HasValue)
                query = DA.Current.Query<Client>().Where(x => x.Active == active.Value);
            else
                query = DA.Current.Query<Client>();

            IList<Client> result = query.Where(x => (x.Privs & priv) > 0).ToList();

            return result;
        }
    }
}
