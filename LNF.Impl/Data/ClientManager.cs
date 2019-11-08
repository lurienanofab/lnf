using LNF.Mail;
using LNF.Models;
using LNF.Models.Billing;
using LNF.Models.Data;
using LNF.Models.Scheduler;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Caching;

namespace LNF.Impl.Data
{
    public class ClientManager : ManagerBase, IClientManager
    {
        private readonly ObjectCache _cache = new MemoryCache("ClientManagerCache");

        public ClientManager(IProvider provider) : base(provider) { }

        public IEnumerable<IAccount> ActiveAccounts(int clientId)
        {
            return ClientAccounts(clientId).Where(x => x.ClientActive && x.ClientOrgActive && x.AccountActive);
        }

        public IEnumerable<IAccount> ActiveAccounts(int clientId, DateTime sd, DateTime ed)
        {
            var accountIds = ActiveClientAccounts(clientId, sd, ed).Select(x => x.AccountID).Distinct().ToArray();
            var result = Session.Query<AccountInfo>().Where(x => accountIds.Contains(x.AccountID)).CreateModels<IAccount>();
            return result;
        }

        public IEnumerable<IClientAccount> ActiveClientAccounts(int clientId)
        {
            return Session.Query<ClientAccountInfo>().Where(x => x.ClientID == clientId && x.ClientAccountActive && x.ClientOrgActive).CreateModels<IClientAccount>();
        }

        public IEnumerable<IClientAccount> ActiveClientAccounts(int clientId, DateTime sd, DateTime ed)
        {
            var query = Session.Query<ActiveLog>().Where(x => x.TableName == "ClientAccount" && (x.EnableDate < ed && (x.DisableDate == null || x.DisableDate > sd)));
            var join = query.Join(Session.Query<ClientAccountInfo>(), o => o.Record, i => i.ClientAccountID, (outer, inner) => inner);
            return join.Where(x => x.ClientID == clientId).CreateModels<IClientAccount>();
        }

        public IEnumerable<IClient> ActiveClientOrgs(int clientId)
        {
            return Session.Query<ClientOrgInfo>().Where(x => x.ClientID == clientId && x.ClientOrgActive).CreateModels<IClient>();
        }

        public IEnumerable<IClient> ActiveClientOrgs(int clientId, DateTime sd, DateTime ed)
        {
            var query = Session.Query<ActiveLog>().Where(x => x.TableName == "ClientOrg" && (x.EnableDate < ed && (x.DisableDate == null || x.DisableDate > sd)));
            var join = query.Join(Session.Query<ClientOrgInfo>(), o => o.Record, i => i.ClientOrgID, (outer, inner) => inner);
            return join.Where(x => x.ClientID == clientId).CreateModels<IClient>();
        }

        public string[] ActiveEmails(int clientId)
        {
            //this function returns the same result as sselData.dbo.udf_ClientEmails()
            return Session.Query<ClientOrgInfo>().Where(x => x.ClientID == clientId && x.ClientOrgActive).Select(x => x.Email).Distinct().ToArray();
        }

        public bool CheckPassword(int clientId, string password)
        {
            if (password == Provider.DataAccess.UniversalPassword)
                return true;

            var pw = Provider.Encryption.EncryptText(password);
            var hash = Provider.Encryption.Hash(password);

            bool result = Session.NamedQuery("CheckPassword")
                .SetParameters(new { ClientID = clientId, Password = pw, PasswordHash = hash })
                .Result<bool>();

            return result;
        }

        public string CleanMiddleName(string raw)
        {
            if (raw == null) raw = string.Empty;

            //strip period if entered
            string stripped = raw.Trim();
            if (stripped.Length > 0)
            {
                if (stripped.EndsWith("."))
                    stripped = stripped.Remove(stripped.Length - 1, 1);
            }
            return stripped;
        }

        public IEnumerable<IClientAccount> ClientAccounts(int clientId)
        {
            return Session.Query<ClientAccountInfo>().Where(x => x.ClientID == clientId).CreateModels<IClientAccount>();
        }

        public IEnumerable<IClient> ClientOrgs(int clientId)
        {
            return Session.Query<ClientOrgInfo>().Where(x => x.ClientID == clientId).CreateModels<IClient>();
        }

        public IEnumerable<IClient> FindByCommunity(int flag, bool? active = true)
        {
            if (active.HasValue)
                return Session.Query<ClientInfo>().Where(x => x.ClientActive == active.Value && (x.Communities & flag) > 0).CreateModels<IClient>();
            else
                return Session.Query<ClientInfo>().Where(x => (x.Communities & flag) > 0).CreateModels<IClient>();
        }

        public IClient FindByDisplayName(string displayName)
        {
            string[] splitter = displayName.Split(',');

            if (splitter.Length < 2)
                return null;

            string lname = splitter[0].Trim();
            string fname = splitter[1].Trim();

            var result = Session.Query<ClientInfo>().FirstOrDefault(x => x.LName == lname && x.FName == fname).CreateModel<IClient>();

            return result;
        }

        public IEnumerable<IClient> FindByManager(int managerClientId, bool? active = true)
        {
            List<IClient> result = new List<IClient>();
            List<Repository.Data.ClientManager> cmlist = new List<Repository.Data.ClientManager>();
            IList<ClientOrgInfo> mgrClientOrgs;

            if (active.HasValue)
            {
                mgrClientOrgs = Session.Query<ClientOrgInfo>().Where(x => x.ClientID == managerClientId && (x.IsFinManager || x.IsManager)).ToList();

                foreach (ClientOrgInfo co in mgrClientOrgs)
                    cmlist.AddRange(Session.Query<Repository.Data.ClientManager>().Where(x => x.ManagerOrg.ClientOrgID == co.ClientOrgID && x.Active == active.Value));

                result.AddRange(cmlist.AsQueryable().CreateModels<IClient>());
            }
            else
            {
                mgrClientOrgs = Session.Query<ClientOrgInfo>().Where(x => x.ClientID == managerClientId && (x.IsFinManager || x.IsManager)).ToList();
                foreach (ClientOrgInfo co in mgrClientOrgs)
                    cmlist.AddRange(Session.Query<Repository.Data.ClientManager>().Where(x => x.ManagerOrg.ClientOrgID == co.ClientOrgID));
                result.AddRange(cmlist.AsQueryable().CreateModels<IClient>());
            }

            return result;
        }

        ///// <summary>
        ///// Find Clients that were active during the period, filtered on the current user status. Administrators see all users, Staff see themselves only unless displayAllUsersToStaff is true, Managers see themselves and any users they manage, Normal users see only themselves.
        ///// </summary>
        ///// <param name="client">The current user</param>
        ///// <param name="period">The period during which clients must be active</param>
        ///// <param name="displayAllUsersToStaff">When true all users are displayed if the current user is staff</param>
        ///// <returns>A list of active clients during the period, filtered on the current user</returns>
        public IEnumerable<IClient> FindByPeriod(IClient client, DateTime period, bool displayAllUsersToStaff = false)
        {
            //populate the user dropdown list

            DateTime sd = period;
            DateTime ed = sd.AddMonths(1);

            IList<ActiveLogClientAccount> allClientAccountsActiveInPeriod = Session.Query<ActiveLogClientAccount>()
                .Where(x => x.EnableDate < ed && (x.DisableDate == null || x.DisableDate > sd))
                .ToList();

            // check for administrator, manager, or normal user

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

            var result = Session.Query<ClientInfo>().Where(x => clientIds.Contains(x.ClientID)).CreateModels<IClient>();

            return result;
        }

        public IEnumerable<IClient> FindByPrivilege(ClientPrivilege priv, bool? active = true)
        {
            if (active.HasValue)
                return Session.Query<ClientInfo>().Where(x => x.ClientActive == active.Value && (x.Privs & priv) > 0).CreateModels<IClient>();
            else
                return Session.Query<ClientInfo>().Where(x => (x.Privs & priv) > 0).CreateModels<IClient>();
        }

        public IEnumerable<IClient> FindByTools(int[] resourceIds, bool? active = true)
        {
            var query = Session.Query<ResourceClientInfo>().Where(x => x.ClientID > 0 && resourceIds.Contains(x.ResourceID) && (x.Expiration == null || x.Expiration.Value < DateTime.Now)).CreateModels<IResourceClient>();
            var join = query.Join(Session.Query<ClientInfo>(), o => ((IPrivileged)o).ClientID, i => i.ClientID, (o, i) => i);
            var result = join.Where(x => x.ClientActive == active.GetValueOrDefault(x.ClientActive)).CreateModels<IClient>();
            return result;
        }

        ///// <summary>
        ///// Returns the number of currently active accounts for a given client.
        ///// </summary>
        public int GetActiveAccountCount(int clientId)
        {
            int result = Session.Query<ActiveLogClientAccount>().Count(x => x.ClientID == clientId && !x.DisableDate.HasValue);
            return result;
        }

        public IEnumerable<IClient> GetClients(int limit, int skip = 0)
        {
            if (limit > 100)
                throw new ArgumentOutOfRangeException("The parameter 'limit' must not be greater than 100.");

            var result = Session.Query<ClientInfo>().Skip(skip).Take(limit).OrderBy(x => x.DisplayName).CreateModels<IClient>();

            return result;
        }

        public IEnumerable<IClient> GetActiveClients()
        {
            return Session.Query<ClientInfo>().Where(x => x.ClientActive).CreateModels<IClient>().ToList();
        }

        public IEnumerable<IClient> GetActiveClients(ClientPrivilege priv)
        {
            return GetActiveClients().Where(x => (x.Privs & priv) > 0).ToList();
            //var ed = DateTime.Now.AddDays(1);
            //var sd = ed.AddMonths(-1);
            //return GetActiveClients(sd, ed, priv);
        }

        public IEnumerable<IClient> GetActiveClients(DateTime sd, DateTime ed)
        {
            /*
             * ------------------------------------------------------------------------------------------
             * -- this method should return the same results as sselData.dbo.Client_Select @Action = 'All'
             * ------------------------------------------------------------------------------------------
             * 
             * 	IF @eDate IS NULL
             *  BEGIN
             *      IF @Action='NeedApportionment'
             *          SET @eDate = DATEADD(MM, 1, @sDate)
             *      ELSE
             *          SET @eDate = DATEADD(DD, 1, GETDATE())
             *  END
             *
             *  IF @sDate IS NULL
             *      SET @sDate = DATEADD(MM, -1, @eDate)

             *  SELECT DISTINCT c.ClientID, c.FName, c.MName, c.LName, c.UserName, c.[Password], c.PasswordHash, c.DemCitizenID, c.DemGenderID
             *      , c.DemRaceID, c.DemEthnicID, c.DemDisabilityID, c.Privs, c.Communities, c.TechnicalInterestID, c.Active, c.isChecked, c.isSafetyTest
             *      , c.LName + ', ' + c.FName AS DisplayName
             *  FROM Client c
             *  WHERE (c.Privs & @Privs > 0) AND c.ClientID IN
             *  (
             *      SELECT Record
             *      FROM ActiveLog
             *      WHERE TableName = 'Client' AND
             *        EnableDate < @eDate AND
             *        ((DisableDate IS NULL) OR (DisableDate > @sDate))
             *  )
             *  ORDER BY DisplayName 
             *  
             */

            var activeLogs = Session.Query<ActiveLog>().Where(x => x.TableName == "Client" && x.EnableDate < ed && (!x.DisableDate.HasValue || x.DisableDate.Value > sd)).CreateModels<IActiveLog>();

            var join = activeLogs
                .Join(Session.Query<ClientInfo>(), o => o.Record, i => i.ClientID, (o, i) => i);

            var result = join.CreateModels<IClient>();

            return result;
        }

        public IEnumerable<IClient> GetActiveClients(DateTime sd, DateTime ed, ClientPrivilege priv)
        {
            var activeLogs = Session.Query<ActiveLog>().Where(x => x.TableName == "Client" && x.EnableDate < ed && (!x.DisableDate.HasValue || x.DisableDate.Value > sd)).CreateModels<IActiveLog>();

            var join = activeLogs
                .Join(Session.Query<ClientInfo>(), o => o.Record, i => i.ClientID, (o, i) => i);

            var result = join.CreateModels<IClient>();

            return result;
        }

        public IClient GetClient(string username) => SingleClient(username);

        public IClient GetClient(int clientId) => SingleClient(clientId);

        public IClient GetClient(int clientId, int rank)
        {
            var result = Session.Query<ClientOrgInfo>().FirstOrDefault(x => x.ClientID == clientId && x.EmailRank == rank).CreateModel<IClient>();
            return result;
        }

        public DateTime? LastReservation(int clientId)
        {
            return Session.Query<Reservation>()
                .Where(x => x.Client.ClientID == clientId && x.IsActive && x.IsStarted)
                .Max(x => x.ActualBeginDateTime);
        }

        public DateTime? LastRoomEntry(int clientId)
        {
            DateTime? result = Session.Query<RoomDataClean>().Where(x => x.ClientID == clientId).Max(x => x.EntryDT);
            return result;
        }

        public IClient Login(string username, string password)
        {
            if (string.IsNullOrEmpty(password))
                return null;

            var client = ClientInfo.Find(username);

            if (client == null)
                throw new Exception("Invalid username.");

            if (!string.IsNullOrEmpty(Provider.DataAccess.UniversalPassword) && password == Provider.DataAccess.UniversalPassword)
                return client.CreateModel<IClient>();

            if (CheckPassword(client.ClientID, password))
                return client.CreateModel<IClient>();
            else
                throw new Exception("Invalid password.");
        }

        public IChargeType MaxChargeType(int clientId)
        {
            var client = ActiveClientOrgs(clientId)
                .OrderByDescending(x => x.ChargeTypeID)
                .FirstOrDefault();

            var result = new ChargeTypeItem();

            return result;
        }

        ///// <summary>
        ///// Sets the user's password.
        ///// </summary>
        public int SetPassword(int clientId, string password)
        {
            var pw = Provider.Encryption.EncryptText(password);
            var hash = Provider.Encryption.Hash(password);

            int result = Session.NamedQuery("SetPassword")
                .SetParameters(new { ClientID = clientId, Password = pw, PasswordHash = hash })
                .Result<int>();

            return result;
        }

        public IClient StoreClientInfo(ref int clientId, string lname, string fname, string mname, string username, IClientDemographics demographics, IEnumerable<IPriv> privs, IEnumerable<ICommunity> communities, int technicalFieldId, int orgId, int roleId, int deptId, string email, string phone, bool isManager, bool isFinManager, DateTime? subsidyStart, DateTime? newFacultyStart, int[] addedAddressIds, int[] deletedAddressIds, int[] clientManagerIds, int[] clientAccountIds, out string alert)
        {
            //add rows to Client, ClientSite and ClientOrg for new entries

            var client = Session.Single<Client>(clientId);
            //IClient c;

            bool isNewClientEntry = (client == null);

            if (isNewClientEntry)
            {
                //add an entry to the client table
                client = NewClient(username, username, lname, fname, LNF.Data.PrivUtility.CalculatePriv(privs), true);
                clientId = client.ClientID;
            }
            else
            {
                client.UserName = username;
                client.FName = fname;
                client.LName = lname;

                //store Privs's
                client.Privs = LNF.Data.PrivUtility.CalculatePriv(privs);

                SetPassword(client.ClientID, client.UserName);
            }

            client.MName = CleanMiddleName(mname);
            UpdateClientDemographics(demographics);

            //store NULL for non-entered usertype
            client.Communities = LNF.Data.CommunityUtility.CalculateFlag(communities);
            client.TechnicalFieldID = technicalFieldId;

            //next the ClientOrg table
            ClientOrg co = Session.Query<ClientOrg>().FirstOrDefault(x => x.Client == client && x.Org.OrgID == orgId);

            if (co == null)
            {
                // need new row in ClientOrg
                co = new ClientOrg
                {
                    Client = client,
                    Org = Session.Single<Org>(orgId),
                    ClientAddressID = 0
                };

                Session.Insert(co);
            }

            co.Role = Session.Single<Role>(roleId);
            co.Department = Session.Single<Department>(deptId);
            co.Email = email;
            co.Phone = phone;
            co.IsManager = isManager;
            co.IsFinManager = isFinManager;
            co.SubsidyStartDate = subsidyStart;
            co.NewFacultyStartDate = newFacultyStart;

            Provider.Data.ActiveLog.Enable("ClientOrg", co.ClientOrgID);

            //find any address that need to be dealt with
            if (addedAddressIds != null && addedAddressIds.Length > 0)
            {
                foreach (int addrId in addedAddressIds)
                {
                    co.ClientAddressID = addrId;
                }
            }

            if (deletedAddressIds != null && deletedAddressIds.Length > 0)
            {
                co.ClientAddressID = 0;
                var addrs = Session.Query<Address>().Where(x => deletedAddressIds.Contains(x.AddressID));
                Session.Delete(addrs);
            }

            //update rows in ClientManager as needed
            var clientManagers = Session.Query<Repository.Data.ClientManager>().Where(x => clientManagerIds.Contains(x.ClientManagerID));
            foreach (var cm in clientManagers)
            {
                cm.ClientOrg = co;
                Session.SaveOrUpdate(cm);
            }

            //update rows in ClientAccount as needed
            var clientAccounts = Session.Query<ClientAccount>().Where(x => clientAccountIds.Contains(x.ClientAccountID));
            foreach (var ca in clientAccounts)
            {
                ca.ClientOrg = co;
                Session.SaveOrUpdate(ca);
            }

            var c = client.CreateModel<IClient>();

            // For clients who have Lab User priv only, only allow access if they
            // have an active account. If access is not enabled, show an alert.
            UpdatePhysicalAccess(c, out alert);

            ClearClientCache();

            return c;
        }

        public ITechnicalField TechnicalField(int clientId)
        {
            var client = SingleClient(clientId);

            if (client == null) return null;

            return new TechnicalFieldItem
            {
                TechnicalFieldID = client.TechnicalInterestID,
                TechnicalFieldName = client.TechnicalInterestName
            };
        }

        public string TechnicalFieldName(int clientId)
        {
            var tf = TechnicalField(clientId);
            if (tf != null) return tf.TechnicalFieldName;
            else return string.Empty;
        }

        public int TotalDaysInLab(int clientId, int roomId, DateTime period)
        {
            IList<RoomData> query = Session.Query<RoomData>().Where(x => x.ClientID == clientId && x.RoomID == roomId && x.Period == period).ToList();
            IEnumerable<DateTime> days = query.Select(x => x.EvtDate);
            IEnumerable<int> distinctDays = days.Select(x => x.Day).Distinct();
            int result = distinctDays.Count();
            return result;
        }

        public bool Insert(IClient client)
        {
            throw new NotImplementedException();
        }

        public bool Update(IClient client)
        {
            var c = Session.Single<Client>(client.ClientID);

            if (c == null)
                return false;

            c.FName = client.FName;
            c.MName = client.MName;
            c.LName = client.LName;
            c.UserName = client.UserName;
            c.DemCitizenID = client.DemCitizenID;
            c.DemGenderID = client.DemGenderID;
            c.DemRaceID = client.DemRaceID;
            c.DemEthnicID = client.DemEthnicID;
            c.DemDisabilityID = client.DemDisabilityID;
            c.Privs = client.Privs;
            c.Communities = client.Communities;
            c.TechnicalFieldID = client.TechnicalInterestID;
            c.IsChecked = client.IsChecked;
            c.IsSafetyTest = client.IsSafetyTest;

            if (client.ClientActive)
                Provider.Data.ActiveLog.Enable("Client", c.ClientID);
            else
                Provider.Data.ActiveLog.Disable("Client", c.ClientID);

            ClearClientCache();

            return true;
        }

        public bool UpdatePhysicalAccess(IClient client, out string alert)
        {
            bool result;

            var check = LNF.Data.AccessCheck.Create(client, Provider);

            if (check.CanEnableAccess()) //access should be enabled per privs and accounts
            {
                if (!check.IsPhysicalAccessEnabled()) //does not already have physical access
                {
                    if (check.AllowReenable()) //access can be enabled based on old account rule
                    {
                        check.EnablePhysicalAccess();
                        alert = string.Empty;
                        result = true;
                    }
                    else
                    {
                        //if client has been disabled for a "long time", do not enable access and alert user
                        check.RemovePhysicalAccessPriv();
                        alert = "Note that this client has been inactive for so long that access is not automatically reenabled. The Physical Access privilege has been removed.";
                        result = false;
                    }
                }
                else
                {
                    // can enable access and physical access already enabled
                    alert = string.Empty;
                    result = true;
                }
            }
            else //access should not be enabled per privs and accounts
            {
                string temp = string.Empty;
                string reason = check.Reason;

                if (check.IsPhysicalAccessEnabled())
                {
                    check.DisablePhysicalAccess();
                    temp += reason + " Physical access has been disabled.";
                    reason = string.Empty;
                }

                // remove PhysicalAccess priv
                if (check.HasPhysicalAccessPriv)
                {
                    check.RemovePhysicalAccessPriv();
                    temp = reason + " The Physical Access privilege has been removed.";
                    reason = string.Empty;
                }

                // remove StoreUser priv if LabUser and no ActiveAccounts
                if (check.HasLabUserPriv && check.HasStoreUserPriv && !check.HasActiveAccounts)
                {
                    check.RemoveStoreUserPriv();
                    temp += reason + " The Store User privilege has been removed.";
                    reason = string.Empty;
                }

                alert = temp.Trim();

                // alert will be empty if nothing changed (privs removed or access disabled)

                result = false;
            }

            // alert should be an empty string unless access is not enabled (result = false)
            // and something changed (privs removed or access disabled)

            return result;
        }

        private Client NewClient(string username, string password, string lname, string fname, ClientPrivilege privs, bool active)
        {
            var result = new Client()
            {
                UserName = username,
                LName = lname,
                FName = fname,
                Privs = privs
            };

            Session.Insert(result);

            SetPassword(result.ClientID, password);

            if (active)
                Provider.Data.ActiveLog.Enable("Client", result.ClientID);
            else
                Provider.Data.ActiveLog.Disable("Client", result.ClientID);

            ClearClientCache();

            return result;
        }

        public IClientDemographics GetClientDemographics(int clientId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateClientDemographics(IClientDemographics value)
        {
            var client = Session.Single<Client>(value.ClientID);

            if (client == null)
                return false;

            client.DemCitizenID = value.DemCitizenID;
            client.DemEthnicID = value.DemEthnicID;
            client.DemRaceID = value.DemRaceID;
            client.DemGenderID = value.DemGenderID;
            client.DemDisabilityID = value.DemDisabilityID;

            Session.SaveOrUpdate(client);

            ClearClientCache();

            return true;
        }

        public IEnumerable<IClientAccount> GetClientAccounts(int clientId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClientAccount> GetActiveClientAccounts(int clientId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClientAccount> GetActiveClientAccounts(int clientId, DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public string AccountEmail(int clientId, int accountId)
        {
            var c = SelectByClientAccount(clientId, accountId).FirstOrDefault();

            if (c != null)
                return c.Email;
            else
                return null;
        }

        public string AccountPhone(int clientId, int accountId)
        {
            var c = SelectByClientAccount(clientId, accountId).FirstOrDefault();

            if (c != null)
                return c.Phone;
            else
                return null;
        }

        public IEnumerable<ListItem> AllActiveManagers()
        {
            var dt = GroupEmailManager.GetAllActiveManagers();

            var result = new List<ListItem>();

            foreach (DataRow dr in dt.Rows)
            {
                result.Add(new ListItem(dr.Field<int>("ClientID").ToString(), dr.Field<string>("DisplayName")));
            }

            return result;
        }

        public IEnumerable<IClient> GetActiveManagers()
        {
            return GetActiveManagers(false);
        }

        public IEnumerable<IClient> GetActiveManagers(bool includeFinancialManagers)
        {
            IQueryable<ClientOrgInfo> query;

            if (!includeFinancialManagers)
                query = Session.Query<ClientOrgInfo>().Where(x => x.IsManager && x.ClientOrgActive).OrderBy(x => x.DisplayName);
            else
                query = Session.Query<ClientOrgInfo>().Where(x => (x.IsManager || x.IsFinManager) && x.ClientOrgActive).OrderBy(x => x.DisplayName);

            var result = query.CreateModels<IClient>();

            return result;
        }

        public void Disable(IClient client)
        {
            // when we disable a ClientOrg we might have to also disable the Client and/or disable physical access

            Provider.Data.ActiveLog.Disable("ClientOrg", client.ClientOrgID); // normal disable of ClientOrg

            // first check for other active ClientOrgs, this one won't be included because it was just disabled
            bool otherActive = Provider.Data.Client.ClientOrgs(client.ClientID).Any(x => x.ClientOrgActive);

            if (!otherActive)
                Provider.Data.ActiveLog.Disable("Client", client.ClientID);

            // be sure to check physical access after this
        }

        public IBillingType GetBillingType(int clientOrgId)
        {
            var logs = Session.Query<ClientOrgBillingTypeLog>().Where(x => x.ClientOrg.ClientOrgID == clientOrgId);

            ClientOrgBillingTypeLog active = logs.FirstOrDefault(x => x.DisableDate == null);

            if (active != null)
                return active.BillingType.CreateModel<IBillingType>();

            return logs.OrderBy(x => x.DisableDate).Last().BillingType.CreateModel<IBillingType>();
        }

        public int GetMaxChargeTypeID(int clientId)
        {
            var result = Session.Query<ClientOrg>()
                .Where(x => x.Client.ClientID == clientId).Max(x => (int?)x.Org.OrgType.ChargeType.ChargeTypeID) ?? 0;

            return result;
        }

        public IClient GetPrimary(int clientId)
        {
            var result = Session.Query<ClientOrgInfo>()
                .Where(x => x.ClientID == clientId)
                .OrderByDescending(x => x.ClientOrgActive)
                .ThenByDescending(x => x.PrimaryOrg)
                .ThenBy(x => x.ClientOrgID)
                .FirstOrDefault();

            return result.CreateModel<IClient>();
        }

        public IEnumerable<IClient> GetClientOrgs(int clientId)
        {
            var query = Session.Query<ClientOrgInfo>()
                .Where(x => x.ClientID == clientId)
                .OrderBy(x => x.EmailRank);

            return query.CreateModels<IClient>();
        }

        public IEnumerable<IClient> GetActiveClientOrgs(int clientId)
        {
            var query = Session.Query<ClientOrgInfo>()
                .Where(x => x.ClientID == clientId && x.ClientActive && x.ClientOrgActive)
                .OrderBy(x => x.EmailRank);

            return query.CreateModels<IClient>();
        }

        public IEnumerable<IClient> GetActiveClientOrgs(int clientId, DateTime sd, DateTime ed)
        {
            var query = Session.Query<ActiveLogClientOrg>()
                .Where(x => x.ClientID == clientId && x.EnableDate < ed && (x.DisableDate == null || x.DisableDate > sd));

            var join = query.Join(Session.Query<ClientOrgInfo>(), o => o.ClientOrgID, i => i.ClientOrgID, (o, i) => i);

            return join.OrderBy(x => x.EmailRank).CreateModels<IClient>();
        }

        public IEnumerable<IClient> SelectByClientAccount(int clientId, int accountId)
        {
            return Session.Query<ClientAccountInfo>().Where(x => x.ClientID == clientId && x.AccountID == accountId).CreateModels<IClient>();
        }

        public IEnumerable<IClient> SelectOrgManagers(int orgId = 0)
        {
            IEnumerable<IClient> result = null;

            if (orgId > 0)
                result = Session.Query<ClientOrgInfo>().Where(x => x.OrgID == orgId && x.ClientOrgActive && (x.IsManager || x.IsFinManager)).CreateModels<IClient>();
            else
                result = Session.Query<ClientOrgInfo>().Where(x => x.ClientOrgActive && (x.IsManager || x.IsFinManager)).CreateModels<IClient>();

            return result
                .OrderBy(x => x.LName)
                .ThenBy(x => x.FName)
                .ToList();
        }

        public IEnumerable<IPriv> GetPrivs()
        {
            return Session.Query<Priv>().CreateModels<IPriv>();
        }

        public IEnumerable<ICommunity> GetCommunities()
        {
            return Session.Query<Community>().CreateModels<ICommunity>();
        }

        public void InsertClientRemote(IClientRemote model, DateTime period)
        {
            var exists = Session.Query<ActiveLogClientRemote>().Any(x =>
                x.ClientID == model.ClientID
                && x.RemoteClientID == model.RemoteClientID
                && x.AccountID == model.AccountID
                && (x.EnableDate < period.AddMonths(1) && (x.DisableDate == null || x.DisableDate > period)));

            if (!exists)
            {
                var client = Session.Single<Client>(model.ClientID);
                var remoteClient = Session.Single<Client>(model.RemoteClientID);
                var account = Session.Single<Account>(model.AccountID);

                if (client == null)
                    throw new Exception(string.Format("Cannot find a Client with ClientID = {0}", model.ClientID));

                if (remoteClient == null)
                    throw new Exception(string.Format("Cannot find a Client with ClientID = {0}", model.RemoteClientID));

                if (account == null)
                    throw new Exception(string.Format("Cannot find an Account with AccountID = {0}", model.AccountID));

                //create a new ClientRemote
                var clientRemote = new ClientRemote()
                {
                    Client = client,
                    RemoteClient = remoteClient,
                    Account = account,
                    Active = true
                };

                Session.Insert(clientRemote);

                if (!client.Active)
                {
                    client.Active = true;
                    Provider.Data.ActiveLog.Enable("Client", client.ClientID);
                    ClearClientCache();
                }

                EnableClientRemote(clientRemote.ClientRemoteID, period);

                model.ClientRemoteID = clientRemote.ClientRemoteID;
            }
            else
            {
                throw new Exception("A duplicate record already exists.");
            }
        }

        public void DeleteClientRemote(int clientRemoteId, DateTime period)
        {
            ClientRemote cr = Session.Single<ClientRemote>(clientRemoteId);
            if (cr == null) throw new Exception($"Cannot find ClientRemote with ClientRemoteID = {clientRemoteId}");
            var alogs = Session.Query<ActiveLog>().Where(x => x.TableName == "ClientRemote" && x.Record == clientRemoteId);
            Session.Delete(cr);
            Session.Delete(alogs);
            IBillingType bt = Provider.Billing.BillingType.GetBillingType(cr.Client.ClientID, cr.Account.AccountID, period);
            Provider.Billing.Tool.UpdateBillingType(cr.Client.ClientID, cr.Account.AccountID, bt.BillingTypeID, period);
            LNF.Billing.RoomBillingUtility.UpdateBillingType(cr.Client, cr.Account, bt, period);
        }

        public IEnumerable<IClientRemote> GetActiveClientRemotes(DateTime sd, DateTime ed)
        {
            var query = Session.Query<ActiveLogClientRemote>()
                .Where(x => x.EnableDate < ed && (x.DisableDate == null || x.DisableDate > sd));

            var join = query.Join(Session.Query<ClientRemoteInfo>(), o => o.ClientRemoteID, i => i.ClientRemoteID, (o, i) => i);

            var result = join.CreateModels<IClientRemote>().OrderBy(x => x.DisplayName).ThenBy(x => x.AccountName).ToList();

            return result;
        }

        private void EnableClientRemote(int clientRemoteId, DateTime period)
        {
            ActiveLog alog = null;

            IList<ActiveLog> alogs = Session.Query<ActiveLog>().Where(x => x.TableName == "ClientRemote" && x.Record == clientRemoteId).ToList();

            if (alogs.Count > 0)
            {
                alog = alogs[0];

                // clean up invalid records, if any
                if (alogs.Count > 1)
                    Session.Delete(alogs.Skip(1).ToArray());
            }

            if (alog != null)
            {
                alog.EnableDate = period;
                alog.DisableDate = period.AddMonths(1);
            }
            else
            {
                alog = new ActiveLog()
                {
                    TableName = "ClientRemote",
                    Record = clientRemoteId,
                    EnableDate = period,
                    DisableDate = period.AddMonths(1)
                };
            }

            Session.SaveOrUpdate(alog);
        }

        private IClient SingleClient(int clientId)
        {
            IClient result;

            if (!_cache.Contains($"Client#{clientId}"))
            {
                result = Session.Single<ClientInfo>(clientId).CreateModel<IClient>();
                _cache.Add($"Client#{clientId}", result, DateTimeOffset.Now.AddMinutes(5));
                if (!_cache.Contains($"Client:{result.UserName}"))
                    _cache.Add($"Client:{result.UserName}", result.ClientID, ObjectCache.InfiniteAbsoluteExpiration);
            }
            else
            {
                result = (IClient)_cache[$"Client#{clientId}"];
            }

            return result;
        }

        private IClient SingleClient(string username)
        {
            IClient result;

            if (!_cache.Contains($"Client:{username}"))
            {
                result = Session.Query<ClientInfo>().FirstOrDefault(x => x.UserName == username).CreateModel<IClient>();
                _cache.Add($"Client#{result.ClientID}", result, DateTimeOffset.Now.AddMinutes(5));
                _cache.Add($"Client:{username}", result.ClientID, ObjectCache.InfiniteAbsoluteExpiration);
            }
            else
            {
                int clientId = (int)_cache[$"Client:{username}"];
                result = SingleClient(clientId);
            }

            return result;
        }

        private void ClearClientCache()
        {
            _cache.Remove("AllClients");
        }
    }
}
