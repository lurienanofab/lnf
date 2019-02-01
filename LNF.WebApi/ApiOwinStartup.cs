using LNF.Impl.DependencyInjection.Web;
using LNF.Repository;
using Owin;
using System.Web.Http;

namespace LNF.WebApi
{
    public abstract class ApiOwinStartup
    {
        protected HttpConfiguration config;

        public virtual void Configuration(IAppBuilder app)
        {
            ServiceProvider.Current = IOC.Resolver.GetInstance<ServiceProvider>();

            // Data Access setup
            app.Use(async (ctx, next) =>
            {
                using (DA.StartUnitOfWork())
                    await next.Invoke();
            });

            // WebApi setup (includes adding the Authorization filter)
            config = new HttpConfiguration();
            WebApiConfig.Register(config);          

            app.UseWebApi(config);
        }
    }
}
