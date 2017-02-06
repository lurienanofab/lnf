using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Web.Http.Description;

namespace LNF.WebApi.Swagger
{
    internal class AddApiKeySecurity : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            if (swaggerDoc.security == null)
                swaggerDoc.security = new List<IDictionary<string, IEnumerable<string>>>();
            
            swaggerDoc.security.Add(new Dictionary<string, IEnumerable<string>> { { "ApiKeySecurity", new string[] { } } });
        }
    }
}
