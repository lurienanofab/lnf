using System.Web;

namespace LNF.Impl.Context
{
    public class WebContextFactory : IHttpContextFactory
    {
        public HttpContextBase CreateContext()
        {
            return new HttpContextWrapper(HttpContext.Current);
        }
    }
}
