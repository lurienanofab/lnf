using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using System.Web.Http;

namespace LNF.WebApi
{
    public static class HttpConfigurationExtensions
    {
        /// <summary>
        /// Sets up dependency injection including constructor injection on WebApi Controllers and initializes ServiceProvider.
        /// Applications should register any additional types before calling this method.
        /// </summary>
        public static void BootstrapWebApi(this HttpConfiguration config, Container container)
        {
            container.RegisterWebApiControllers(config);
            container.Verify();
            ServiceProvider.Setup(container.GetInstance<IProvider>());
            config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
        }
    }
}
