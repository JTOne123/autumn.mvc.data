using System;
using Autumn.Mvc.Configurations;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.Middlewares;
using Microsoft.AspNetCore.Builder;

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
            return app;
        }

        public static AutumnDataSettings GetAutumnDataSettings(this IApplicationBuilder app)
        {
            return (AutumnDataSettings) app.ApplicationServices.GetService(typeof(AutumnDataSettings));
        }
    }
}