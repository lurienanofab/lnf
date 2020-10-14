using LNF.DataAccess;
using LNF.Impl;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using SimpleInjector;
using SimpleInjector.Diagnostics;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace LNF.Web
{
    public class WebApp
    {
        public static WebApp Current { get; private set; }

        static WebApp()
        {
            Current = new WebApp();
        }

        private Container _container;
        private bool _dataAccessRegistered = false;

        private WebApp()
        {
            _container = new Container();

            // Needed for SimpleInjector v5
            // See https://simpleinjector.readthedocs.io/en/latest/resolving-unregistered-concrete-types-Is-disallowed-by-default.html
            _container.Options.ResolveUnregisteredConcreteTypes = true;

            _container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();
        }

        public Container GetContainer()
        {
            return _container;
        }

        public void EnablePropertyInjection()
        {
            _container.Options.PropertySelectionBehavior = new InjectPropertySelectionBehavior();
        }

        public void RegisterWebPages(Assembly[] assemblies)
        {
            var pageTypes =
                from assembly in assemblies
                where !assembly.IsDynamic
                where !assembly.GlobalAssemblyCache
                from type in assembly.GetExportedTypes()
                where type.IsSubclassOf(typeof(Page)) || type.IsSubclassOf(typeof(MasterPage))
                where !type.IsAbstract && !type.IsGenericType
                select type;

            foreach (Type type in pageTypes)
            {
                var reg = Lifestyle.Transient.CreateRegistration(type, _container);
                reg.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "ASP.NET creates and disposes page classes for us.");
                _container.AddRegistration(type, reg);
            }
        }

        public void RegisterMvcControllers(Assembly[] assemblies)
        {
            _container.RegisterMvcControllers(assemblies.ToArray());
        }

        public void Configure()
        {
            var cfg = new WebContainerConfiguration(_container);
            cfg.SkipDataAccessRegistration = _dataAccessRegistered;
            cfg.Configure();
        }

        public void InitializeHandler(IHttpHandler handler)
        {
            var handlerType = handler is Page ? handler.GetType().BaseType : handler.GetType();
            _container.GetRegistration(handlerType, true).Registration.InitializeInstance(handler);
        }

        public T GetInstance<T>() where T : class
        {
            return _container.GetInstance<T>();
        }

        public void RegisterDataAccess<TImplementation>() where TImplementation : class, IDataAccessService
        {
            _container.RegisterSingleton<IDataAccessService, TImplementation>();
            _dataAccessRegistered = true;
        }

        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            _container.Register<TService, TImplementation>();
        }

        /// <summary>
        /// Sets up dependency injection including property injection on aspx WebForms and initializes ServiceProvider.
        /// Applications should register any additional types before calling this method.
        /// </summary>
        public void Bootstrap(params Assembly[] assemblies)
        {
            EnablePropertyInjection();
            Configure();
            RegisterWebPages(assemblies);
            _container.Verify();
            ServiceProvider.Setup(_container.GetInstance<IProvider>());
        }

        /// <summary>
        /// Sets up dependency injection including constructor injection on MVC Controllers and initializes ServiceProvider.
        /// Applications should register any additional types before calling this method.
        /// </summary>
        public void BootstrapMvc(params Assembly[] assemblies)
        {
            Configure();
            RegisterMvcControllers(assemblies);
            _container.Verify();
            ServiceProvider.Setup(_container.GetInstance<IProvider>());
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(_container));
        }
    }

    public class PageInitializerModule : IHttpModule
    {
        public static void Initialize()
        {
            DynamicModuleUtility.RegisterModule(typeof(PageInitializerModule));
        }

        public void Init(HttpApplication context)
        {
            context.PreRequestHandlerExecute += (sender, e) =>
            {
                IHttpHandler handler = context.Context.CurrentHandler;
                if (handler != null)
                {
                    var name = handler.GetType().Assembly.FullName;
                    if (!name.StartsWith("System.Web") && !name.StartsWith("Microsoft"))
                        WebApp.Current.InitializeHandler(handler);
                }
            };
        }

        public void Dispose() { }
    }
}
