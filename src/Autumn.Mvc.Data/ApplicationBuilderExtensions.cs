using System;
using System.Linq;
using Autumn.Mvc.Data.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace Autumn.Mvc.Data
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAutumn(this IApplicationBuilder app, ILoggerFactory loggerFactory = null)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            var result = app;
            result = result
                .UseMiddleware(typeof(ErrorHandlingMiddleware))
                .UseMiddleware(typeof(IgnoreOperationMiddleware));

            if (!AutumnApplication.Current.UseSwagger) return result;

            result = app.UseSwagger();
            result = result.UseSwaggerUI(c =>
            {
                foreach (var version in AutumnApplication.Current.EntitiesInfos.Values.Select(e => e.ApiVersion)
                    .Distinct())
                {
                    c.SwaggerEndpoint(string.Format("/swagger/{0}/swagger.json", version),
                        string.Format("API {0}", version));
                }
            });
            return result;
        }
    }
}