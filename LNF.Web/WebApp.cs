using LNF.DependencyInjection;
using LNF.Impl.DependencyInjection;
using SimpleInjector;
using SimpleInjector.Diagnostics;
using SimpleInjector.Integration.Web.Mvc;
using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.UI;

namespace LNF.Web
{
    public class WebApp
    {
        private readonly SimpleInjectorContainerContext _context;

        public WebApp()
        {
            _context = ContainerContextFactory.Current.NewWebRequestContext();
        }

        public IContainerContext Context => _context;

        public WebContainerConfiguration GetConfiguration()
        {
            return new WebContainerConfiguration(_context);
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
                var reg = Lifestyle.Transient.CreateRegistration(type, _context.Container);
                reg.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, "ASP.NET creates and disposes page classes for us.");
                _context.Container.AddRegistration(type, reg);
            }
        }

        /// <summary>
        /// Sets up dependency injection including property injection on aspx WebForms and initializes ServiceProvider.
        /// Applications should register any additional types before calling this method.
        /// </summary>
        public void Bootstrap(params Assembly[] assemblies)
        {
            RegisterWebPages(assemblies);
            _context.Container.Verify();
            ServiceProvider.Setup(_context.Container.GetInstance<IProvider>());
        }

        /// <summary>
        /// Sets up dependency injection including constructor injection on MVC Controllers and initializes ServiceProvider.
        /// Applications should register any additional types before calling this method.
        /// </summary>
        public void BootstrapMvc(params Assembly[] assemblies)
        {
            _context.Container.RegisterMvcControllers(assemblies);
            _context.Container.Verify();
            ServiceProvider.Setup(_context.Container.GetInstance<IProvider>());
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(_context.Container));
        }

        public Container GetContainer() => _context.Container;
    }
}
