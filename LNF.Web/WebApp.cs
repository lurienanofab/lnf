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
        public WebApp()
        {
            Container = new Container();

            // Needed for SimpleInjector v5
            // See https://simpleinjector.readthedocs.io/en/latest/resolving-unregistered-concrete-types-Is-disallowed-by-default.html
            Container.Options.ResolveUnregisteredConcreteTypes = true;

            Container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

            if (Container.IsLocked)
                throw new Exception("Container is locked");
        }

        public Container Container { get; }

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
                var reg = Lifestyle.Transient.CreateRegistration(type, Container);
                reg.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "ASP.NET creates and disposes page classes for us.");
                Container.AddRegistration(type, reg);
            }
        }

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
        /// Sets up dependency injection including property injection on aspx WebForms and initializes ServiceProvider.
        /// Applications should register any additional types before calling this method.
        /// </summary>
        public void Bootstrap(params Assembly[] assemblies)
        {
            RegisterWebPages(assemblies);
            Container.Verify();
            ServiceProvider.Setup(Container.GetInstance<IProvider>());
        }

        /// <summary>
        /// Sets up dependency injection including constructor injection on MVC Controllers and initializes ServiceProvider.
        /// Applications should register any additional types before calling this method.
        /// </summary>
        public void BootstrapMvc(params Assembly[] assemblies)
        {
            Container.RegisterMvcControllers(assemblies);
            Container.Verify();
            ServiceProvider.Setup(Container.GetInstance<IProvider>());
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(Container));
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
                        InitializeHandler(handler);
                }
            };
        }

        public static void RegisterModule(Type pageInitializerType)
        {
            DynamicModuleUtility.RegisterModule(pageInitializerType);
        }

        // This should be overridden and call a static method in Global.asax
        // (see https://docs.simpleinjector.org/en/latest/webformsintegration.html)
        protected abstract void InitializeHandler(IHttpHandler handler);

        protected void ConfigureHandler(IHttpHandler handler, Container container)
        {
            var handlerType = handler is Page ? handler.GetType().BaseType : handler.GetType();
            container.GetRegistration(handlerType, true).Registration.InitializeInstance(handler);
        }

        public void Dispose() { }
    }
}
