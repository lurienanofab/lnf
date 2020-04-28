using System.Web.Mvc;

namespace LNF.Web.Mvc
{
    public abstract class BaseController : Controller
    {
        public virtual ActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
    }
}
