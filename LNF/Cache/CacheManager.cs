using LNF.CommonTools;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using LNF.Scheduler;
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

        private CacheManager() { }

        public string GetCurrentUserName()
        {
            var user = ServiceProvider.Current.Context.User;
            if (user == null || user.Identity == null) return null;
            return user.Identity.Name;
        }

        public ClientItem GetClient(string username)
        {
            IList<ClientItem> list = GetContextItem<IList<ClientItem>>("Clients");

            if (list == null)
            {
                list = new List<ClientItem>();
                SetContextItem("Clients", list);
            }

            ClientItem result = list.FirstOrDefault(x => x.UserName == username);

            if (result == null)
            {
                result = DA.Current.Query<ClientInfo>().FirstOrDefault(x => x.UserName == username).Model<ClientItem>();
                list.Add(result);
            }

            return result;
        }

        public ClientItem GetClient(int clientId)
        {
            IList<ClientItem> list = GetContextItem<IList<ClientItem>>("Clients");

            if (list == null)
            {
                list = new List<ClientItem>();
                SetContextItem("Clients", list);
            }

            ClientItem result = list.FirstOrDefault(x => x.ClientID == clientId);

            if (result == null)
            {
                result = DA.Current.Query<ClientInfo>().FirstOrDefault(x => x.ClientID == clientId).Model<ClientItem>();
                if (result != null)
                    list.Add(result);
            }

            return result;
        }

        /// <summary>
        /// The currently logged in user. Returns null if no one is logged in.
        /// </summary>
        public ClientItem CurrentUser
        {
            get
            {
                var result = GetContextItem<ClientItem>("CurrentUser");

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
            ClientItem model = null;

            if (ServiceProvider.Current.Context.User.Identity.IsAuthenticated)
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
                    var qs = ServiceProvider.Current.Context.QueryString;
                    if (qs.AllKeys.Contains("cid"))
                    {
                        if (int.TryParse(qs["cid"], out int cid))
                        {
                            model = GetClient(cid);
                            if (model != null)
                            {
                                var user = new GenericPrincipal(new GenericIdentity(model.UserName), model.Roles());
                                ServiceProvider.Current.Context.User = user;
                                ServiceProvider.Current.Context.SetSessionValue("UserName", model.UserName);
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
                    ServiceProvider.Current.Context.RemoveSessionValue(key);

                RemoveCacheData();

                ServiceProvider.Current.Context.SetSessionValue(SessionKeys.Logout, GetLoginUrl());
                ServiceProvider.Current.Context.SetSessionValue(SessionKeys.UserName, username);
                ServiceProvider.Current.Context.SetSessionValue(SessionKeys.ClientID, clientId);
                ServiceProvider.Current.Context.SetSessionValue(SessionKeys.Active, active);
                ServiceProvider.Current.Context.SetSessionValue(SessionKeys.Communities, communities);
                ServiceProvider.Current.Context.SetSessionValue(SessionKeys.DisplayName, displayName);
                ServiceProvider.Current.Context.SetSessionValue(SessionKeys.Email, email);
                ServiceProvider.Current.Context.SetSessionValue(SessionKeys.MaxChargeTypeID, maxChargeTypeId);
                ServiceProvider.Current.Context.SetSessionValue(SessionKeys.Phone, phone);
                ServiceProvider.Current.Context.SetSessionValue(SessionKeys.Privs, privs);
                ServiceProvider.Current.Context.SetSessionValue(SessionKeys.OrgID, orgId);
                ServiceProvider.Current.Context.SetSessionValue(SessionKeys.Cache, Guid.NewGuid().ToString("n"));
                ServiceProvider.Current.Context.SetSessionValue(SessionKeys.IsKiosk, KioskUtility.IsKiosk());
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
            return ServiceProvider.Current.IsProduction();
        }

        public string GetLoginUrl()
        {
            return ServiceProvider.Current.Context.LoginUrl;
        }

        public bool WagoEnabled
        {
            get { return Utility.ConvertTo(ServiceProvider.Current.Context.GetAppSetting("WagoEnabled"), false); }
        }

        public bool UseStartReservationPage
        {
            get { return Utility.ConvertTo(ServiceProvider.Current.Context.GetAppSetting("UseStartReservationPage"), false); }
        }

        public bool ShowCanceledForModification
        {
            get { return Utility.ConvertTo(ServiceProvider.Current.Context.GetAppSetting("ShowCanceledForModification"), false); }
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
            ServiceProvider.Current.Context.AbandonSession();
        }

        public void RemoveSessionValue(string key)
        {
            ServiceProvider.Current.Context.RemoveSessionValue(key);
        }

        public T GetSessionValue<T>(string key, Func<T> defval)
        {
            object value = ServiceProvider.Current.Context.GetSessionValue(key);

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
            ServiceProvider.Current.Context.SetSessionValue(key, value);
        }

        public void RemoveContextItem(string key)
        {
            ServiceProvider.Current.Context.Items.Remove(key);
        }

        public T GetContextItem<T>(string key)
        {
            return ServiceProvider.Current.Context.GetItem<T>(key);
        }

        public void SetContextItem<T>(string key, T item)
        {
            ServiceProvider.Current.Context.SetItem(key, item);
        }
    }
}
