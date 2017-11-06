using System;
using Autumn.Mvc.Data.Middlewares;
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