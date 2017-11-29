using System.Linq;
using System.Net.Http;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Autumn.Mvc.Data.Swagger
{
    public class AutumnSwaggerDocumentFilter : IDocumentFilter
    {
        
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var key in swaggerDoc.Paths.Keys)
            {
                var pathItem = swaggerDoc.Paths[key];
                var path = key.Replace("{id}", string.Empty).TrimEnd('/').TrimStart('/');
                if (!AutumnApplication.Current.IgnoreOperations.ContainsKey(path)) continue;
                var ignores = AutumnApplication.Current.IgnoreOperations[path];
                if (ignores.Contains(HttpMethod.Post))
                {
                    pathItem.Post = null;
                }
                if (ignores.Contains(HttpMethod.Put))
                {
                    pathItem.Put = null;
                }
                if (ignores.Contains(HttpMethod.Delete))
                {
                    pathItem.Delete = null;
                }
            }
        }
    }
}