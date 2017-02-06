using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Net.Http;
using OnlineServices.Api;

namespace LNF.WebApi
{
    public static class AuthorizationExtensions
    {
        public static ApiClientOptions GetClientOptions(this HttpActionContext context)
        {
            return context.Request.GetOwinContext().Get<ApiClientOptions>("ApiClientOptions");
        }

        public static void SetClientOptions(this HttpActionContext context, ApiClientOptions value)
        {
            context.Request.GetOwinContext().Set<ApiClientOptions>("ApiClientOptions", value);
        }
    }
}
