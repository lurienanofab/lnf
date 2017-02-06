using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace LNF.Web.Mvc.Utility
{
    public class JsonpResult : JsonResult
    {
        public string Callback { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var request = context.HttpContext.Request;
            var response = context.HttpContext.Response;

            if (!string.IsNullOrEmpty(Callback))
            {
                if (string.IsNullOrEmpty(base.ContentType))
                    base.ContentType = "application/x-javascript";

                response.Write(string.Format("{0}(", Callback));
            }

            base.ExecuteResult(context);
            
            if (!string.IsNullOrEmpty(Callback))
                response.Write(")");
        }
    }
}
