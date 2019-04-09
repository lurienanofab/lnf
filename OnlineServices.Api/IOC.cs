using LNF.Models;
using SimpleInjector;

namespace OnlineServices.Api
{
    public class IOC
    {
        public static void Configure(Container container)
        {
            // Register your stuff here
            container.Register<IProvider, ServiceProvider>(Lifestyle.Scoped);
            container.Verify();
        }
    }
}
