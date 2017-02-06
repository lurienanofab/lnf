using Owin;
using System.Web.Http;

namespace LNF.WebApi
{
    public abstract class ApiOwinStartup
    {
        protected HttpConfiguration config;

        public virtual void Configuration(IAppBuilder app)
        {
            // Data Access setup
            app.Use(async (ctx, next) =>
            {
                using (Providers.DataAccess.StartUnitOfWork())
                    await next.Invoke();
            });

            // WebApi setup (includes adding the Authorization filter)
            config = new HttpConfiguration();
            WebApiConfig.Register(config);          

            app.UseWebApi(config);
        }
    }
}
