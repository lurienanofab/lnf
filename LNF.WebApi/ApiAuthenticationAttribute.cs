using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Security;

namespace LNF.WebApi
{
    public class ApiAuthenticationAttribute : AuthorizeAttribute
    {
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            AuthHandler handler;

            if (actionContext.Request.Headers.Authorization != null)
            {
                string scheme = actionContext.Request.Headers.Authorization.Scheme.ToLower();

                switch (scheme)
                {
                    case "basic":
                        handler = new BasicAuthHandler();
                        break;
                    case "forms":
                        handler = new FormsAuthHandler();
                        break;
                    default:
                        return false;
                }

                return handler.Authenticate(actionContext);
            }
             
            handler = new ApiKeyAuthHandler();

            if (handler.Authenticate(actionContext))
                return true;

            TryFormsAuth(actionContext);

            if (IsAuthenticated(actionContext))
                return true;

            return false;
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            // This will prevent redirecting to the login page, which is bad for webapi.
            if (HttpContext.Current != null)
                HttpContext.Current.Response.SuppressFormsAuthenticationRedirect = true;

            base.HandleUnauthorizedRequest(actionContext);
        }

        public bool IsAuthenticated(HttpActionContext actionContext)
        {
            if (actionContext.RequestContext.Principal != null)
                if (actionContext.RequestContext.Principal.Identity != null)
                    return actionContext.RequestContext.Principal.Identity.IsAuthenticated;

            return false;
        }

        private void TryFormsAuth(HttpActionContext actionContext)
        {
            var cookieName = FormsAuthentication.FormsCookieName;
            var cookieHeaderValue = actionContext.Request.Headers.GetCookies(cookieName).FirstOrDefault();
            if (cookieHeaderValue != null)
            {
                var cookieState = cookieHeaderValue.Cookies.FirstOrDefault(x => x.Name == cookieName);
                if (cookieState != null)
                {
                    var ticket = FormsAuthentication.Decrypt(cookieState.Value);
                    var roles = ticket.UserData.Split('|');
                    var ident = new FormsIdentity(ticket);
                    actionContext.RequestContext.Principal = new GenericPrincipal(ident, roles);
                }
            }
        }
    }

    public class BasicAuthHandler : AuthHandler
    {
        public override bool Authenticate(HttpActionContext actionContext)
        {
            string token = actionContext.Request.Headers.Authorization.Parameter;

            string originalString = Encoding.UTF8.GetString(Convert.FromBase64String(token));

            string[] splitter = originalString.Split(':');
            string username = splitter[0];
            string password = splitter[1];

            if (ValidateUser(username, password))
                return true;

            return false;
        }

        private bool ValidateUser(string username, string password)
        {
            string un = ConfigurationManager.AppSettings["BasicAuthUsername"];
            string pw = ConfigurationManager.AppSettings["BasicAuthPassword"];

            if (!string.IsNullOrEmpty(un) && !string.IsNullOrEmpty(pw))
            {
                if (username == un)
                {
                    if (password == pw)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    public class FormsAuthHandler : AuthHandler
    {
        public override bool Authenticate(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization == null)
                return false;

            var token = actionContext.Request.Headers.Authorization.Parameter;

            if (!string.IsNullOrEmpty(token))
            {
                FormsAuthenticationTicket ticket;

                try
                {
                    ticket = GetAuthenticationTicket(token);

                    if (!ticket.Expired)
                    {
                        string[] roles = ticket.UserData.Split('|');
                        actionContext.RequestContext.Principal = new GenericPrincipal(new GenericIdentity(ticket.Name), roles);
                        return true;
                    }
                }
                catch
                {
                    // the ticket is no good
                    return false;
                }
            }

            return false;
        }

        private FormsAuthenticationTicket GetAuthenticationTicket(string token)
        {
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(token);
            return ticket;
        }
    }

    public class ApiKeyAuthHandler : AuthHandler
    {
        public override bool Authenticate(HttpActionContext actionContext)
        {
            var headers = actionContext.Request.Headers.Where(x => x.Key == "apikey").ToList();

            var apikey = GetApiKey();

            if (!string.IsNullOrEmpty(apikey))
            {
                if (headers.Count > 0)
                {
                    var header = headers.First();

                    var val = header.Value.FirstOrDefault();

                    if (!string.IsNullOrEmpty(val))
                    {
                        //e.g. "lnf123"
                        if (val == apikey)
                            return true;
                    }
                }
            }

            return false;
        }

        private string GetApiKey()
        {
            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["ApiKey"]))
                return ConfigurationManager.AppSettings["ApiKey"];

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["BasicAuthPassword"]))
                return ConfigurationManager.AppSettings["BasicAuthPassword"];

            return null;
        }
    }

    public abstract class AuthHandler
    {
        public abstract bool Authenticate(HttpActionContext actionContext);
    }
}
