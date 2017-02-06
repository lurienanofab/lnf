using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LNF.Web.Mvc
{
    public class AccessDeniedModel : BaseModel
    {
        public string ReturnUrl { get; set; }
    }
}