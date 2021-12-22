using LNF.DataAccess;
using System.Web.Mvc;

namespace LNF.Web.Mvc
{
    public abstract class BaseController : Controller
    {
        public IProvider Provider { get; }

        public ISession DataSession => Provider.DataAccess.Session;

        public BaseController(IProvider provider)
        {
            Provider = provider;
        }

        public virtual ActionResult AccessDenied()
        {
            return View("AccessDenied");
        }
    }
}
