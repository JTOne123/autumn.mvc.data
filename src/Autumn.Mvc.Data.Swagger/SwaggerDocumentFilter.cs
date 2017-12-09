using System;
using System.Linq;
using System.Net.Http;
using Autumn.Mvc.Configurations;
using Autumn.Mvc.Data.Configurations;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Autumn.Mvc.Data.Swagger
{
    public class SwaggerDocumentFilter : IDocumentFilter
    {
        private readonly AutumnSettings _settings;

        public SwaggerDocumentFilter(AutumnSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }
        
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var key in swaggerDoc.Paths.Keys)
            {
                var pathItem = swaggerDoc.Paths[key];
                var path = key.Replace("{id}", string.Empty).TrimEnd('/').TrimStart('/');
                if (!_settings.DataSettings().IgnoreOperations.ContainsKey(path)) continue;
                var ignores = _settings.DataSettings().IgnoreOperations[path];
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