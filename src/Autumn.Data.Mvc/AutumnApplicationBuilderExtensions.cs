using System;
using Autumn.Data.Mvc.Middlewares;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Builder
{
    public static class AutumnApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAutumn(this IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            return app.UseMiddleware(typeof(ErrorHandlingMiddleware));
        }
    }
}