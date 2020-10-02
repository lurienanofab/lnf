using Owin;
using System.Web.Http;

namespace LNF.WebApi
{
    public abstract class ApiOwinStartup
    {
        public virtual void Configuration(IAppBuilder app)
        {
            // WebApi setup (includes adding the Authorization filter)
            var config = new HttpConfiguration();
            WebApiConfig.Register(config);          
            app.UseWebApi(config);
        }
    }
}
