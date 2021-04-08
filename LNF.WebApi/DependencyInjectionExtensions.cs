using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using System.Web.Http;

namespace LNF.WebApi
{
    public static class DependencyInjectionExtensions
    {
        public static void Bootstrap(this HttpConfiguration config, Container container)
        {
            config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
        }
    }
}
