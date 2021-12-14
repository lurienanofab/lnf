using LNF.DataAccess;
using System.Web.Http;

namespace LNF.WebApi
{
    public abstract class ApiControllerBase : ApiController
    {
        protected IProvider Provider { get; }

        protected ISession DataSession => Provider.DataAccess.Session;

        public ApiControllerBase(IProvider provider)
        {
            Provider = provider;
        }
    }
}
