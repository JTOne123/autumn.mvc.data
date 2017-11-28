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
                var path = key.Replace("{id}", string.Empty).TrimEnd('/');
                if (!AutumnApplication.Current.IgnoresPaths.ContainsKey(path)) continue;
                var ignore = ((int) AutumnApplication.Current.IgnoresPaths[path]).ToString().PadLeft(3, '0');
                pathItem.Post = ignore[0] == '1' ? null : pathItem.Post;
                pathItem.Put = ignore[1] == '1' ? null : pathItem.Put;
                pathItem.Delete = ignore[2] == '1' ? null : pathItem.Delete;
            }
        }
    }
}