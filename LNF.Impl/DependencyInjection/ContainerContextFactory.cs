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
        private Container _container;
        
        private ContainerContextFactory() { }

        public Container GetContainer()
        {
            if (_container == null)
                throw new Exception("Container is null. One of the methods (NewWebRequestContext, NewAsyncScopedContext, NewThreadScopedContext) must be called first.");

            return _container;
        }

        public SimpleInjectorContainerContext NewThreadScopedContext()
        {
            if (_container != null)
                throw new Exception($"Container already exists. Lifestyle = {_lifestyle}");
            else
            {
                _lifestyle = "ThreadScoped";

                _container = new Container();

                _container.Options.DefaultScopedLifestyle = new ThreadScopedLifestyle();
            }

            return new SimpleInjectorContainerContext(_container);
        }

        public SimpleInjectorContainerContext NewWebRequestContext()
        {
            if (_container != null)
                throw new Exception($"Container already exists. Lifestyle = {_lifestyle}");
            else
            {
                _lifestyle = "WebRequest";
                _container = new Container();

                // Needed for SimpleInjector v5
                // See https://simpleinjector.readthedocs.io/en/latest/resolving-unregistered-concrete-types-Is-disallowed-by-default.html
                _container.Options.ResolveUnregisteredConcreteTypes = true;

                _container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();
            }

            return new SimpleInjectorContainerContext(_container);
        }

        public SimpleInjectorContainerContext NewAsyncScopedContext()
        {
            if (_container != null)
                throw new Exception($"Container already exists. Lifestyle = {_lifestyle}");
            else
            {
                _lifestyle = "AsyncScoped";
                _container = new Container();

                _container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
            }

            return new SimpleInjectorContainerContext(_container);
        }
    }
}
