using LNF;
using SimpleInjector;

namespace OnlineServices.Api
{
    public static class ContainerExtensions
    {
        public static void ConfigureDependencies(this Container container)
        {
            container.Register(() => ApiClient.NewRestClient());
            container.Register<IProvider, Provider>();
        }
    }
}
