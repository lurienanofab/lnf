using LNF.DependencyInjection;
using LNF.Impl;
using LNF.Impl.DependencyInjection;
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
    public static class IOC
    {
        public static Container Container { get; set; }
    }

    public class WebApp
    {
        private readonly SimpleInjectorContainerContext _context;

        public WebApp()
        {
            var container = new Container();

            // Needed for SimpleInjector v5
            // See https://simpleinjector.readthedocs.io/en/latest/resolving-unregistered-concrete-types-Is-disallowed-by-default.html
            container.Options.ResolveUnregisteredConcreteTypes = true;

            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

            _context = new SimpleInjectorContainerContext(container);

            IOC.Container = container;
        }

        public IContainerContext Context => _context;

        public WebContainerConfiguration GetConfiguration()
        {
            return new WebContainerConfiguration(IOC.Container);
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
                var reg = Lifestyle.Transient.CreateRegistration(type, IOC.Container);
                reg.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "ASP.NET creates and disposes page classes for us.");
                IOC.Container.AddRegistration(type, reg);
            }
        }

        /// <summary>
        /// Sets up dependency injection including property injection on aspx WebForms and initializes ServiceProvider.
        /// Applications should register any additional types before calling this method.
        /// </summary>
        public void Bootstrap(params Assembly[] assemblies)
        {
            RegisterWebPages(assemblies);
            IOC.Container.Verify();
            ServiceProvider.Setup(IOC.Container.GetInstance<IProvider>());
        }

        /// <summary>
        /// Sets up dependency injection including constructor injection on MVC Controllers and initializes ServiceProvider.
        /// Applications should register any additional types before calling this method.
        /// </summary>
        public void BootstrapMvc(params Assembly[] assemblies)
        {
            IOC.Container.RegisterMvcControllers(assemblies);
            IOC.Container.Verify();
            ServiceProvider.Setup(IOC.Container.GetInstance<IProvider>());
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(IOC.Container));
        }
    }

    public abstract class PageInitializerModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.PreRequestHandlerExecute += (sender, e) =>
            {
                IHttpHandler handler = context.Context.CurrentHandler;
                if (handler != null)
                {
                    var name = handler.GetType().Assembly.FullName;
                    if (!name.StartsWith("System.Web") && !name.StartsWith("Microsoft"))
                    {
                        InitializeHandler(handler);
                    }
                }
            };
        }

        public static void RegisterModule(Type pageInitializerType)
        {
            DynamicModuleUtility.RegisterModule(pageInitializerType);
        }

        // This should be overridden and call a static method in Global.asax
        // (see https://docs.simpleinjector.org/en/latest/webformsintegration.html)
        protected virtual void InitializeHandler(IHttpHandler handler)
        {
            var handlerType = handler is Page ? handler.GetType().BaseType : handler.GetType();
            IOC.Container.GetRegistration(handlerType, true).Registration.InitializeInstance(handler);
        }

        public void Dispose() { }
    }
}
