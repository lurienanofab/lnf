using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Repository.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Data
{
    public static class ClientUtility
    {
        /// <summary>
        /// The current authenticated session client.
        /// </summary>
        [Obsolete("User ClientInfoUtility.Current instead.")]
        public static Client Current
        {
            get { return ClientInfoUtility.Current.GetClient(); }
        }

        public static Client Find(int clientId)
        {
            return DA.Current.Single<Client>(clientId);
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
        public static IList<Client> FindByPeriod(Client client, DateTime period, bool displayAllUsersToStaff = false)
        {
            //populate the user dropdown list

            DateTime sd = period;
            DateTime ed = sd.AddMonths(1);

            IList<ActiveLogClientAccount> allClientAccountsActiveInPeriod = DA.Current
                .Query<ActiveLogClientAccount>().Where(x => x.EnableDate < ed && (x.DisableDate == null || x.DisableDate > sd))
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

        public static IList<Client> FindByClientPrivilege(ClientPrivilege priv, bool? active = true)
        {
            IQueryable<Client> query;

            if (active.HasValue)
                query = DA.Current.Query<Client>().Where(x => x.Active == active.Value);
            else
                query = DA.Current.Query<Client>();

            IList<Client> result = query.Where(x => (x.Privs & priv) > 0).ToList();

            return result;
        }

        public static IList<Client> FindByCommunity(int flag, bool? active = true)
        {
            IQueryable<Client> query;

            if (active.HasValue)
                query = DA.Current.Query<Client>().Where(x => x.Active == active.Value);
            else
                query = DA.Current.Query<Client>();

            var result = query.Where(x => (x.Communities & flag) > 0);

            return result.ToList();
        }

        public static IList<Client> FindByTools(IEnumerable<int> resourceIds, bool? active = true)
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

        public static IList<Client> FindByManager(int managerClientId, bool? active = true)
        {
            List<Client> result = new List<Client>();
            List<ClientManager> cmlist = new List<ClientManager>();
            IList<ClientOrg> mgrClientOrgs;

            if (active.HasValue)
            {
                mgrClientOrgs = DA.Current.Query<ClientOrg>().Where(x => x.Client.ClientID == managerClientId && (x.IsFinManager || x.IsManager) && x.Active == active.Value).ToList();
                foreach (ClientOrg co in mgrClientOrgs)
                    cmlist.AddRange(DA.Current.Query<ClientManager>().Where(x => x.ManagerOrg == co && x.Active == active.Value));
                result.AddRange(cmlist.Where(x => x.ClientOrg.Active == active.Value && x.ClientOrg.Client.Active == active.Value).Select(x => x.ClientOrg.Client));
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

        public static Client NewClient(int id, string username, string password, string lname, string fname, ClientPrivilege privs, bool active)
        {
            var result = new Client()
            {
                ClientID = id,
                UserName = username,
                LName = lname,
                FName = fname,
                Privs = privs,
                Active = active
            };

            result.SetPassword(password);

            return result;
        }

        public static Client StoreClientInfo(ref int clientId, bool isNewClientEntry, bool addClient, string lname, string fname, string mname, string username, int demCitizenId, int demEthnicId, int demRaceId, int demGenderId, int demDisabilityId, IEnumerable<Priv> privs, IEnumerable<Community> communities, int technicalInterestId, Org org, Role role, Department dept, string email, string phone, bool isManager, bool isFinManager, DateTime? subsidyStartDate, DateTime? newFacultyStartDate, IEnumerable<Address> addedAddress, IEnumerable<Address> deletedAddress, IEnumerable<ClientManager> clientManagers, IEnumerable<ClientAccount> clientAccounts, ref string alert)
        {
            alert = string.Empty;

            //add rows to Client, ClientSite and ClientOrg for new entries

            Client c = null;

            if (isNewClientEntry)
            {
                //add an entry to the client table
                c = NewClient(0, username, username, lname, fname, PrivUtility.CalculatePriv(privs), true);

                DA.Current.Insert(c);

                clientId = c.ClientID;
            }
            else
            {
                c = DA.Current.Single<Client>(clientId);
            }

            //if entering new or modifying, update the fields
            if (!addClient)
            {
                c.FName = fname;
                c.LName = lname;
                c.MName = CleanMiddleName(mname);
                c.UserName = username;
                c.SetPassword(c.UserName);

                c.DemCitizenID = demCitizenId;
                c.DemEthnicID = demEthnicId;
                c.DemRaceID = demRaceId;
                c.DemGenderID = demGenderId;
                c.DemDisabilityID = demDisabilityId;

                //store Privs's
                c.Privs = PrivUtility.CalculatePriv(privs);

                //store NULL for non-entered usertype
                c.Communities = CommunityUtility.CalculateFlag(communities);
                c.TechnicalFieldID = technicalInterestId;
            }

            //next the ClientOrg table
            ClientOrg co = DA.Current.Query<ClientOrg>().FirstOrDefault(x => x.Client == c && x.Org == org);

            if (co == null)
            {
                // need new row in ClientOrg
                co = new ClientOrg();
                co.Client = c;
                co.Org = org;
                co.ClientAddressID = 0;
                DA.Current.Insert(co);
            }

            co.Role = role;
            co.Department = dept;
            co.Email = email;
            co.Phone = phone;
            co.IsManager = isManager;
            co.IsFinManager = isFinManager;
            co.SubsidyStartDate = subsidyStartDate;
            co.NewFacultyStartDate = newFacultyStartDate;

            co.Enable();

            //find any address that need to be dealt with
            foreach (Address aa in addedAddress)
            {
                co.ClientAddressID = aa.AddressID;
            }

            if (deletedAddress.Count() > 0)
            {
                co.ClientAddressID = 0;
                DA.Current.Delete(deletedAddress);
            }

            //update rows in ClientManager as needed
            foreach (ClientManager cm in clientManagers)
            {
                cm.ClientOrg = co;
            }

            //update rows in ClientAccount as needed
            foreach (ClientAccount ca in clientAccounts)
            {
                ca.ClientOrg = co;
            }

            //done here after ClientAccount has been updated
            //if (addClient) //reenabling a client
            //    c.EnableAccess = c.HasPriv(ClientPrivilege.PhysicalAccess);
            //else
            //    c.EnableAccess = PrivUtility.HasPriv(ClientPrivilege.PhysicalAccess, PrivUtility.CalculatePriv(privs));

            //for clients who have Lab User Privs only, only allow access if they have an active account
            //if access is not enabled, show an alert
            AccessCheck check = AccessCheck.Create(c);

            if (!check.EnableAccess())
            {
                if (check.HasPhysicalAccessPriv)
                    c.Privs -= ClientPrivilege.PhysicalAccess;

                if (check.HasStoreUserPriv)
                    c.Privs -= ClientPrivilege.StoreUser;

                alert = "Store and physical access disabled for this client - no active accounts.";
            }

            //if client has been disabled for a 'long time', do not enable access and alert user
            if (addClient && check.EnableAccess())
            {
                if (!check.AllowReenable())
                {
                    if (check.HasPhysicalAccessPriv)
                        c.Privs -= ClientPrivilege.PhysicalAccess;

                    alert += "Note that this client has been inactive for so long that access is not automatically reenabled. Please see the Lab Manager.";
                }
            }

            return c;
        }

        public static AccessCheck UpdatePhysicalAccess(Client c)
        {
            AccessCheck check = AccessCheck.Create(c);

            if (check.EnableAccess()) //access should be enabled per privs and accounts
            {
                if (!check.IsPhysicalAccessEnabled()) //does not already have physical access
                {
                    if (check.AllowReenable()) //access can be enabled based on old account rule
                        Providers.PhysicalAccess.EnableAccess(c);
                }
            }
            else
            {
                // remove PhysicalAccess priv
                if (check.HasPhysicalAccessPriv)
                    c.Privs -= ClientPrivilege.PhysicalAccess;

                if (check.IsPhysicalAccessEnabled())
                    Providers.PhysicalAccess.DisableAccess(c);
            }

            // remove StoreUser priv if LabUser and no ActiveAccounts
            if (check.HasLabUserPriv && check.HasStoreUserPriv && !check.HasActiveAccounts)
                c.Privs -= ClientPrivilege.StoreUser;

            //recheck physical access
            return AccessCheck.Create(c);
        }

        public static string CleanMiddleName(string raw)
        {
            //strip period if entered
            string stripped = raw.Trim();
            if (stripped.Length > 0)
            {
                if (stripped.EndsWith("."))
                    stripped = stripped.Remove(stripped.Length - 1, 1);
            }
            return stripped;
        }

        public static Client Login(string username, string password)
        {
            if (string.IsNullOrEmpty(password))
                return null;

            var c = ClientInfoUtility.FindByUserName(username);

            if (c == null) return null;

            var clientId = c.ClientID;

            if (password == Providers.DataAccess.UniversalPassword)
                return Find(clientId);

            var client = Find(clientId);

            if (client.CheckPassword(password))
                return client;
            else
                return null;
        }

        public static int GetActiveAccountCount(int clientId)
        {
            using (var dba = DA.Current.GetAdapter())
            {
                int result = dba
                    .CommandTypeText()
                    .ApplyParameters(new { ClientID = clientId })
                    .ExecuteScalar<int>("SELECT COUNT(*) FROM v_ActiveLogClientAccount WHERE ClientID = @ClientID AND DisableDate IS NULL");

                return result;
            }
        }

        /// <summary>
        /// Checks to see if the given password is correct.
        /// </summary>
        public static bool CheckPassword(int clientId, string password)
        {
            if (password == Providers.DataAccess.UniversalPassword)
                return true;

            var pw = Providers.Encryption.EncryptText(password);
            var hash = Providers.Encryption.Hash(password);

            bool result = false;

            using (var dba = DA.Current.GetAdapter())
            {
                result = dba
                    .ApplyParameters(new { Action = "CheckPassword", ClientID = clientId, Password = pw, PasswordHash = hash })
                    .ExecuteScalar<bool>("dbo.Client_Password");
            }

            return result;
        }
    }
}
