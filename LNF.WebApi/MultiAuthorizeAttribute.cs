using System;
using System.Collections.Generic;
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
    public class BasicAuthenticationAttribute : ApiAuthorizeAttribute
    {
        public override bool CheckAuthorization(HttpActionContext actionContext)
        {
            if (actionContext.Request.Headers.Authorization != null)
            {
                if (actionContext.Request.Headers.Authorization.Scheme.ToLower() == "basic")
                {
                    string authenticationString = actionContext.Request.Headers.Authorization.Parameter;
                    string originalString = Encoding.UTF8.GetString(Convert.FromBase64String(authenticationString));

                    string[] splitter = originalString.Split(':');
                    string username = splitter[0];
                    string password = splitter[1];

                    if (ValidateUser(username, password))
                        return true;
                }
            }

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

    public class FormsAuthenticationAttribute : ApiAuthorizeAttribute
    {
        public override bool CheckAuthorization(HttpActionContext actionContext)
        {
            if (IsAuthenticated(actionContext))
            {
                // will be true when already signed in (for example ajax call)
                return true;
            }
            else
            {
                // check the authorization header

                if (actionContext.Request.Headers.Authorization != null)
                {
                    if (actionContext.Request.Headers.Authorization.Scheme.ToLower() == "forms")
                    {
                        string accessToken = actionContext.Request.Headers.Authorization.Parameter;

                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            FormsAuthenticationTicket ticket;

                            try
                            {
                                ticket = GetAuthenticationTicket(accessToken);

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
                            }
                        }
                    }
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

    public class ApiKeyAuthenticationAttribute : ApiAuthorizeAttribute
    {
        public override bool CheckAuthorization(HttpActionContext actionContext)
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

    [Flags]
    public enum AuthorizeMethod
    {
        Basic = 1,
        Forms = 2,
        ApiKey = 4
    }

    public class MultiAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly List<ApiAuthorizeAttribute> _attributes;

        /// <summary>
        /// Creates a new instance using all authorize methods.
        /// </summary>
        public MultiAuthorizeAttribute() : this(AuthorizeMethod.Basic | AuthorizeMethod.Forms | AuthorizeMethod.ApiKey) { }

        /// <summary>
        /// Creates a new instance using the specified authorize methods.
        /// </summary>
        public MultiAuthorizeAttribute(AuthorizeMethod method)
        {
            _attributes = new List<ApiAuthorizeAttribute>();

            if ((method & AuthorizeMethod.Basic) > 0)
                _attributes.Add(new BasicAuthenticationAttribute());

            if ((method & AuthorizeMethod.Forms) > 0)
                _attributes.Add(new FormsAuthenticationAttribute());

            if ((method & AuthorizeMethod.ApiKey) > 0)
                _attributes.Add(new ApiKeyAuthenticationAttribute());
        }

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            foreach (var attr in _attributes)
            {
                if (attr.CheckAuthorization(actionContext))
                    return true;
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
    }

    public abstract class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        public abstract bool CheckAuthorization(HttpActionContext actionContext);

        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            return CheckAuthorization(actionContext);
        }

        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            // This will prevent redirecting to the login page, which is bad for webapi.
            if (HttpContext.Current != null)
                HttpContext.Current.Response.SuppressFormsAuthenticationRedirect = true;

            base.HandleUnauthorizedRequest(actionContext);
        }

        protected bool IsAuthenticated(HttpActionContext actionContext)
        {
            if (actionContext.RequestContext.Principal != null)
                if (actionContext.RequestContext.Principal.Identity != null)
                    return actionContext.RequestContext.Principal.Identity.IsAuthenticated;

            return false;
        }
    }
}
