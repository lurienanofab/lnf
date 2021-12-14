using LNF.DependencyInjection;
using SimpleInjector;
using SimpleInjector.Diagnostics;
using System;

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

        public void RegisterSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            Container.Register<TService, TImplementation>(Lifestyle.Singleton);
        }

        public void RegisterSingleton<TService>(Func<TService> instanceCreator)
            where TService : class
        {
            Container.Register(instanceCreator, Lifestyle.Singleton);
        }

        public void RegisterDisposableTransient(Type type, string justification)
        {
            var registration = Lifestyle.Transient.CreateRegistration(type, Container);
            Container.AddRegistration(type, registration);
            registration.SuppressDiagnosticWarning(DiagnosticType.DisposableTransientComponent, justification);
        }

        public void EnablePropertyInjection()
        {
            Container.Options.PropertySelectionBehavior = new InjectPropertySelectionBehavior();
        }

        public bool IsLocked() => Container.IsLocked;
    }
}
