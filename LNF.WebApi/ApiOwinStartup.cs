using System.Web.Http;

namespace LNF.WebApi
{
    public abstract class ApiOwinStartup
    {
        public virtual HttpConfiguration CreateConfiguration()
        {
            // WebApi setup (includes adding the Authorization filter)
            var config = new HttpConfiguration();
            WebApiConfig.Register(config);
            return config;
        }
    }
}
