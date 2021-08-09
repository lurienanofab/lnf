using LNF.DependencyInjection;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using SimpleInjector.Lifestyles;
using System.Web.Http;

namespace LNF.WebApi
{
    public class WebApp
    {
        public Container Container { get; }

        private WebApp()
        {
            Container = new Container();
            Container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
        }

        public IContainerContext Context { get; }

        /// <summary>
        /// Sets up dependency injection including constructor injection on WebApi Controllers and initializes ServiceProvider.
        /// Applications should register any additional types before calling this method.
        /// </summary>
        public void BootstrapWebApi(HttpConfiguration config)
        {
            Container.RegisterWebApiControllers(config);
            Container.Verify();
            ServiceProvider.Setup(Container.GetInstance<IProvider>());
            config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(Container);
        }
    }
}
