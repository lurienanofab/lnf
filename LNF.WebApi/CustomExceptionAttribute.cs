using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace LNF.WebApi
{
    public class CustomExceptionAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            Exception ex = actionExecutedContext.Exception;

            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                ReasonPhrase = ex.Message,
                Content = new StringContent(ex.ToString())
            };

            actionExecutedContext.Response = response;
        }
    }
}
