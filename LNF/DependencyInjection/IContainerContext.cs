namespace LNF.DependencyInjection
{
    public interface IContainerContext
    {
        T GetInstance<T>() where T : class;

        void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;
    }
}
