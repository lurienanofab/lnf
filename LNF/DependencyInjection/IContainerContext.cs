using System;

namespace LNF.DependencyInjection
{
    public interface IContainerContext
    {
        T GetInstance<T>() where T : class;

        void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;
        
        void RegisterSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;

        void RegisterSingleton<TService>(Func<TService> instanceCreator)
            where TService : class;

        void RegisterDisposableTransient(Type type, string justification);

        void EnablePropertyInjection();

        bool IsLocked();
    }
}
