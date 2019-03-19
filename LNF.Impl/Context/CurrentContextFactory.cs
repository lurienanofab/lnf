using System.Web;

namespace LNF.Impl.Context
{
    /// <summary>
    /// Implementation of IHttpContextFactory that uses HttpContext.Current.
    /// </summary>
    public class CurrentContextFactory : IHttpContextFactory
    {
        public HttpContextBase Create()
        {
            return new HttpContextWrapper(HttpContext.Current);
        }
    }
}
