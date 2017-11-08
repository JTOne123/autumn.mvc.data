using System;
using Autumn.Mvc.Data.Configurations;
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
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                foreach (var version in AutumnSettings.Instance.ApiVersions)
                {
                    c.SwaggerEndpoint(string.Format("/swagger/{0}/swagger.json", version),
                        string.Format("API {0}", version));
                }
            });
            return app.UseMiddleware(typeof(ErrorHandlingMiddleware))
            .UseMvc();
        }
    }
}