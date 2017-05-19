using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Scheduler;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Principal;

namespace LNF.Cache
{
    public class CacheManager
    {
        public static CacheManager Current { get; }

        static CacheManager()
        {
            Current = new CacheManager();
        }

        public MongoRepository Repository
        {
            get
            {
                return MongoRepository.Default;
            }
        }

        internal IMongoCollection<CacheObject<T>> GetCollection<T>(string name)
        {
            return Repository.GetClient()
                .GetDatabase("cachemgr")
                .GetCollection<CacheObject<T>>(name);
        }

        internal IMongoCollection<CacheObject<ClientModel>> GetClientCollection()
        {
            return GetCollection<ClientModel>("clients")
                .Expire(TimeSpan.FromHours(1), x => x.CreatedAt)
                .Unique(x => x.Value.ClientID);
        }

        public string GetCurrentUserName()
        {
            var user = Providers.Context.Current.User;
            if (user == null || user.Identity == null) return null;
            return user.Identity.Name;
        }

        public ClientModel GetClient(string username)
        {
            IList<ClientModel> list = GetContextItem<IList<ClientModel>>("Clients");

            if (list == null)
            {
                list = new List<ClientModel>();
                SetContextItem("Clients", list);
            }

            ClientModel result = list.FirstOrDefault(x => x.UserName == username);

            if (result == null)
            {
                var query = GetClientCollection().Query(x => x.Value.UserName == username, () => CacheObjectFactory.CreateMany(DA.Current.Query<ClientInfo>().Where(x => x.UserName == username).Model<ClientModel>()), false);
                result = query.FirstOrDefault().GetValue();
                list.Add(result);
            }

            return result;
        }

        public ClientModel GetClient(int clientId)
        {
            IList<ClientModel> list = GetContextItem<IList<ClientModel>>("Clients");

            if (list == null)
            {
                list = new List<ClientModel>();
                SetContextItem("Clients", list);
            }

            ClientModel result = list.FirstOrDefault(x => x.ClientID == clientId);

            if (result == null)
            {
                var query = GetClientCollection().Query(x => x.Value.ClientID == clientId, () => CacheObjectFactory.CreateMany(DA.Current.Query<ClientInfo>().Where(x => x.ClientID == clientId).Model<ClientModel>()), false);
                result = query.FirstOrDefault().GetValue();
                if (result != null)
                    list.Add(result);
            }

            return result;
        }

        /// <summary>
        /// The currently logged in user. Returns null if no one is logged in.
        /// </summary>
        public ClientModel CurrentUser
        {
            get
            {
                var result = GetContextItem<ClientModel>("CurrentUser");

                if (result == null)
                {
                    result = GetClient(GetCurrentUserName());
                    SetContextItem("CurrentUser", result);
                }

                return result;
            }
        }

        /// <summary>
        /// Ensures that the current session contains data for the authenticated client.
        /// </summary>
        public void CheckSession()
        {
            ClientModel model = null;

            if (Providers.Context.Current.User.Identity.IsAuthenticated)
            {
                model = CurrentUser;
            }
            else
            {
                // this provides a secret backdoor mechanism for logging in by providing the parameter
                // cid in the querystring, no password required (!) - seems like a very bad idea - maybe
                // this was a debugging thing that wasn't removed? added IsProduction check to be safe
                if (!IsProduction())
                {
                    var qs = Providers.Context.Current.QueryString;
                    if (qs.AllKeys.Contains("cid"))
                    {
                        int cid;
                        if (int.TryParse(qs["cid"], out cid))
                        {
                            model = GetClient(cid);
                            if (model != null)
                            {
                                var user = new GenericPrincipal(new GenericIdentity(model.UserName), model.Roles());
                                Providers.Context.Current.User = user;
                                Providers.Context.Current.SetSessionValue("UserName", model.UserName);
                            }
                        }
                    }
                }
            }

            // at this point the client object still might be null because unauthenticated requests are allowed in some cases
            // and we should now check the session UserName value (if client is null then default values will be used)

            bool setValues = false;
            string username = string.Empty;
            bool active = true; //becuase an unauthenticated user should not be considered inactive
            int clientId = 0;
            int communities = 0;
            string displayName = string.Empty;
            ClientPrivilege privs = 0;
            string email = string.Empty;
            string phone = string.Empty;
            int orgId = 0;
            int maxChargeTypeId = 0;

            // see if the session is invalid
            if (model != null)
            {
                string un = GetSessionValue(SessionKeys.UserName, () => string.Empty);

                if (un != GetCurrentUserName())
                {
                    // the UserName session variable is incorrect so reload everything
                    setValues = true;
                    username = model.UserName;
                    active = model.ClientActive;
                    clientId = model.ClientID;
                    communities = model.Communities;
                    displayName = model.DisplayName;
                    privs = model.Privs;
                    email = model.Email;
                    phone = model.Phone;
                    orgId = model.OrgID;
                    maxChargeTypeId = model.MaxChargeTypeID;

                    // unset the UserState object
                    this.DeleteUserState(clientId);
                }
            }
            else
            {
                // there is no client probably because this request does not require authentication
                // in this case we should set the session variables to default values (remember the session was cleared)
                setValues = true;
            }

            if (setValues)
            {
                foreach (string key in SessionKeys.AllKeys())
                    Providers.Context.Current.RemoveSessionValue(key);

                RemoveCacheData();

                Providers.Context.Current.SetSessionValue(SessionKeys.Logout, GetLoginUrl());
                Providers.Context.Current.SetSessionValue(SessionKeys.UserName, username);
                Providers.Context.Current.SetSessionValue(SessionKeys.ClientID, clientId);
                Providers.Context.Current.SetSessionValue(SessionKeys.Active, active);
                Providers.Context.Current.SetSessionValue(SessionKeys.Communities, communities);
                Providers.Context.Current.SetSessionValue(SessionKeys.DisplayName, displayName);
                Providers.Context.Current.SetSessionValue(SessionKeys.Email, email);
                Providers.Context.Current.SetSessionValue(SessionKeys.MaxChargeTypeID, maxChargeTypeId);
                Providers.Context.Current.SetSessionValue(SessionKeys.Phone, phone);
                Providers.Context.Current.SetSessionValue(SessionKeys.Privs, privs);
                Providers.Context.Current.SetSessionValue(SessionKeys.OrgID, orgId);
                Providers.Context.Current.SetSessionValue(SessionKeys.Cache, Guid.NewGuid().ToString("n"));
                Providers.Context.Current.SetSessionValue(SessionKeys.IsKiosk, KioskUtility.IsKiosk());
            }

            // now we either have an authenticated user with matching session variables
            // or no authentication was required and the session variables have default values
        }

        public int ClientID
        {
            get { return GetSessionValue(SessionKeys.ClientID, () => 0); }
        }

        public string Email
        {
            get { return GetSessionValue(SessionKeys.Email, () => string.Empty); }
        }

        public int MaxChargeTypeID
        {
            get { return GetSessionValue(SessionKeys.MaxChargeTypeID, () => 0); }
        }

        public string Logout
        {
            get { return GetSessionValue(SessionKeys.Logout, () => GetLoginUrl()); }
            set { SetSessionValue(SessionKeys.Logout, value); }
        }

        public bool IsProduction()
        {
            return Providers.IsProduction();
        }

        public string GetLoginUrl()
        {
            return Providers.Context.LoginUrl;
        }

        public bool WagoEnabled
        {
            get { return RepositoryUtility.ConvertTo(Providers.Context.Current.GetAppSetting("WagoEnabled"), false); }
        }

        public bool UseStartReservationPage
        {
            get { return RepositoryUtility.ConvertTo(Providers.Context.Current.GetAppSetting("UseStartReservationPage"), false); }
        }

        public bool ShowCanceledForModification
        {
            get { return RepositoryUtility.ConvertTo(Providers.Context.Current.GetAppSetting("ShowCanceledForModification"), false); }
        }

        public string ErrorID
        {
            get { return GetSessionValue(SessionKeys.ErrorID, () => string.Empty); }
            set { SetSessionValue(SessionKeys.ErrorID, value); }
        }

        public void RemoveCacheData()
        {
            RemoveSessionValue(Cache.ToString("n"));
            RemoveSessionValue(SessionKeys.Cache);
        }

        public Guid Cache
        {
            get { return GetSessionValue(SessionKeys.Cache, () => Guid.NewGuid()); }
            set { SetSessionValue(SessionKeys.Cache, value); }
        }

        public void CacheData(DataSet ds)
        {
            SetSessionValue(Cache.ToString("n"), ds);
        }

        public DataSet CacheData()
        {
            DataSet result = GetSessionValue<DataSet>(Cache.ToString("n"), () => null);
            return result;
        }

        public void AbandonSession()
        {
            Providers.Context.Current.AbandonSession();
        }

        public void RemoveSessionValue(string key)
        {
            Providers.Context.Current.RemoveSessionValue(key);
        }

        public T GetSessionValue<T>(string key, Func<T> defval)
        {
            object value = Providers.Context.Current.GetSessionValue(key);

            T result;

            if (value == null || !(value is T))
            {
                result = defval();
                SetSessionValue(key, result);
            }
            else
            {
                result = (T)value;
            }

            return result;
        }

        public void SetSessionValue(string key, object value)
        {
            Providers.Context.Current.SetSessionValue(key, value);
        }

        public void RemoveContextItem(string key)
        {
            Providers.Context.Current.Items.Remove(key);
        }

        public T GetContextItem<T>(string key)
        {
            return Providers.Context.Current.GetItem<T>(key);
        }

        public void SetContextItem<T>(string key, T item)
        {
            Providers.Context.Current.SetItem(key, item);
        }
    }
}
