using System;
using System.Configuration;
using System.Linq;
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
        public AuthHandler Handler { get; private set; } = null;

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization != null)
            {
                string scheme = actionContext.Request.Headers.Authorization.Scheme.ToLower();

                switch (scheme)
                {
                    case "basic":
                        Handler = new BasicAuthHandler();
                        break;
                    case "forms":
                        Handler = new FormsAuthHandler();
                        break;
                    case "apikey":
                        Handler = new ApiKeyAuthHandler();
                        break;
                    default:
                        return false;
                }

                return Handler.Authenticate(actionContext);
            }

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

            var apikey = ConfigurationManager.AppSettings["ApiKey"];

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
    }

    public abstract class AuthHandler
    {
        public abstract bool Authenticate(HttpActionContext actionContext);
    }
}
