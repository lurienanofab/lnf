using LNF.Cache;
using LNF.Data;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LNF.Ordering
{
    public class IOFClient
    {
        public readonly static ClientPrivilege StoreManagerPrivilege = ClientPrivilege.Staff | ClientPrivilege.StoreManager;

        private ClientAccountInfo _Client = null;
        private int _ClientID = 0;
        private int _AccountID = 0;

        public IOFClient(int clientId) : this(clientId, 0) { }

        public IOFClient(int clientId, int accountId)
        {
            _ClientID = clientId;
            _AccountID = accountId;

            if (accountId == 0)
                _Client = DA.Current.Query<ClientAccountInfo>().FirstOrDefault(x => x.ClientID == clientId && x.EmailRank == 1);
            else
                _Client = DA.Current.Query<ClientAccountInfo>().FirstOrDefault(x => x.ClientID == clientId && x.AccountID == accountId);
        }

        public bool Loaded
        {
            get { return _Client != null; }
        }

        public int ClientID
        {
            get { return _ClientID; }
        }

        public int AccountID
        {
            get { return _AccountID; }
        }

        public string LastName
        {
            get
            { return (Loaded) ? _Client.LName : string.Empty; }
        }

        public string FirstName
        {
            get { return (Loaded) ? _Client.FName : string.Empty; }
        }

        public string Phone
        {
            get { return (Loaded) ? _Client.Phone : string.Empty; }
        }

        public string Email
        {
            get { return (Loaded) ? _Client.Email : string.Empty; }
        }

        public string DisplayName
        {
            get { return (Loaded) ? string.Empty : _Client.DisplayName; }
        }

        public static IList<ClientSelectListItem> GetAllClients(int clientId)
        {
            var clients = DA.Current.Query<Client>().Where(x => x.Active).OrderBy(x => x.LName).ThenBy(x => x.FName).ToArray();

            //Must return ONLY one row, else is critical error
            var c = clients.FirstOrDefault(x => x.ClientID == clientId);

            List<ClientSelectListItem> result = new List<ClientSelectListItem>();

            result.Add(new ClientSelectListItem { ClientID = -1, DisplayName = "-- View All --" });

            if (c != null)
            {
                result.Add(new ClientSelectListItem { ClientID = c.ClientID, DisplayName = c.DisplayName });
                result.AddRange(clients.Where(x => x.ClientID != clientId).OrderBy(x => x.DisplayName).Select(x => new ClientSelectListItem { ClientID = x.ClientID, DisplayName = x.DisplayName }));
            }
            else
            {
                result.AddRange(clients.OrderBy(x => x.DisplayName).Select(x => new ClientSelectListItem { ClientID = x.ClientID, DisplayName = x.DisplayName }));
            }

            return result;
        }

        public static int ActiveClientID
        {
            get
            {
                if (StoreManager)
                    return 0;
                else
                    return CacheManager.Current.CurrentUser.ClientID;
            }
        }

        public static bool StoreManager
        {
            get
            {
                if (Providers.Context.Current.GetSessionValue("StoreManager") == null) return false;
                if (!HasStoreManagerPriv()) return false;
                return Convert.ToBoolean(Providers.Context.Current.GetSessionValue("StoreManager"));
            }
            set
            {
                Providers.Context.Current.SetSessionValue("StoreManager", value);
            }
        }

        public static bool HasStoreManagerPriv()
        {
            bool result = CacheManager.Current.CurrentUser.HasPriv(StoreManagerPrivilege);
            return result;
        }

        public static bool IsAdministrator()
        {
            bool result = CacheManager.Current.CurrentUser.HasPriv(ClientPrivilege.Administrator);
            return result;
        }

        public class ClientSelectListItem : LNF.GenericListItem
        {
            public int ClientID
            {
                get { return Convert.ToInt32(Value); }
                set { Value = value; }
            }

            public string DisplayName
            {
                get { return Text; }
                set { Text = value; }
            }
        }
    }
}
