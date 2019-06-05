using System.Web;

namespace LNF.Scripting
{
    public class WebContextFactory
    {
        public HttpContextBase GetContext()
        {
            return new HttpContextWrapper(HttpContext.Current);
        }
    }
}
