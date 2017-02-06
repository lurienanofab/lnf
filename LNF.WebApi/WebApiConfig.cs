using LNF.WebApi.Swagger;
using Newtonsoft.Json.Serialization;
using System.Web.Http;

namespace LNF.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Add routes
            config.MapHttpAttributeRoutes();

            // Allow cross-origin
            var cors = new CustomCorsPolicyAttribute();
            config.EnableCors(cors);

            // Add authorize filter
            config.Filters.Add(new ApiAuthorizeAttribute());

            // Always show error details
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            // This will make is so the [Serializable] attribute will be ignored. Some classes need this
            // attribute so they can be saved in the session, but it causes the property names to be munged.
            // [http://stackoverflow.com/questions/12334382/net-webapi-serialization-k-backingfield-nastiness#22486064]
            var serializerSettings = config.Formatters.JsonFormatter.SerializerSettings;
            var contractResolver = (DefaultContractResolver)serializerSettings.ContractResolver;
            contractResolver.IgnoreSerializableAttribute = true;

            // Swagger is an API documentation tool
            SwaggerConfig.Register(config);
        }
    }
}
