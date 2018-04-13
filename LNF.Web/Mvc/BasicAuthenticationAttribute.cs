using System;
using System.Configuration;
using System.Text;
using System.Web.Mvc;

namespace LNF.Web.Mvc
{
    public class BasicAuthenticationAttribute : ActionFilterAttribute
    {
        private static string GetUsername()
        {
            return ConfigurationManager.AppSettings["BasicAuthUsername"];
        }

        private static string GetPassword()
        {
            return ConfigurationManager.AppSettings["BasicAuthPassword"];
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var req = filterContext.HttpContext.Request;
            var auth = req.Headers["Authorization"];

            if (!String.IsNullOrEmpty(auth))
            {
                var cred = Encoding.ASCII.GetString(Convert.FromBase64String(auth.Substring(6))).Split(':');
                var user = new { name = cred[0], pass = cred[1] };

                if (user.name == GetUsername() && user.pass == GetPassword())
                    return;
            }

            //filterContext.HttpContext.Response.AddHeader("WWW-Authenticate", "Basic realm=\"\"");

            filterContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;
            filterContext.Result = new HttpUnauthorizedResult();
        }
    }
}