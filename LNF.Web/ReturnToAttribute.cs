using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LNF.Web
{
    public class ReturnToAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string returnTo = filterContext.HttpContext.Request.QueryString["ReturnTo"];

            if (!string.IsNullOrEmpty(returnTo))
            {
                filterContext.HttpContext.Session["ReturnTo"] = returnTo;
                filterContext.Result = new RedirectResult(GetRedirectUrl(filterContext.HttpContext));
            }

            base.OnActionExecuting(filterContext);
        }

        private string GetRedirectUrl(HttpContextBase context)
        {
            string result = context.Request.Url.GetLeftPart(UriPartial.Path);

            // get an array of all the querystring key/value pairs, excluding the ReturnTo parameter
            string[] pairs = context.Request.QueryString.AllKeys.Where(x => x != "ReturnTo").Select(x => string.Format("{0}={1}", x, context.Request.QueryString[x])).ToArray();

            if (pairs.Length > 0)
                result += "?" + string.Join("&", pairs);

            return result;
        }
    }
}
