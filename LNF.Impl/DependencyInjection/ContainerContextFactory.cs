using LNF.DependencyInjection;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Lifestyles;
using System;

namespace LNF.Impl.DependencyInjection
{
    public class ContainerContextFactory
    {
        public static ContainerContextFactory Current { get; }

        static ContainerContextFactory()
        {
            Current = new ContainerContextFactory();
        }

        public string Lifestyle => _lifestyle;

        private string _lifestyle;
        private SimpleInjectorContainerContext _context;
        
        private ContainerContextFactory() { }

        public bool ContextExists()
        {
            return _context != null;
        }

        public SimpleInjectorContainerContext GetContext()
        {
            if (!ContextExists())
                throw new Exception("One of the methods (NewWebRequestContext, NewAsyncScopedContext, NewThreadScopedContext) must be called first.");

            return _context;
        }

        public void NewThreadScopedContext()
        {
            Container container;

            if (ContextExists())
                throw new Exception($"Context already exists. Lifestyle = {_lifestyle}");
            else
            {
                _lifestyle = "ThreadScoped";
                container = new Container();
                container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();
            }

            _context = new SimpleInjectorContainerContext(container);
        }

        public void NewWebRequestContext()
        {
            Container container;

            if (ContextExists())
                throw new Exception($"Container already exists. Lifestyle = {_lifestyle}");
            else
            {
                _lifestyle = "WebRequest";
                container = new Container();

                // Needed for SimpleInjector v5
                // See https://simpleinjector.readthedocs.io/en/latest/resolving-unregistered-concrete-types-Is-disallowed-by-default.html
                container.Options.ResolveUnregisteredConcreteTypes = true;

                container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();
            }

            _context = new SimpleInjectorContainerContext(container);
        }

        public void NewAsyncScopedContext()
        {
            Container container;

            if (ContextExists())
                throw new Exception($"Container already exists. Lifestyle = {_lifestyle}");
            else
            {
                _lifestyle = "AsyncScoped";
                container = new Container();

                container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            }

            _context = new SimpleInjectorContainerContext(container);
        }
    }
}
