using System;
using Autumn.Data.Rest.Middlewares;

namespace Microsoft.AspNetCore.Builder
{
    public static class AutumnApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAutumn(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            return app.UseMiddleware(typeof(ErrorHandlingMiddleware));
        }
    }
}