using System.Web;

namespace LNF.Impl.Context
{
    public interface IHttpContextFactory
    {
        HttpContextBase CreateContext();
    }
}
