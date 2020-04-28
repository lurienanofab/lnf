using LNF.Data;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Security.Principal;

namespace LNF.Impl.Cache
{
    public class Cache
    {
        private readonly MemoryCache _cache;

        protected ISession Session { get; }
        protected IContext Context { get; }

        public Cache(ISession session, IContext context)
        {
            _cache = new MemoryCache("CacheManager");
            Session = session;
            Context = context;
        }

        /// <summary>
        /// Gets the currently authenticated username using the current context IPrincipal user (the user loged in via Forms Authentication).
        /// </summary>
        [Obsolete("Use HttpContextBase instead.")]
        public string GetCurrentUserName()
        {
            var user = Context.User;
            if (user == null || user.Identity == null) return null;
            return user.Identity.Name;
        }

        /// <summary>
        /// Gets all active clients. Clients are cached for 5 minutes.
        /// </summary>
        [Obsolete("Use Provider.Data.Client.GetActiveClients instead.")]
        public IEnumerable<IClient> Clients() => ServiceProvider.Current.Data.Client.GetActiveClients();

        /// <summary>
        /// Gets one active client by ClientID. Clients are cached for 5 minutes.
        /// </summary>
        [Obsolete("Use Provider.Data.Client.GetClient instead.")]
        public IClient GetClient(int clientId) => ServiceProvider.Current.Data.Client.GetClient(clientId);

        /// <summary>
        /// Gets one active client by UserName. Clients are cached for 5 minutes.
        /// </summary>
        [Obsolete("Use Provider.Data.Client.GetClient instead.")]
        public IClient GetClient(string username) => ServiceProvider.Current.Data.Client.GetClient(username);

        /// <summary>
        /// The currently logged in user. Returns null if no one is logged in.
        /// </summary>
        [Obsolete("Use HttpContextBase instead.")]
        public IClient CurrentUser
        {
            get
            {
                throw new NotImplementedException();
                //var sessionValue = GetSessionValue(SessionKeys.CurrentUser, s => GetClient(GetCurrentUserName()));
                //var result = CheckSession(sessionValue);
                //return result;
            }
        }

        /// <summary>
        /// Ensures that the current session contains data for the authenticated user.
        /// </summary>
        [Obsolete("Use HttpContextBase instead.")]
        public IClient CheckSession()
        {
            IClient model = null;

            if (Context.User.Identity.IsAuthenticated)
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
                    var qs = Context.QueryString;
                    if (qs.AllKeys.Contains("cid"))
                    {
                        if (int.TryParse(qs["cid"], out int cid))
                        {
                            model = GetClient(cid);
                            if (model != null)
                            {
                                var user = new GenericPrincipal(new GenericIdentity(model.UserName), model.Roles());
                                Context.User = user;
                                Context.SetSessionValue("UserName", model.UserName);
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

                Context.SetSessionValue(SessionKeys.CurrentUser, result); //might be null, that's ok
                Context.SetSessionValue(SessionKeys.Cache, Guid.NewGuid().ToString("n"));
            }

            // now we either have an authenticated user with matching session variables
            // or no authentication was required and the session variables have default values
            return result;
        }

        public bool IsProduction() => Configuration.Current.Production;

        public bool WagoEnabled => CommonTools.Utility.ConvertTo(Context.GetAppSetting("WagoEnabled"), false);

        public bool UseStartReservationPage => CommonTools.Utility.ConvertTo(Context.GetAppSetting("UseStartReservationPage"), false);

        public bool ShowCanceledForModification => CommonTools.Utility.ConvertTo(Context.GetAppSetting("ShowCanceledForModification"), false);



    }
}
