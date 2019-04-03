using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public class ClientManager : ManagerBase, IClientManager
    {
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

        public IClient Find(string username)
        {
            return Session.Query<ClientInfo>().FirstOrDefault(x => x.UserName == username).CreateModel<IClient>();
        }

        public IEnumerable<IClient> FindByCommunity(int flag, bool? active = true)
        {
            IQueryable<ClientInfo> query;

            if (active.HasValue)
                query = Session.Query<ClientInfo>().Where(x => x.ClientActive == active.Value);
            else
                query = Session.Query<ClientInfo>();

            var result = query.Where(x => (x.Communities & flag) > 0);

            return result.CreateModels<IClient>();
        }

        public IClient FindByDisplayName(string displayName)
        {
            string[] splitter = displayName.Split(',');

            if (splitter.Length < 2)
                return null;

            string lname = splitter[0].Trim();
            string fname = splitter[1].Trim();

            var result = Session.Query<ClientInfo>().FirstOrDefault(x => x.LName == lname && x.FName == fname);

            return result.CreateModel<IClient>();
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
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> FindByTools(IEnumerable<int> resourceIds, bool? active = true)
        {
            throw new NotImplementedException();
        }

        public int GetActiveAccountCount(int clientId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> GetActiveClients()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> GetActiveClients(ClientPrivilege priv)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> GetActiveClients(DateTime sd, DateTime ed)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IClient> GetActiveClients(DateTime sd, DateTime ed, ClientPrivilege priv)
        {
            throw new NotImplementedException();
        }

        public IClient GetClientInfo(int clientId)
        {
            throw new NotImplementedException();
        }

        public IClient GetClientOrgInfo(int clientId, int rank)
        {
            throw new NotImplementedException();
        }

        public DateTime? LastReservation(int clientId)
        {
            throw new NotImplementedException();
        }

        public DateTime? LastRoomEntry(int clientId)
        {
            throw new NotImplementedException();
        }

        public IClient Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public IChargeType MaxChargeType(int clientId)
        {
            throw new NotImplementedException();
        }

        public IClient NewClient(string username, string password, string lname, string fname, ClientPrivilege privs, bool active)
        {
            throw new NotImplementedException();
        }

        public int SetPassword(int clientId, string password)
        {
            throw new NotImplementedException();
        }

        public IClient StoreClientInfo(ref int clientId, string lname, string fname, string mname, string username, IClientDemographics demographics, IEnumerable<IPriv> privs, IEnumerable<ICommunity> communities, int technicalFieldId, IOrg org, IRole role, IDepartment dept, string email, string phone, bool isManager, bool isFinManager, DateTime? subsidyStart, DateTime? newFacultyStart, IEnumerable<IAddress> addedAddress, IEnumerable<IAddress> deletedAddress, IEnumerable<IClientManager> clientManagers, IEnumerable<IClientAccount> clientAccounts, out string alert)
        {
            throw new NotImplementedException();
        }

        public ITechnicalField TechnicalField(int clientId)
        {
            throw new NotImplementedException();
        }

        public string TechnicalFieldName(int clientId)
        {
            throw new NotImplementedException();
        }

        public int TotalDaysInLab(int clientId, IRoom r, DateTime period)
        {
            throw new NotImplementedException();
        }

        public void Update(IClient client)
        {
            throw new NotImplementedException();
        }

        public bool UpdatePhysicalAccess(IClient client, out string alert)
        {
            throw new NotImplementedException();
        }

        //public void Update(Client client, ClientItem item)
        //{
        //    client.FName = item.FName;
        //    client.MName = item.MName;
        //    client.LName = item.LName;
        //    client.UserName = item.UserName;
        //    client.DemCitizenID = item.DemCitizenID;
        //    client.DemGenderID = item.DemGenderID;
        //    client.DemRaceID = item.DemRaceID;
        //    client.DemEthnicID = item.DemEthnicID;
        //    client.DemDisabilityID = item.DemDisabilityID;
        //    client.Privs = item.Privs;
        //    client.Communities = item.Communities;
        //    client.TechnicalFieldID = item.TechnicalInterestID;
        //    client.IsChecked = item.IsChecked;
        //    client.IsSafetyTest = item.IsSafetyTest;

        //    if (item.ClientActive)
        //        ActiveDataItemManager.Enable(client);
        //    else
        //        ActiveDataItemManager.Disable(client);
        //}

        //public ClientInfo GetClientInfo(Client client)
        //{
        //    ClientInfo result = Session.Single<ClientInfo>(client.ClientID);
        //    return result;
        //}

        //public string PrimaryEmail(Client client)
        //{
        //    ClientInfo c = GetClientInfo(client);
        //    return PrimaryEmail(c.CreateClientItem());
        //}

        //public string PrimaryEmail(ClientItem client)
        //{
        //    if (client == null) return string.Empty;
        //    return client.Email;
        //}

        //public string PrimaryPhone(Client client)
        //{
        //    ClientInfo c = GetClientInfo(client);
        //    if (c == null) return string.Empty;
        //    return c.Phone;
        //}

        //public Org PrimaryOrg(Client client)
        //{
        //    ClientInfo c = GetClientInfo(client);
        //    if (c == null) return null;
        //    return Session.Single<Org>(c.OrgID);
        //}

        //public ClientOrg PrimaryClientOrg(Client client)
        //{
        //    ClientInfo c = GetClientInfo(client);
        //    if (c == null) return null;
        //    return Session.Single<ClientOrg>(c.ClientOrgID);
        //}

        //public ChargeType MaxChargeType(Client client)
        //{
        //    var result = ActiveClientOrgs(client)
        //        .Select(x => x.Org.OrgType.ChargeType)
        //        .OrderBy(x => x.ChargeTypeID)
        //        .LastOrDefault();

        //    return result;
        //}

        //public TechnicalField TechnicalField(Client client)
        //{
        //    return Session.Single<TechnicalField>(client.TechnicalFieldID);
        //}

        //public string TechnicalFieldName(Client client)
        //{
        //    var tf = TechnicalField(client);

        //    if (tf != null)
        //        return tf.TechnicalFieldName;
        //    else
        //        return string.Empty;
        //}

        //public DateTime? LastReservation(Client client)
        //{
        //    return Session.Query<Reservation>()
        //        .Where(x => x.Client == client && x.IsActive && x.IsStarted)
        //        .Max(x => x.ActualBeginDateTime);
        //}

        //public DateTime? LastRoomEntry(Client client)
        //{
        //    DateTime? result = Session.Query<RoomDataClean>().Where(x => x.ClientID == client.ClientID).Max(x => x.EntryDT);
        //    return result;
        //}

        //public ClientOrgInfo GetClientOrgInfo(Client client, int rank)
        //{
        //    ClientOrgInfo result = Session.Query<ClientOrgInfo>().Where(x => x.ClientID == client.ClientID && x.EmailRank == rank).FirstOrDefault();
        //    return result;
        //}

        //public int TotalDaysInLab(Client client, Room r, DateTime period)
        //{
        //    IList<RoomData> query = Session.Query<RoomData>().Where(x => x.ClientID == client.ClientID && x.RoomID == r.RoomID && x.Period == period).ToList();
        //    IEnumerable<DateTime> days = query.Select(x => x.EvtDate);
        //    IEnumerable<int> distinctDays = days.Select(x => x.Day).Distinct();
        //    int result = distinctDays.Count();
        //    return result;
        //}

        ///// <summary>
        ///// Find Clients that were active during the period, filtered on the current user status. Administrators see all users, Staff see themselves only unless displayAllUsersToStaff is true, Managers see themselves and any users they manage, Normal users see only themselves.
        ///// </summary>
        ///// <param name="client">The current user</param>
        ///// <param name="period">The period during which clients must be active</param>
        ///// <param name="displayAllUsersToStaff">When true all users are displayed if the current user is staff</param>
        ///// <returns>A list of active clients during the period, filtered on the current user</returns>
        //public IEnumerable<Client> FindByPeriod(Client client, DateTime period, bool displayAllUsersToStaff = false)
        //{

        //}

        //public IEnumerable<Client> FindByTools(IEnumerable<int> resourceIds, bool? active = true)
        //{
        //    List<Client> result = new List<Client>();
        //    List<ResourceClient> rclist = new List<ResourceClient>();

        //    foreach (int id in resourceIds)
        //        rclist.AddRange(Session.Query<ResourceClient>().Where(x => x.ClientID > 0 && x.Resource.ResourceID == id && (x.Expiration == null || x.Expiration.Value < DateTime.Now)));

        //    if (active.HasValue)
        //        result.AddRange(rclist.Select(x => Session.Single<Client>(x.ClientID)).Where(x => x.Active == active.Value));
        //    else
        //        result.AddRange(rclist.Select(x => Session.Single<Client>(x.ClientID)));

        //    return result;
        //}

        //public IEnumerable<Client> FindByPrivilege(ClientPrivilege priv, bool? active = true)
        //{
        //    IQueryable<Client> query;

        //    if (active.HasValue)
        //        query = Session.Query<Client>().Where(x => x.Active == active.Value);
        //    else
        //        query = Session.Query<Client>();

        //    IList<Client> result = query.Where(x => (x.Privs & priv) > 0).ToList();

        //    return result;
        //}

        //public Client NewClient(string username, string password, string lname, string fname, ClientPrivilege privs, bool active)
        //{
        //    var result = new Client()
        //    {
        //        UserName = username,
        //        LName = lname,
        //        FName = fname,
        //        Privs = privs
        //    };

        //    Session.Insert(result);

        //    SetPassword(result.ClientID, password);

        //    if (active)
        //        ActiveDataItemManager.Enable(result);
        //    else
        //        ActiveDataItemManager.Disable(result);

        //    return result;
        //}

        ///// <summary>
        ///// Sets the user's password.
        ///// </summary>
        //public int SetPassword(int clientId, string password)
        //{
        //    var pw = Provider.Encryption.EncryptText(password);
        //    var hash = Provider.Encryption.Hash(password);

        //    int result = Session.NamedQuery("SetPassword")
        //        .SetParameters(new { ClientID = clientId, Password = pw, PasswordHash = hash })
        //        .Result<int>();

        //    return result;
        //}

        //public Client StoreClientInfo(ref int clientId, string lname, string fname, string mname, string username, ClientDemographics demographics, IEnumerable<Priv> privs, IEnumerable<Community> communities, int technicalFieldId, Org org, Role role, Department dept, string email, string phone, bool isManager, bool isFinManager, DateTime? subsidyStart, DateTime? newFacultyStart, IEnumerable<Address> addedAddress, IEnumerable<Address> deletedAddress, IEnumerable<Repository.Data.ClientManager> clientManagers, IEnumerable<ClientAccount> clientAccounts, out string alert)
        //{
        //    //add rows to Client, ClientSite and ClientOrg for new entries

        //    var c = Session.Single<Client>(clientId);

        //    bool isNewClientEntry = c == null;

        //    if (isNewClientEntry)
        //    {
        //        //add an entry to the client table
        //        c = NewClient(username, username, lname, fname, PrivUtility.CalculatePriv(privs), true);
        //        clientId = c.ClientID;
        //    }
        //    else
        //    {
        //        c.UserName = username;
        //        c.FName = fname;
        //        c.LName = lname;

        //        //store Privs's
        //        c.Privs = PrivUtility.CalculatePriv(privs);

        //        SetPassword(c.ClientID, c.UserName);
        //    }

        //    c.MName = CleanMiddleName(mname);
        //    demographics.Update(c);

        //    //store NULL for non-entered usertype
        //    c.Communities = CommunityUtility.CalculateFlag(communities);
        //    c.TechnicalFieldID = technicalFieldId;

        //    //next the ClientOrg table
        //    ClientOrg co = Session.Query<ClientOrg>().FirstOrDefault(x => x.Client == c && x.Org == org);

        //    if (co == null)
        //    {
        //        // need new row in ClientOrg
        //        co = new ClientOrg
        //        {
        //            Client = c,
        //            Org = org,
        //            ClientAddressID = 0
        //        };

        //        Session.Insert(co);
        //    }

        //    co.Role = role;
        //    co.Department = dept;
        //    co.Email = email;
        //    co.Phone = phone;
        //    co.IsManager = isManager;
        //    co.IsFinManager = isFinManager;
        //    co.SubsidyStartDate = subsidyStart;
        //    co.NewFacultyStartDate = newFacultyStart;

        //    ActiveDataItemManager.Enable(co);

        //    //find any address that need to be dealt with
        //    if (addedAddress != null && addedAddress.Count() > 0)
        //    {
        //        foreach (Address aa in addedAddress)
        //        {
        //            co.ClientAddressID = aa.AddressID;
        //        }
        //    }

        //    if (deletedAddress != null && deletedAddress.Count() > 0)
        //    {
        //        co.ClientAddressID = 0;
        //        Session.Delete(deletedAddress);
        //    }

        //    //update rows in ClientManager as needed
        //    foreach (Repository.Data.ClientManager cm in clientManagers)
        //    {
        //        cm.ClientOrg = co;
        //    }

        //    //update rows in ClientAccount as needed
        //    foreach (ClientAccount ca in clientAccounts)
        //    {
        //        ca.ClientOrg = co;
        //    }

        //    // For clients who have Lab User priv only, only allow access if they
        //    // have an active account. If access is not enabled, show an alert.
        //    AccessCheck check = AccessCheck.Create(c.Model<ClientItem>(), Provider);
        //    UpdatePhysicalAccess(check, out alert);

        //    return c;
        //}

        //public bool UpdatePhysicalAccess(AccessCheck check, out string alert)
        //{
        //    bool result;

        //    if (check.CanEnableAccess()) //access should be enabled per privs and accounts
        //    {
        //        if (!check.IsPhysicalAccessEnabled()) //does not already have physical access
        //        {
        //            if (check.AllowReenable()) //access can be enabled based on old account rule
        //            {
        //                check.EnablePhysicalAccess();
        //                alert = string.Empty;
        //                result = true;
        //            }
        //            else
        //            {
        //                //if client has been disabled for a "long time", do not enable access and alert user
        //                check.RemovePhysicalAccessPriv();
        //                alert = "Note that this client has been inactive for so long that access is not automatically reenabled. The Physical Access privilege has been removed.";
        //                result = false;
        //            }
        //        }
        //        else
        //        {
        //            // can enable access and physical access already enabled
        //            alert = string.Empty;
        //            result = true;
        //        }
        //    }
        //    else //access should not be enabled per privs and accounts
        //    {
        //        string temp = string.Empty;
        //        string reason = check.Reason;

        //        if (check.IsPhysicalAccessEnabled())
        //        {
        //            check.DisablePhysicalAccess();
        //            temp += reason + " Physical access has been disabled.";
        //            reason = string.Empty;
        //        }

        //        // remove PhysicalAccess priv
        //        if (check.HasPhysicalAccessPriv)
        //        {
        //            check.RemovePhysicalAccessPriv();
        //            temp = reason + " The Physical Access privilege has been removed.";
        //            reason = string.Empty;
        //        }

        //        // remove StoreUser priv if LabUser and no ActiveAccounts
        //        if (check.HasLabUserPriv && check.HasStoreUserPriv && !check.HasActiveAccounts)
        //        {
        //            check.RemoveStoreUserPriv();
        //            temp += reason + " The Store User privilege has been removed.";
        //            reason = string.Empty;
        //        }

        //        alert = temp.Trim();

        //        // alert will be empty if nothing changed (privs removed or access disabled)

        //        result = false;
        //    }

        //    // alert should be an empty string unless access is not enabled (result = false)
        //    // and something changed (privs removed or access disabled)

        //    return result;
        //}

        //public ClientItem Login(string username, string password)
        //{
        //    if (string.IsNullOrEmpty(password))
        //        return null;

        //    var client = ClientInfo.Find(username);

        //    if (client == null)
        //        throw new Exception("Invalid username.");

        //    if (!string.IsNullOrEmpty(Provider.DataAccess.UniversalPassword) && password == Provider.DataAccess.UniversalPassword)
        //        return client.CreateClientItem();

        //    if (CheckPassword(client.ClientID, password))
        //        return client.CreateClientItem();
        //    else
        //        throw new Exception("Invalid password.");
        //}

        ///// <summary>
        ///// Returns the number of currently active accounts for a given client.
        ///// </summary>
        //public int GetActiveAccountCount(int clientId)
        //{
        //    int result = Session.Query<ActiveLogClientAccount>().Count(x => x.ClientID == clientId && !x.DisableDate.HasValue);
        //    return result;
        //}

        //public IEnumerable<Client> GetActiveClients()
        //{
        //    DateTime ed = DateTime.Now.AddDays(1);
        //    DateTime sd = ed.AddMonths(-1);
        //    return GetActiveClients(sd, ed);
        //}

        //public IEnumerable<Client> GetActiveClients(DateTime sd, DateTime ed)
        //{
        //    /*
        //     * ------------------------------------------------------------------------------------------
        //     * -- this method should return the same results as sselData.dbo.Client_Select @Action = 'All'
        //     * ------------------------------------------------------------------------------------------
        //     * 
        //     * 	IF @eDate IS NULL
        //     *  BEGIN
        //     *      IF @Action='NeedApportionment'
        //     *          SET @eDate = DATEADD(MM, 1, @sDate)
        //     *      ELSE
        //     *          SET @eDate = DATEADD(DD, 1, GETDATE())
        //     *  END
        //     *
        //     *  IF @sDate IS NULL
        //     *      SET @sDate = DATEADD(MM, -1, @eDate)

        //     *  SELECT DISTINCT c.ClientID, c.FName, c.MName, c.LName, c.UserName, c.[Password], c.PasswordHash, c.DemCitizenID, c.DemGenderID
        //     *      , c.DemRaceID, c.DemEthnicID, c.DemDisabilityID, c.Privs, c.Communities, c.TechnicalInterestID, c.Active, c.isChecked, c.isSafetyTest
        //     *      , c.LName + ', ' + c.FName AS DisplayName
        //     *  FROM Client c
        //     *  WHERE (c.Privs & @Privs > 0) AND c.ClientID IN
        //     *  (
        //     *      SELECT Record
        //     *      FROM ActiveLog
        //     *      WHERE TableName = 'Client' AND
        //     *        EnableDate < @eDate AND
        //     *        ((DisableDate IS NULL) OR (DisableDate > @sDate))
        //     *  )
        //     *  ORDER BY DisplayName 
        //     *  
        //     */

        //    var activeLogs = Session.Query<ActiveLog>().Where(x => x.TableName == "Client" && x.EnableDate < ed && (!x.DisableDate.HasValue || x.DisableDate.Value > sd));

        //    var join = activeLogs
        //        .Join(Session.Query<Client>(), o => o.Record, i => i.ClientID, (o, i) => new { ActiveLog = o, Client = i });

        //    return join.Select(x => x.Client).ToList();
        //}

        //public IEnumerable<Client> GetActiveClients(ClientPrivilege priv)
        //{
        //    DateTime ed = DateTime.Now.AddDays(1);
        //    DateTime sd = ed.AddMonths(-1);
        //    return GetActiveClients(sd, ed, priv);
        //}

        //public IQueryable<Client> GetActiveClients(DateTime sd, DateTime ed, ClientPrivilege priv)
        //{
        //    var activeLogs = Session.Query<ActiveLog>().Where(x => x.TableName == "Client" && x.EnableDate < ed && (!x.DisableDate.HasValue || x.DisableDate.Value > sd));

        //    var join = activeLogs
        //        .Join(Session.Query<Client>(), o => o.Record, i => i.ClientID, (o, i) => new { ActiveLog = o, Client = i });

        //    return join.Where(x => (x.Client.Privs & priv) > 0).Select(x => x.Client);
        //}
    }
}
