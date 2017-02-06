using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LNF.Web.Mvc
{
    public class CustomJsonResult : JsonResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            var response = context.HttpContext.Response;

            response.ContentType = string.IsNullOrEmpty(ContentType) ? "application/json" : ContentType;

            response.ContentEncoding = (ContentEncoding == null) ? Encoding.UTF8 : ContentEncoding;

            if (Data != null)
            {
                var isoConvert = new IsoDateTimeConverter();
                response.Write(JsonConvert.SerializeObject(Data, isoConvert));
            }
        }
    }
}
