using LNF.Billing;
using LNF.Cache;
using LNF.Data;
using LNF.Impl.DataAccess;
using LNF.Impl.Mail;
using LNF.Impl.Repository;
using LNF.Impl.Repository.Billing;
using LNF.Impl.Repository.Data;
using LNF.Impl.Repository.Scheduler;
using LNF.PhysicalAccess;
using LNF.Util.Encryption;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Caching;

namespace LNF.Impl.Data
{
    public class ClientRepository : RepositoryBase, IClientRepository
    {
        public IEncryptionUtility Encryption { get; }
        public IPhysicalAccessService PhysicalAccess { get; }
        public IBillingTypeRepository BillingType { get; }
        public IToolBillingRepository ToolBilling { get; }
        public ICostRepository Cost { get; set; }

        public ClientRepository(ISessionManager mgr, IEncryptionUtility encryption, IPhysicalAccessService physicalAccess, IBillingTypeRepository billingType, IToolBillingRepository toolBilling, ICostRepository cost) : base(mgr)
        {
            Encryption = encryption;
            PhysicalAccess = physicalAccess;
            BillingType = billingType;
            ToolBilling = toolBilling;
            Cost = cost;
        }

        public IEnumerable<IClientAccount> GetClientAccounts()
        {
            return Session.Query<ClientAccountInfo>().CreateModels<IClientAccount>();
        }

        public IEnumerable<IClientAccount> GetClientAccounts(int clientId)
        {
            return Session.Query<ClientAccountInfo>().Where(x => x.ClientID == clientId).CreateModels<IClientAccount>();
        }

        public IEnumerable<IClientAccount> GetClientAccounts(IClient client)
        {
            return Session.Query<ClientAccountInfo>().Where(x => x.ClientOrgID == client.ClientOrgID).CreateModels<IClientAccount>();
        }

        public IEnumerable<IClientAccount> GetClientAccounts(int clientId, int[] accountIds)
        {
            return Session.Query<ClientAccountInfo>().Where(x => x.ClientID == clientId && accountIds.Contains(x.AccountID)).CreateModels<IClientAccount>();
        }

        public IEnumerable<IClientAccount> GetActiveClientAccounts()
        {
            var result = Session.Query<ClientAccountInfo>()
                .Where(x => x.ClientActive && x.ClientAccountActive && x.ClientOrgActive)
                .ToList();
            return result;
        }

        public IEnumerable<IClientAccount> GetActiveClientAccounts(int clientId)
        {
            var result = Session.Query<ClientAccountInfo>()
                .Where(x => x.ClientID == clientId && x.ClientActive && x.ClientAccountActive && x.ClientOrgActive)
                .ToList();
            return result;
        }

        public IEnumerable<IClientAccount> GetActiveClientAccounts(int[] clientIds)
        {
            var result = Session.Query<ClientAccountInfo>()
                .Where(x => clientIds.Contains(x.ClientID) && x.ClientActive && x.ClientAccountActive && x.ClientOrgActive)
                .ToList();
            return result;
        }

        public IEnumerable<IClientAccount> GetActiveClientAccounts(int clientId, DateTime sd, DateTime ed)
        {
            var query = Session.Query<ActiveLog>().Where(x => x.TableName == "ClientAccount" && (x.EnableDate < ed && (x.DisableDate == null || x.DisableDate > sd)));
            var join = query.Join(Session.Query<ClientAccountInfo>(), o => o.Record, i => i.ClientAccountID, (outer, inner) => inner);
            var result = join.Where(x => x.ClientID == clientId).ToList();
            return result;
        }

        public IEnumerable<IClientAccount> GetActiveClientAccounts(string username)
        {
            var result = Session.Query<ClientAccountInfo>()
                .Where(x => x.UserName == username && x.ClientActive && x.ClientAccountActive && x.ClientOrgActive)
                .ToList();
            return result;
        }

        public IEnumerable<IClientAccount> GetActiveClientAccounts(string username, DateTime sd, DateTime ed)
        {
            var query = Session.Query<ActiveLog>().Where(x => x.TableName == "ClientAccount" && (x.EnableDate < ed && (x.DisableDate == null || x.DisableDate > sd)));
            var join = query.Join(Session.Query<ClientAccountInfo>(), o => o.Record, i => i.ClientAccountID, (outer, inner) => inner);
            var result = join.Where(x => x.UserName == username).ToList();
            return result;
        }

        public IEnumerable<IClientAccountAssignment> GetClientAccountAssignments(int managerOrgId)
        {
            return Session.Query<ClientAccountAssignment>().Where(x => x.ManagerOrgID == managerOrgId).ToList();
        }

        //public IEnumerable<IAccount> ActiveAccounts(int clientId, DateTime sd, DateTime ed)
        //{
        //    var accountIds = ActiveClientAccounts(clientId, sd, ed).Select(x => x.AccountID).Distinct().ToArray();
        //    var result = Session.Query<AccountInfo>().Where(x => accountIds.Contains(x.AccountID)).CreateModels<IAccount>();
        //    return result;
        //}

        //public IEnumerable<IAccount> ActiveAccounts(int clientId)
        //{
        //    return ClientAccounts(clientId).Where(x => x.ClientActive && x.ClientOrgActive && x.AccountActive);
        //}

        public IEnumerable<IClient> GetActiveClientOrgs()
        {
            var query = Session.Query<ClientOrgInfo>()
                .Where(x => x.ClientActive && x.ClientOrgActive)
                .OrderBy(x => x.ClientID)
                .ThenBy(x => x.EmailRank);

            return query.ToList();
        }

        public IEnumerable<IClient> GetActiveClientOrgs(int clientId)
        {
            var query = Session.Query<ClientOrgInfo>()
                .Where(x => x.ClientID == clientId && x.ClientActive && x.ClientOrgActive)
                .OrderBy(x => x.EmailRank);

            return query.ToList();
        }

        public IEnumerable<IClient> GetActiveClientOrgs(int clientId, DateTime sd, DateTime ed)
        {
            return ActiveClientOrgQuery(sd, ed).Where(x => x.ClientID == clientId).ToList();
        }

        public IEnumerable<IClient> GetActiveClientOrgs(DateTime sd, DateTime ed)
        {
            return ActiveClientOrgQuery(sd, ed).ToList();
        }

        public string[] ActiveEmails(int clientId)
        {
            //this function returns the same result as sselData.dbo.udf_ClientEmails()
            return Session.Query<ClientOrgInfo>().Where(x => x.ClientID == clientId && x.ClientOrgActive).Select(x => x.Email).Distinct().ToArray();
        }

        public bool CheckPassword(int clientId, string password)
        {
            if (password == Configuration.Current.DataAccess.UniversalPassword)
                return true;

            DataTable dt = Session.Command(CommandType.Text).Param("ClientID", clientId).FillDataTable("SELECT ClientID, Password, PasswordHash FROM sselData.dbo.Client WHERE ClientID = @ClientID");

            if (dt.Rows.Count == 0)
                throw new ItemNotFoundException("Client", "ClientID", clientId);

            var pw = dt.Rows[0].Field<string>("Password");
            var salt = dt.Rows[0].Field<string>("PasswordHash");

            var encrypted = Encryption.EncryptText(password + salt);

            var result = pw == encrypted;

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

        public IEnumerable<IClient> FindByCommunity(int flag, bool? active = true)
        {
            if (active.HasValue)
                return Session.Query<ClientInfo>().Where(x => x.ClientActive == active.Value && (x.Communities & flag) > 0).ToList();
            else
                return Session.Query<ClientInfo>().Where(x => (x.Communities & flag) > 0).ToList();
        }

        public IClient FindByDisplayName(string displayName)
        {
            string[] splitter = displayName.Split(',');

            if (splitter.Length < 2)
                return null;

            string lname = splitter[0].Trim();
            string fname = splitter[1].Trim();

            var result = Session.Query<ClientInfo>().FirstOrDefault(x => x.LName == lname && x.FName == fname);

            return result;
        }

        public IEnumerable<IClient> FindByManager(int managerClientId, bool? active = true)
        {
            List<IClient> result = new List<IClient>();
            List<ClientManager> cmlist = new List<ClientManager>();
            IList<ClientOrgInfo> mgrClientOrgs;

            if (active.HasValue)
            {
                mgrClientOrgs = Session.Query<ClientOrgInfo>().Where(x => x.ClientID == managerClientId && (x.IsFinManager || x.IsManager)).ToList();

                foreach (ClientOrgInfo co in mgrClientOrgs)
                    cmlist.AddRange(Session.Query<ClientManager>().Where(x => x.ManagerOrg.ClientOrgID == co.ClientOrgID && x.Active == active.Value));

                result.AddRange(cmlist.AsQueryable().CreateModels<IClient>());
            }
            else
            {
                mgrClientOrgs = Session.Query<ClientOrgInfo>().Where(x => x.ClientID == managerClientId && (x.IsFinManager || x.IsManager)).ToList();
                foreach (ClientOrgInfo co in mgrClientOrgs)
                    cmlist.AddRange(Session.Query<ClientManager>().Where(x => x.ManagerOrg.ClientOrgID == co.ClientOrgID));
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

            var result = Session.Query<ClientInfo>().Where(x => clientIds.Contains(x.ClientID)).ToList();

            return result;
        }

        public IEnumerable<IClient> FindByPrivilege(ClientPrivilege priv, bool? active = true)
        {
            if (active.HasValue)
                return Session.Query<ClientInfo>().Where(x => x.ClientActive == active.Value && (x.Privs & priv) > 0).ToList();
            else
                return Session.Query<ClientInfo>().Where(x => (x.Privs & priv) > 0).ToList();
        }

        public IEnumerable<IClient> FindByTools(int[] resourceIds, bool? active = true)
        {
            var query = Session.Query<ResourceClientInfo>().Where(x => x.ClientID > 0 && resourceIds.Contains(x.ResourceID) && (x.Expiration == null || x.Expiration.Value < DateTime.Now)).ToList();
            var join = query.Join(Session.Query<ClientInfo>(), o => o.ClientID, i => i.ClientID, (o, i) => i);
            var result = join.Where(x => x.ClientActive == active.GetValueOrDefault(x.ClientActive));
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

        public IEnumerable<IClient> GetClients()
        {
            return Session.Query<ClientInfo>().ToList();
        }

        public IEnumerable<IClient> GetClients(int limit, int skip = 0)
        {
            if (limit > 100)
                throw new ArgumentOutOfRangeException("The parameter 'limit' must not be greater than 100.");

            var result = Session.Query<ClientInfo>().Skip(skip).Take(limit).OrderBy(x => x.DisplayName).ToList();

            return result;
        }

        public IEnumerable<IClient> GetClients(int[] ids)
        {
            return Session.Query<ClientInfo>().Where(x => ids.Contains(x.ClientID)).ToList();
        }

        public IEnumerable<IClient> GetActiveClients()
        {
            return Session.Query<ClientInfo>().Where(x => x.ClientActive).ToList();
        }

        public IEnumerable<IClient> GetActiveClients(ClientPrivilege priv = 0)
        {
            return GetActiveClients().Where(x => priv == 0 || (x.Privs & priv) > 0).ToList();
        }

        public IEnumerable<IClient> GetActiveClients(DateTime sd, DateTime ed, ClientPrivilege priv = 0)
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

            var activeLogs = Session.Query<ActiveLog>().Where(x => x.TableName == "Client" && x.EnableDate < ed && (!x.DisableDate.HasValue || x.DisableDate.Value > sd));
            var join = activeLogs.Join(Session.Query<ClientInfo>(), o => o.Record, i => i.ClientID, (o, i) => i);
            return join.Where(x => priv == 0 || (x.Privs & priv) > 0).ToList();
        }

        public IClient GetClient(string username) => SingleClient(username);

        public IClient GetClient(int clientId) => SingleClient(clientId);

        public IClient GetClient(int clientId, int rank)
        {
            var result = Session.Query<ClientOrgInfo>().FirstOrDefault(x => x.ClientID == clientId && x.EmailRank == rank);
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

            var client = Session.FindClientInfo(username);

            if (client == null)
                throw new Exception("Invalid username.");

            if (!string.IsNullOrEmpty(Configuration.Current.DataAccess.UniversalPassword) && password == Configuration.Current.DataAccess.UniversalPassword)
                return client;

            if (CheckPassword(client.ClientID, password))
                return client;
            else
                throw new Exception("Invalid password.");
        }

        public IChargeType MaxChargeType(int clientId)
        {
            var client = GetActiveClientOrgs(clientId)
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
            var salt = Guid.NewGuid().ToString("n");
            var pw = Encryption.EncryptText(password + salt);

            var sql = "UPDATE sselData.dbo.Client SET Password = @Password, PasswordHash = @PasswordHash WHERE ClientID = @ClientID";

            int result = Session.Command(CommandType.Text)
                .Param("ClientID", clientId)
                .Param("Password", pw)
                .Param("PasswordHash", salt)
                .ExecuteNonQuery(sql).Value;

            return result;
        }

        public IClient StoreClientInfo(ref int clientId, string lname, string fname, string mname, string username, IClientDemographics demographics, IEnumerable<IPriv> privs, IEnumerable<ICommunity> communities, int technicalInterestId, int orgId, int roleId, int deptId, string email, string phone, bool isManager, bool isFinManager, DateTime? subsidyStart, DateTime? newFacultyStart, int[] addedAddressIds, int[] deletedAddressIds, int[] clientManagerIds, int[] clientAccountIds, out string alert)
        {
            //add rows to Client, ClientSite and ClientOrg for new entries

            var client = Session.Get<Client>(clientId);
            //IClient c;

            bool isNewClientEntry = (client == null);

            if (isNewClientEntry)
            {
                //add an entry to the client table
                client = NewClient(username, username, lname, fname, PrivUtility.CalculatePriv(privs), true);
                clientId = client.ClientID;
            }
            else
            {
                client.UserName = username;
                client.FName = fname;
                client.LName = lname;

                //store Privs's
                client.Privs = PrivUtility.CalculatePriv(privs);

                SetPassword(client.ClientID, client.UserName);
            }

            client.MName = CleanMiddleName(mname);
            UpdateClientDemographics(demographics);

            //store NULL for non-entered usertype
            client.Communities = CommunityUtility.CalculateFlag(communities);
            client.TechnicalInterestID = technicalInterestId;

            //next the ClientOrg table
            ClientOrg co = Session.Query<ClientOrg>().FirstOrDefault(x => x.Client == client && x.Org.OrgID == orgId);

            if (co == null)
            {
                // need new row in ClientOrg
                co = new ClientOrg
                {
                    Client = client,
                    Org = Session.Get<Org>(orgId),
                    ClientAddressID = 0
                };

                Session.Save(co);
            }

            co.Role = Session.Get<Role>(roleId);
            co.Department = Session.Get<Department>(deptId);
            co.Email = email;
            co.Phone = phone;
            co.IsManager = isManager;
            co.IsFinManager = isFinManager;
            co.SubsidyStartDate = subsidyStart;
            co.NewFacultyStartDate = newFacultyStart;

            Session.Enable(co);

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
                Session.DeleteMany(addrs);
            }

            //update rows in ClientManager as needed
            var clientManagers = Session.Query<ClientManager>().Where(x => clientManagerIds.Contains(x.ClientManagerID));
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

            // For clients who have Lab User priv only, only allow access if they
            // have an active account. If access is not enabled, show an alert.
            var c = client.CreateModel<IClient>();
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
            var c = Session.Get<Client>(client.ClientID);

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
            c.TechnicalInterestID = client.TechnicalInterestID;
            c.IsChecked = client.IsChecked;
            c.IsSafetyTest = client.IsSafetyTest;

            if (client.ClientActive)
                Session.Enable(c);
            else
                Session.Disable(c);

            ClearClientCache();

            return true;
        }

        public bool UpdatePhysicalAccess(IClient client, out string alert)
        {
            bool result;

            var gc = Cost.GetActiveGlobalCost();
            var flag = GetActiveAccountCount(client.ClientID) > 0;
            var check = AccessCheck.Create(PhysicalAccess, client, gc, flag);

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

        public IClientDemographics GetClientDemographics(int clientId)
        {
            var client = Session.Get<Client>(clientId);

            if (client == null) return null;

            var citizen = Session.Get<DemCitizen>(client.DemCitizenID);
            var disability = Session.Get<DemDisability>(client.DemDisabilityID);
            var ethnic = Session.Get<DemEthnic>(client.DemEthnicID);
            var gender = Session.Get<DemGender>(client.DemGenderID);
            var race = Session.Get<DemRace>(client.DemRaceID);

            return new ClientDemographics
            {
                ClientID = client.ClientID,
                UserName = client.UserName,
                FName = client.FName,
                LName = client.LName,
                DemCitizenID = citizen.DemCitizenID,
                DemCitizenValue = citizen.DemCitizenValue,
                DemDisabilityID = disability.DemDisabilityID,
                DemDisabilityValue = disability.DemDisabilityValue,
                DemEthnicID = ethnic.DemEthnicID,
                DemEthnicValue = ethnic.DemEthnicValue,
                DemGenderID = gender.DemGenderID,
                DemGenderValue = gender.DemGenderValue,
                DemRaceID = race.DemRaceID,
                DemRaceValue = race.DemRaceValue
            };
        }

        public bool UpdateClientDemographics(IClientDemographics value)
        {
            var client = Session.Get<Client>(value.ClientID);

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

        public IEnumerable<GenericListItem> AllActiveManagers()
        {
            var groupEmailManager = new GroupEmailManager(Session);
            var dt = groupEmailManager.GetAllActiveManagers();

            var result = new List<GenericListItem>();

            foreach (DataRow dr in dt.Rows)
            {
                result.Add(new GenericListItem(dr.Field<int>("ClientID").ToString(), dr.Field<string>("DisplayName")));
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

            var result = query.ToList();

            return result;
        }

        public void Disable(IClient client)
        {
            // when we disable a ClientOrg we might have to also disable the Client and/or disable physical access

            Session.Disable(Session.Get<ClientOrg>(client.ClientOrgID)); // normal disable of ClientOrg

            // first check for other active ClientOrgs, this one won't be included because it was just disabled
            bool otherActive = GetClientOrgs(client.ClientID).Any(x => x.ClientOrgActive);

            // disable the Client if there are no other active ClientOrgs
            if (!otherActive)
                Session.Disable(Session.Get<Client>(client.ClientID));

            // be sure to check physical access after this
        }

        public IBillingType GetBillingType(int clientOrgId)
        {
            var logs = Session.Query<ClientOrgBillingTypeLog>().Where(x => x.ClientOrgID == clientOrgId);

            ClientOrgBillingTypeLog active = logs.FirstOrDefault(x => x.DisableDate == null);

            int billingTypeId;

            if (active != null)
                billingTypeId = active.BillingTypeID;
            else
                billingTypeId = logs.OrderBy(x => x.DisableDate).Last().BillingTypeID;

            return Require<BillingType>(billingTypeId);
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

            return result;
        }

        public IEnumerable<IClient> GetClientOrgs()
        {
            return Session.Query<ClientOrgInfo>().OrderBy(x => x.ClientID).ThenBy(x => x.EmailRank).ToList();
        }

        public IEnumerable<IClient> GetClientOrgs(int clientId)
        {
            var query = Session.Query<ClientOrgInfo>()
                .Where(x => x.ClientID == clientId)
                .OrderBy(x => x.EmailRank);

            return query.ToList();
        }

        public IEnumerable<IClient> SelectByClientAccount(int clientId, int accountId)
        {
            return Session.Query<ClientAccountInfo>().Where(x => x.ClientID == clientId && x.AccountID == accountId).ToList();
        }

        public IEnumerable<IClient> SelectOrgManagers(int orgId = 0)
        {
            IEnumerable<IClient> result = null;

            if (orgId > 0)
                result = Session.Query<ClientOrgInfo>().Where(x => x.OrgID == orgId && x.ClientOrgActive && (x.IsManager || x.IsFinManager)).ToList();
            else
                result = Session.Query<ClientOrgInfo>().Where(x => x.ClientOrgActive && (x.IsManager || x.IsFinManager)).ToList();

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
                var client = Session.Get<Client>(model.ClientID);
                var remoteClient = Session.Get<Client>(model.RemoteClientID);
                var account = Session.Get<Account>(model.AccountID);

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

                Session.Save(clientRemote);

                if (!client.Active)
                {
                    Session.Enable(client);
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
            ClientRemote cr = Session.Get<ClientRemote>(clientRemoteId);

            if (cr == null)
                throw new Exception($"Cannot find ClientRemote with ClientRemoteID = {clientRemoteId}");

            var alogs = Session.Query<ActiveLog>().Where(x => x.TableName == "ClientRemote" && x.Record == clientRemoteId);

            Session.Delete(cr);
            Session.DeleteMany(alogs);

            var bt = BillingType.GetBillingType(cr.Client.ClientID, cr.Account.AccountID, period);
            ToolBilling.UpdateBillingType(cr.Client.ClientID, cr.Account.AccountID, bt.BillingTypeID, period);
        }

        public IEnumerable<IClientRemote> GetActiveClientRemotes(DateTime sd, DateTime ed)
        {
            var query = Session.Query<ActiveLogClientRemote>()
                .Where(x => x.EnableDate < ed && (x.DisableDate == null || x.DisableDate > sd));

            var join = query.Join(Session.Query<ClientRemoteInfo>(), o => o.ClientRemoteID, i => i.ClientRemoteID, (o, i) => i);

            var result = join.CreateModels<IClientRemote>().OrderBy(x => x.DisplayName).ThenBy(x => x.AccountName).ToList();

            return result;
        }

        public IClientPreference GetClientPreference(int clientId, string appName)
        {
            var result = Session.Query<ClientPreference>().FirstOrDefault(x => x.ClientID == clientId && x.ApplicationName == appName);

            if (result == null)
            {
                result = new ClientPreference { ClientID = clientId, ApplicationName = appName };
                Session.Save(result);
            }

            return result;
        }

        public IStaffDirectory GetStaffDirectory(string userName)
        {
            return Session.Query<StaffDirectory>().FirstOrDefault(x => x.Client.UserName == userName).CreateModel<IStaffDirectory>();
        }

        public IEnumerable<IClientManager> GetClientManagersByManager(int managerOrgId)
        {
            return Session.Query<ClientManager>().Where(x => x.ManagerOrg.ClientOrgID == managerOrgId).CreateModels<IClientManager>();
        }

        public IEnumerable<IClientManager> GetClientManagersByManaged(int clientOrgId)
        {
            return Session.Query<ClientManager>().Where(x => x.ClientOrg.ClientOrgID == clientOrgId).CreateModels<IClientManager>();
        }

        public IEnumerable<IAccount> GetActiveAccounts(int clientId)
        {
            AccountInfo acct = null;
            ClientAccount ca = null;
            ClientOrg co = null;

            var result = Session.QueryOver(() => acct)
                .JoinEntityAlias(() => ca, () => ca.Account.AccountID == acct.AccountID)
                .JoinEntityAlias(() => co, () => co.ClientOrgID == ca.ClientOrg.ClientOrgID)
                .Where(() => co.Client.ClientID == clientId && acct.AccountActive && ca.Active)
                .List();

            return result;
        }

        public IEnumerable<IAccount> GetActiveAccounts(int clientId, DateTime sd, DateTime ed)
        {
            AccountInfo acct = null;
            ClientAccount ca = null;
            ClientOrg co = null;
            ActiveLog alog1 = null;
            ActiveLog alog2 = null;

            var result = Session.QueryOver(() => acct)
                .JoinEntityAlias(() => ca, () => ca.Account.AccountID == acct.AccountID)
                .JoinEntityAlias(() => co, () => co.ClientOrgID == ca.ClientOrg.ClientOrgID)
                .JoinEntityAlias(() => alog1, () => alog1.Record == ca.ClientAccountID && alog1.TableName == "ClientAccount")
                .JoinEntityAlias(() => alog2, () => alog2.Record == ca.ClientOrg.ClientOrgID && alog2.TableName == "ClientOrg")
                .Where(() =>
                    (co.Client.ClientID == clientId)
                    && (alog1.EnableDate < ed && (alog1.DisableDate == null || alog1.DisableDate > sd))
                    && (alog2.EnableDate < ed && (alog2.DisableDate == null || alog2.DisableDate > sd)))
                .List();

            return result;
        }

        public IMessengerMessage CreateMessage(int clientId, string subject, string body, int parentId, bool disableReply, bool exclusive, bool acknowledgeRequired, bool blockAccess, int accessCutoff)
        {
            MessengerMessage mm = new MessengerMessage
            {
                ClientID = clientId,
                Subject = subject,
                Body = body,
                ParentID = parentId,
                Created = DateTime.Now,
                Status = "Draft",
                DisableReply = disableReply,
                Exclusive = exclusive,
                AcknowledgeRequired = acknowledgeRequired,
                BlockAccess = blockAccess,
                AccessCutoff = accessCutoff
            };

            Session.Save(mm);

            return mm;
        }

        public void SendMessage(int messageId, int[] recipients)
        {
            var message = Require<MessengerMessage>(messageId);
            message.Sent = DateTime.Now;
            message.Status = "Sent";
            Session.Update(message);
            foreach (int cid in recipients)
            {
                MessengerRecipient mr = new MessengerRecipient
                {
                    ClientID = cid,
                    MessageID = message.MessageID,
                    Folder = "Inbox",
                    Received = DateTime.Now,
                    Acknowledged = null,
                    AccessCount = 0
                };
                Session.Save(mr);
            }
        }

        public IEnumerable<IMessengerRecipient> GetMessages(int clientId, string folder)
        {
            return Session.Query<MessengerRecipient>().Where(x => x.ClientID == clientId && x.Folder == folder && x.Acknowledged == null).ToList();
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

            Session.Save(result);

            SetPassword(result.ClientID, password);

            if (active)
                Session.Enable(result);
            else
                Session.Disable(result);

            ClearClientCache();

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
                    Session.DeleteMany(alogs.Skip(1));
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

            if (!CacheManager.Current.Contains($"Client#{clientId}"))
            {
                result = Session.Get<ClientInfo>(clientId);
                if (result != null)
                {
                    CacheManager.Current.SetValue($"Client#{clientId}", result, DateTimeOffset.Now.AddMinutes(5));
                    if (!CacheManager.Current.Contains($"Client:{result.UserName}"))
                        CacheManager.Current.SetValue($"Client:{result.UserName}", result.ClientID, ObjectCache.InfiniteAbsoluteExpiration);
                }
            }
            else
            {
                result = (IClient)CacheManager.Current[$"Client#{clientId}"];
            }

            return result;
        }

        private IClient SingleClient(string username)
        {
            IClient result;

            if (!CacheManager.Current.Contains($"Client:{username}"))
            {
                result = Session.Query<ClientInfo>().FirstOrDefault(x => x.UserName == username);
                if (result != null)
                {
                    CacheManager.Current.SetValue($"Client#{result.ClientID}", result, DateTimeOffset.Now.AddMinutes(5));
                    CacheManager.Current.SetValue($"Client:{username}", result.ClientID, ObjectCache.InfiniteAbsoluteExpiration);
                }
            }
            else
            {
                int clientId = (int)CacheManager.Current[$"Client:{username}"];
                result = SingleClient(clientId);
            }

            return result;
        }

        private void ClearClientCache()
        {
            CacheManager.Current.RemoveValue("AllClients");
        }

        private IQueryable<ClientOrgInfo> ActiveClientOrgQuery(DateTime sd, DateTime ed)
        {
            var query = Session.Query<ActiveLog>().Where(x => x.TableName == "ClientOrg" && (x.EnableDate < ed && (x.DisableDate == null || x.DisableDate > sd)));
            var join = query.Join(Session.Query<ClientOrgInfo>(), o => o.Record, i => i.ClientOrgID, (outer, inner) => inner);
            return join;
        }

        public bool GetRequirePasswordReset(int clientId)
        {
            return Require<Client>(clientId).RequirePasswordReset;
        }

        public IPasswordResetRequest GetPasswordResetRequest(string code)
        {
            return Session.Query<PasswordResetRequest>().FirstOrDefault(x => x.ResetCode == code);
        }

        public IPasswordResetRequest AddPasswordResetRequest(int clientId)
        {
            IPasswordResetRequest result = null;

            int maxAttempts = 10000;
            int counter = 0;

            while (result == null && counter < maxAttempts)
            {
                var code = NewPasswordResetCode(6);
                if (!Session.Query<PasswordResetRequest>().Any(x => x.ResetCode == code))
                {
                    result = new PasswordResetRequest
                    {
                        ClientID = clientId,
                        RequestDateTime = DateTime.Now,
                        ResetCode = code,
                        ResetDateTime = null
                    };
                }
                counter++;
            }

            if (result == null)
                throw new Exception($"Cannot find a unique ResetCode after {maxAttempts} attempts.");

            Session.Save(result);

            return result;
        }

        private static string NewPasswordResetCode(int length)
        {
            string allowed = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            int len = allowed.Length;
            var rnd = new Random();
            var result = string.Empty;

            for (var x = 0; x < length; x++)
            {
                result += allowed[rnd.Next(0, len)];
            }

            return result;
        }

        public void CompletePasswordReset(int clientId, string code)
        {
            var req = Session.Query<PasswordResetRequest>().First(x => x.ClientID == clientId && x.ResetCode == code);
            req.ResetDateTime = DateTime.Now;

            var client = Require<Client>(clientId);
            client.RequirePasswordReset = false;

            Session.Update(req);
            Session.Update(client);
        }

        public void SetRequirePasswordReset(int clientId, bool value)
        {
            var client = Require<Client>(clientId);
            client.RequirePasswordReset = value;
            Session.Update(client);
        }
    }
}
