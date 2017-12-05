using System;
using System.Linq;
using Autumn.Mvc.Configurations;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace Autumn.Mvc.Data
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAutumnData(this IApplicationBuilder app)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            var settings = (AutumnSettings) app.ApplicationServices.GetService(typeof(AutumnSettings));
            app = app.UseMiddleware<IgnoreOperationMiddleware>(settings);
            app = app.UseSwagger();
            app = app.UseSwaggerUI(c =>
            {
                foreach (var version in settings.DataSettings().EntitiesInfos.Values.Select(e => e.ApiVersion)
                    .Distinct())
                {
                    c.SwaggerEndpoint(string.Format("/swagger/{0}/swagger.json", version),
                        string.Format("API {0}", version));
                }
            });
            return app;
        }
    }
}