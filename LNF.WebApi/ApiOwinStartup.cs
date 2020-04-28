using LNF.DataAccess;
using LNF.Impl;
using Owin;
using System.Web.Http;

namespace LNF.WebApi
{
    public abstract class ApiOwinStartup
    {
        protected HttpConfiguration config;
        protected DependencyResolver resolver;

        public virtual void Configuration(IAppBuilder app)
        {
            resolver = new WebResolver();
            ServiceProvider.Setup(resolver.GetInstance<IProvider>());

            // Data Access setup
            app.Use(async (ctx, next) =>
            {
                using (resolver.GetInstance<IUnitOfWork>())
                    await next.Invoke();
            });

            // WebApi setup (includes adding the Authorization filter)
            config = new HttpConfiguration();
            WebApiConfig.Register(config);          

            app.UseWebApi(config);
        }
    }
}
