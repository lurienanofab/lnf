using Swashbuckle.Swagger;
using System.Linq;
using System.Web.Http.Description;

namespace LNF.WebApi.Swagger
{
    internal class IgnoreEmptyPropertyDescriptions : IDocumentFilter
    {
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            if (swaggerDoc.definitions != null)
            {
                var emptyProps = swaggerDoc.definitions
                    .SelectMany(x => x.Value.properties)
                    .Where(x => x.Value.description == string.Empty)
                    .ToList();

                if (emptyProps.Count > 0)
                {
                    foreach (var p in emptyProps)
                        p.Value.description = null;
                }
            }
        }
    }
}
