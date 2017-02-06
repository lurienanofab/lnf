using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using LNF.Data;
using LNF.Repository;
using LNF.Repository.Data;

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
