using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using SimpleInjector.Lifestyles;
using System.Web.Http;

namespace LNF.WebApi
{
    public class WebApp
    {
        public static WebApp Current { get; private set; }

        static WebApp()
        {
            Current = new WebApp();
        }

        private WebApp()
        {
            Container = new Container();
            Container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
        }

        public Container Container { get; }

        public T GetInstance<T>() where T : class
        {
            return Container.GetInstance<T>();
        }

        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            Container.Register<TService, TImplementation>();
        }

        /// <summary>
        /// Sets up dependency injection including constructor injection on WebApi Controllers and initializes ServiceProvider.
        /// Applications should register any additional types before calling this method.
        /// </summary>
        public void Bootstrap(HttpConfiguration config)
        {
            Container.RegisterWebApiControllers(config);
            Container.Verify();
            ServiceProvider.Setup(Container.GetInstance<IProvider>());
            config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(Container);
        }
    }
}
