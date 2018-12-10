using LNF.Cache;
using LNF.Data;
using LNF.Models.Data;
using LNF.Repository;
using System;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace LNF.Web.Mvc
{
    public class LNFAuthorizeAttribute : AuthorizeAttribute
    {
        protected IClientManager ClientManager => ServiceProvider.Current.Use<IClientManager>();

        public Type ModelType { get; set; }
        public ClientPrivilege RequiredPrivilege { get; set; }
        public int[] AllowedClientIDs { get; set; }
        public string AccessDeniedViewName { get; }

        /// <summary>
        /// Authorizes requests using LNF privileges and/or a list of LNF ClientIDs.
        /// </summary>
        /// <param name="requiredPrivilege">The privilege required for access.</param>
        /// <param name="allowedClientIDs">An array of ClientID integers that are allowed access.</param>
        /// <param name="modelType">The model that will be passed to the view if access is denied. Defaults to LNF.Web.Mvc.AccessDeniedModel.</param>
        /// <param name="accessDeniedViewName">The name of the view to display for unauthorized requests. Defaults to "AccessDenied". If null the request will redirect to the login page.</param>
        public LNFAuthorizeAttribute(ClientPrivilege requiredPrivilege = 0, int[] allowedClientIDs = null, Type modelType = null, string accessDeniedViewName = "AccessDenied")
        {
            ModelType = modelType ?? typeof(AccessDeniedModel);
            RequiredPrivilege = requiredPrivilege;
            AllowedClientIDs = allowedClientIDs;
            AccessDeniedViewName = accessDeniedViewName;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var authHeader = httpContext.Request.Headers["Authorization"];
            if (authHeader != null && authHeader.StartsWith("Basic "))
            {
                string token = authHeader.Replace("Basic ", string.Empty);
                string param = Encoding.ASCII.GetString(Convert.FromBase64String(token));
                string[] splitter = param.Split(':');
                if (splitter.Length == 2)
                {
                    string username = splitter[0];
                    string password = splitter[1];

                    var c = ClientManager.Login(username, password);
                    if (c != null && c.ClientActive)
                        httpContext.User = new GenericPrincipal(new GenericIdentity(c.UserName, "Basic"), null);
                }
            }

            bool result;

            if (RequiredPrivilege == 0)
                result = true;
            else
                result = PrivCheck(CacheManager.Current.CurrentUser);

            return result;
        }

        private bool PrivCheck(IPrivileged c)
        {
            bool result = false;

            if (c != null)
            {
                if (c.HasPriv(ClientPrivilege.Developer))
                    result = true;
                else
                    result = c.HasPriv(RequiredPrivilege);

                if (AllowedClientIDs != null && !result)
                    result = AllowedClientIDs.Contains(c.ClientID);
            }

            return result;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!string.IsNullOrEmpty(AccessDeniedViewName))
            {
                ViewEngineResult result = ViewEngines.Engines.FindView(filterContext.Controller.ControllerContext, AccessDeniedViewName, null);
                if (result != null && result.View != null)
                {
                    ViewResult view = new ViewResult() { View = result.View };
                    view.ViewData = new ViewDataDictionary(Activator.CreateInstance(ModelType));
                    view.ViewBag.ReturnUrl = filterContext.RequestContext.HttpContext.Request.RawUrl;
                    filterContext.Result = view;
                    return;
                }
            }

            base.HandleUnauthorizedRequest(filterContext);
        }
    }
}
