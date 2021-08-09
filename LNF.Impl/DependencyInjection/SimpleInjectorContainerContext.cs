using LNF.DependencyInjection;
using SimpleInjector;

namespace LNF.Impl.DependencyInjection
{
    public class SimpleInjectorContainerContext : IContainerContext
    {
        public Container Container { get; }

        public SimpleInjectorContainerContext(Container container)
        {
            Container = container;
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
    }
}
