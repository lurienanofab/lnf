using LNF.Impl.DependencyInjection.Web;
using System.Web;

namespace LNF.Impl
{
    public class ServiceModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            ServiceProvider.Current = IOC.Resolver.GetInstance<ServiceProvider>();
        }
        public void Dispose()
        {
            // nothing to do here...
        }
    }
}
