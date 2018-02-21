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
        public static Client NewClient(string username, string password, string lname, string fname, ClientPrivilege privs, bool active)
        {
            var result = new Client()
            {
                UserName = username,
                LName = lname,
                FName = fname,
                Privs = privs
            };

            DA.Current.Insert(result);

            SetPassword(result.ClientID, password);

            if (active)
                result.Enable();
            else
                result.Disable();

            return result;
        }

        public static Client StoreClientInfo(ref int clientId, string lname, string fname, string mname, string username, ClientDemographics demographics, IEnumerable<Priv> privs, IEnumerable<Community> communities, int technicalFieldId, Org org, Role role, Department dept, string email, string phone, bool isManager, bool isFinManager, DateTime? subsidyStart, DateTime? newFacultyStart, IEnumerable<Address> addedAddress, IEnumerable<Address> deletedAddress, IEnumerable<ClientManager> clientManagers, IEnumerable<ClientAccount> clientAccounts, out string alert)
        {
            //add rows to Client, ClientSite and ClientOrg for new entries

            Client c = DA.Current.Single<Client>(clientId);

            bool isNewClientEntry = c == null;

            if (isNewClientEntry)
            {
                //add an entry to the client table
                c = NewClient(username, username, lname, fname, PrivUtility.CalculatePriv(privs), true);
                clientId = c.ClientID;
            }
            else
            {
                c.UserName = username;
                c.FName = fname;
                c.LName = lname;

                //store Privs's
                c.Privs = PrivUtility.CalculatePriv(privs);

                SetPassword(c.ClientID, c.UserName);
            }

            c.MName = CleanMiddleName(mname);
            demographics.Update(c);

            //store NULL for non-entered usertype
            c.Communities = CommunityUtility.CalculateFlag(communities);
            c.TechnicalFieldID = technicalFieldId;

            //next the ClientOrg table
            ClientOrg co = DA.Current.Query<ClientOrg>().FirstOrDefault(x => x.Client == c && x.Org == org);

            if (co == null)
            {
                // need new row in ClientOrg
                co = new ClientOrg
                {
                    Client = c,
                    Org = org,
                    ClientAddressID = 0
                };

                DA.Current.Insert(co);
            }

            co.Role = role;
            co.Department = dept;
            co.Email = email;
            co.Phone = phone;
            co.IsManager = isManager;
            co.IsFinManager = isFinManager;
            co.SubsidyStartDate = subsidyStart;
            co.NewFacultyStartDate = newFacultyStart;

            co.Enable();

            //find any address that need to be dealt with
            if (addedAddress != null && addedAddress.Count() > 0)
            {
                foreach (Address aa in addedAddress)
                {
                    co.ClientAddressID = aa.AddressID;
                }
            }

            if (deletedAddress != null && deletedAddress.Count() > 0)
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

            // For clients who have Lab User priv only, only allow access if they
            // have an active account. If access is not enabled, show an alert.
            AccessCheck check = AccessCheck.Create(c);
            UpdatePhysicalAccess(check, out alert);

            return c;
        }

        public static bool UpdatePhysicalAccess(AccessCheck check, out string alert)
        {
            bool result;

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

        public static string CleanMiddleName(string raw)
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

        public static Client Login(string username, string password)
        {
            if (string.IsNullOrEmpty(password))
                return null;

            var c = ClientInfoUtility.FindByUserName(username);

            if (c == null)
                throw new Exception("Invalid username.");

            if (!string.IsNullOrEmpty(Providers.DataAccess.UniversalPassword) && password == Providers.DataAccess.UniversalPassword)
                return DA.Current.Single<Client>(c.ClientID);

            if (CheckPassword(c.ClientID, password))
                return DA.Current.Single<Client>(c.ClientID);
            else
                throw new Exception("Invalid password.");
        }

        /// <summary>
        /// Returns the number of currently active accounts for a given client.
        /// </summary>
        public static int GetActiveAccountCount(int clientId)
        {
            int result = DA.Current.Query<ActiveLogClientAccount>().Count(x => x.ClientID == clientId && !x.DisableDate.HasValue);
            return result;
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

            bool result = DA.Current.NamedQuery("CheckPassword", new { ClientID = clientId, Password = pw, PasswordHash = hash }).Result<bool>();

            return result;
        }

        /// <summary>
        /// Sets the user's password.
        /// </summary>
        public static int SetPassword(int clientId, string password)
        {
            var pw = Providers.Encryption.EncryptText(password);
            var hash = Providers.Encryption.Hash(password);

            int result = DA.Current.NamedQuery("SetPassword", new { ClientID = clientId, Password = pw, PasswordHash = hash }).Result<int>();

            return result;
        }

        public static IEnumerable<Client> GetActiveClients()
        {
            DateTime ed = DateTime.Now.AddDays(1);
            DateTime sd = ed.AddMonths(-1);
            return GetActiveClients(sd, ed);
        }

        public static IEnumerable<Client> GetActiveClients(DateTime sd, DateTime ed)
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

            var activeLogs = DA.Current.Query<ActiveLog>().Where(x => x.TableName == "Client" && x.EnableDate < ed && (!x.DisableDate.HasValue || x.DisableDate.Value > sd));

            var join = activeLogs
                .Join(DA.Current.Query<Client>(), o => o.Record, i => i.ClientID, (o, i) => new { ActiveLog = o, Client = i });

            return join.Select(x => x.Client).ToList();
        }

        public static IEnumerable<Client> GetActiveClients(ClientPrivilege priv)
        {
            DateTime ed = DateTime.Now.AddDays(1);
            DateTime sd = ed.AddMonths(-1);
            return GetActiveClients(sd, ed, priv);
        }

        public static IQueryable<Client> GetActiveClients(DateTime sd, DateTime ed, ClientPrivilege priv)
        {
            var activeLogs = DA.Current.Query<ActiveLog>().Where(x => x.TableName == "Client" && x.EnableDate < ed && (!x.DisableDate.HasValue || x.DisableDate.Value > sd));

            var join = activeLogs
                .Join(DA.Current.Query<Client>(), o => o.Record, i => i.ClientID, (o, i) => new { ActiveLog = o, Client = i });

            return join.Where(x => (x.Client.Privs & priv) > 0).Select(x => x.Client);
        }
    }
}
