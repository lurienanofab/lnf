using LNF.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;

namespace LNF.Web
{
    public static class ListItemCollectionExtentions
    {
        public static void LoadPrivs(this ListItemCollection items, IProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");

            var privs = provider.Data.Client.GetPrivs().ToList();

            foreach (var p in privs)
            {
                var item = new ListItem
                {
                    Text = p.PrivType,
                    Value = ((int)p.PrivFlag).ToString()
                };

                items.Add(item);
            }
        }

        public static ClientPrivilege CalculatePriv(this ListItemCollection items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            int result = 0;

            foreach (ListItem chkPriv in items)
            {
                if (chkPriv.Selected)
                    result += int.Parse(chkPriv.Value);
            }

            return (ClientPrivilege)result;
        }

        public static int CalculateCommunities(this ListItemCollection items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            int result = 0;

            foreach (ListItem chk in items)
            {
                if (chk.Selected)
                    result += int.Parse(chk.Value);
            }

            return result;
        }
    }

    public static class HttpRequestExtensions
    {
        public static string GetDocumentContents(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            string documentContents;
            using (Stream receiveStream = request.InputStream)
            {
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    documentContents = readStream.ReadToEnd();
                }
            }
            return documentContents;
        }

        public static T GetDocumentContents<T>(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            var json = request.GetDocumentContents();
            return JsonConvert.DeserializeObject<T>(json);
        }
    }

    public static class HttpContextBaseExtensions
    {
        public static IClient CurrentUser(this HttpContextBase context, IProvider provider)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (provider == null)
                throw new ArgumentNullException("provider");

            if (context.Items["CurrentUser"] == null)
            {
                context.Items["CurrentUser"] = provider.Data.Client.GetClient(context.User.Identity.Name);
            }

            var result = (IClient)context.Items["CurrentUser"];

            return result;
        }

        public static string CurrentDisplayName(this HttpContextBase context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (context.Items["CurrentUser"] == null)
                return "unknown";

            IClient client = (IClient)context.Items["CurrentUser"];
            return client.DisplayName;
        }

        public static string CurrentIP(this HttpContextBase context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        /// <summary>
        /// Ensures that the current session contains data for the authenticated user.
        /// </summary>
        public static IClient CheckSession(this HttpContextBase context, IProvider provider)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (provider == null)
                throw new ArgumentNullException("provider");

            IClient model = null;

            if (context.User.Identity.IsAuthenticated)
            {
                model = context.CurrentUser(provider);
            }
            else
            {
                // this provides a secret backdoor mechanism for logging in by providing the parameter
                // cid in the querystring, no password required (!) - seems like a very bad idea - maybe
                // this was a debugging thing that wasn't removed? added IsProduction check to be safe
                if (!provider.IsProduction())
                {
                    var qs = context.Request.QueryString;
                    if (qs.AllKeys.Contains("cid"))
                    {
                        if (int.TryParse(qs["cid"], out int cid))
                        {
                            model = provider.Data.Client.GetClient(cid);
                            if (model != null)
                            {
                                var user = new GenericPrincipal(new GenericIdentity(model.UserName), model.Roles());
                                context.User = user;
                                context.Session[SessionKeys.UserName] = model.UserName;
                            }
                        }
                    }
                }
            }

            return context.CheckSession(provider, model);
        }

        public static IClient CheckSession(this HttpContextBase context, IProvider provider, IClient client)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (provider == null)
                throw new ArgumentNullException("provider");

            // at this point the client object still might be null because unauthenticated requests are allowed in some cases
            // and we should now check the session UserName value (if client is null then default values will be used)
            IClient result;
            bool update = false;
            string username = context.GetCurrentUserName();

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
                context.RemoveCacheData();
                context.RemoveAllSessionValues();

                result = provider.Data.Client.GetClient(username);

                context.Session[SessionKeys.CurrentUser] = result; //might be null, that's ok
                context.Session[SessionKeys.Cache] = Guid.NewGuid().ToString("n");
            }

            if (result != null)
            {
                CheckSessionVar(context, SessionKeys.ClientID, result.ClientID);
                CheckSessionVar(context, SessionKeys.UserName, result.UserName);
                CheckSessionVar(context, SessionKeys.DisplayName, result.DisplayName);
                CheckSessionVar(context, SessionKeys.Email, result.Email);
            }

            // now we either have an authenticated user with matching session variables
            // or no authentication was required and the session variables have default values
            return result;
        }

        private static void CheckSessionVar(HttpContextBase context, string key, object val)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (context.Session[key] == null || context.Session[SessionKeys.DisplayName] != val)
                context.Session[SessionKeys.DisplayName] = val;
        }

        public static void RemoveAllSessionValues(this HttpContextBase context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            foreach (string key in SessionKeys.AllKeys())
                context.Session.Remove(key);
        }

        public static string GetCurrentUserName(this HttpContextBase context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var user = context.User;
            if (user == null || user.Identity == null) return null;
            return user.Identity.Name;
        }

        public static void ClearErrorID(this HttpContextBase context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.Session.Remove(SessionKeys.ErrorID);
        }

        public static void SetErrorID(this HttpContextBase context, Guid value)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.Session[SessionKeys.ErrorID] = value;
        }

        public static Guid GetErrorID(this HttpContextBase context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (context.Session[SessionKeys.ErrorID] != null)
            {
                if (Guid.TryParse(Convert.ToString(context.Session[SessionKeys.ErrorID]), out Guid result))
                    return result;
            }

            return Guid.Empty;
        }

        public static void RemoveCacheData(this HttpContextBase context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            context.Cache.Remove(context.CacheID().ToString("n"));
            context.Session.Remove(SessionKeys.Cache);
        }

        public static Guid CacheID(this HttpContextBase context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            if (context.Session["Cache"] == null)
                context.Session["Cache"] = Guid.NewGuid();

            return (Guid)context.Session["Cache"];
        }

        public static void CacheData(this HttpContextBase context, DataSet ds)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var key = context.CacheID().ToString("n");
            context.Cache.Insert(key, ds, null, DateTime.Now.AddMinutes(10), System.Web.Caching.Cache.NoSlidingExpiration);
        }

        public static DataSet CacheData(this HttpContextBase context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var key = context.CacheID().ToString("n");
            var obj = context.Cache[key];
            if (obj == null) return null;
            else return (DataSet)obj;
        }

        /// <summary>
        /// Returns the active ClientOrgs for the current user.
        /// </summary>
        public static IEnumerable<IClient> GetCurrentUserClientOrgs(this HttpContextBase context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var obj = context.Cache["CurrentUserClientOrgs"];
            if (obj == null) return null;
            else return (IEnumerable<IClient>)obj;
            //return context.Cache.GetClientOrgs(context.CurrentUser().ClientID);
        }

        /// <summary>
        /// Returns the active ClientAccounts for the current user.
        /// </summary>
        public static IEnumerable<IClientAccount> GetCurrentUserClientAccounts(this HttpContextBase context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var obj = context.Cache["CurrentUserClientAccounts"];
            if (obj == null) return null;
            else return (IEnumerable<IClientAccount>)obj;
            //return context.Cache.Current.GetClientAccounts(context.CurrentUser().ClientID);
        }
    }
}
