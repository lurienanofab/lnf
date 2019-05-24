using LNF.CommonTools;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Caching;
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

        private MemoryCache _cache;

        private CacheManager()
        {
            _cache = new MemoryCache("CacheManager");
        }

        /// <summary>
        /// Gets the currently authenticated username using the current context IPrincipal user (the user loged in via Forms Authentication).
        /// </summary>
        [Obsolete("Use HttpContextBase instead.")]
        public string GetCurrentUserName()
        {
            var user = ServiceProvider.Current.Context.User;
            if (user == null || user.Identity == null) return null;
            return user.Identity.Name;
        }

        /// <summary>
        /// Gets all active clients. Clients are cached for 5 minutes.
        /// </summary>
        public IEnumerable<IClient> Clients() => GetValue("Clients", () => DA.Current.Query<ClientInfo>().Where(x => x.ClientActive).CreateModels<IClient>(), DateTimeOffset.Now.AddMinutes(5));

        /// <summary>
        /// Gets one active client by ClientID. Clients are cached for 5 minutes.
        /// </summary>
        public IClient GetClient(int clientId) => Clients().FirstOrDefault(x => x.ClientID == clientId);

        /// <summary>
        /// Gets one active client by UserName. Clients are cached for 5 minutes.
        /// </summary>
        public IClient GetClient(string username) => Clients().FirstOrDefault(x => x.UserName == username);

        /// <summary>
        /// The currently logged in user. Returns null if no one is logged in.
        /// </summary>
        [Obsolete("Use HttpContextBase instead.")]
        public IClient CurrentUser
        {
            get
            {
                var sessionValue = GetSessionValue(SessionKeys.CurrentUser, () => GetClient(GetCurrentUserName()));
                var result = CheckSession(sessionValue);
                return result;
            }
        }

        /// <summary>
        /// Ensures that the current session contains data for the authenticated user.
        /// </summary>
        [Obsolete("Use HttpContextBase instead.")]
        public IClient CheckSession()
        {
            IClient model = null;

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

            return CheckSession(model);
        }

        [Obsolete("Use HttpContextBase instead.")]
        public IClient CheckSession(IClient client)
        {
            // at this point the client object still might be null because unauthenticated requests are allowed in some cases
            // and we should now check the session UserName value (if client is null then default values will be used)
            IClient result;
            bool update = false;
            string username = GetCurrentUserName();

            // see if the session is invalid
            if (client != null)
            {
                if (client.UserName != username)
                {
                    // the session is incorrect so reload everything
                    update = true;
                    result = null;
                }
                else
                    result = client;
            }
            else
            {
                // there is no client probably because this request does not require authentication
                // in this case we should set the session variables to default values (remember the session was cleared)
                update = true;
                result = null;
            }

            if (update)
            {
                SessionKeys.RemoveAll();

                //RemoveCacheData();

                result = GetClient(username);

                ServiceProvider.Current.Context.SetSessionValue(SessionKeys.CurrentUser, result); //might be null, that's ok
                ServiceProvider.Current.Context.SetSessionValue(SessionKeys.Cache, Guid.NewGuid().ToString("n"));
            }

            // now we either have an authenticated user with matching session variables
            // or no authentication was required and the session variables have default values
            return result;
        }

        public bool IsProduction() => ServiceProvider.Current.IsProduction();

        public bool WagoEnabled => Utility.ConvertTo(ServiceProvider.Current.Context.GetAppSetting("WagoEnabled"), false);

        public bool UseStartReservationPage => Utility.ConvertTo(ServiceProvider.Current.Context.GetAppSetting("UseStartReservationPage"), false);

        public bool ShowCanceledForModification => Utility.ConvertTo(ServiceProvider.Current.Context.GetAppSetting("ShowCanceledForModification"), false);

        [Obsolete("Use HttpContextBase instead.")]
        public void AbandonSession() => ServiceProvider.Current.Context.AbandonSession();

        [Obsolete("Use HttpContextBase instead.")]
        public void RemoveSessionValue(string key) => ServiceProvider.Current.Context.RemoveSessionValue(key);

        [Obsolete("Use HttpContextBase instead.")]
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

        [Obsolete("Use HttpContextBase instead.")]
        public void SetSessionValue(string key, object value) => ServiceProvider.Current.Context.SetSessionValue(key, value);

        [Obsolete("Use HttpContextBase instead.")]
        public void RemoveContextItem(string key) => ServiceProvider.Current.Context.Items.Remove(key);

        [Obsolete("Use HttpContextBase instead.")]
        public T GetContextItem<T>(string key) => ServiceProvider.Current.Context.GetItem<T>(key);

        [Obsolete("Use HttpContextBase instead.")]
        public void SetContextItem<T>(string key, T item) => ServiceProvider.Current.Context.SetItem(key, item);

        public object GetValue(string key) => _cache.Get(key);

        public T GetValue<T>(string key, Func<T> defval, DateTimeOffset? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
        {
            object value = GetValue(key);

            T result;

            if (value == null || !(value is T))
            {
                result = defval();
                SetValue(key, result, absoluteExpiration, slidingExpiration);
            }
            else
            {
                result = (T)value;
            }

            return result;
        }

        public void SetValue(string key, object value, DateTimeOffset? absoluteExpiration = null, TimeSpan? slidingExpiration = null)
        {
            _cache.Set(key, value, new CacheItemPolicy()
            {
                AbsoluteExpiration = absoluteExpiration.GetValueOrDefault(ObjectCache.InfiniteAbsoluteExpiration),
                SlidingExpiration = slidingExpiration.GetValueOrDefault()
            });
        }

        public object RemoveValue(string key) => _cache.Remove(key);

        public void ClearCache()
        {
            _cache.Dispose();
            _cache = new MemoryCache("CacheManager");
        }

        public long GetApproximateSize() => _cache.GetApproximateSize();
    }
}
