using System;
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
            app = app.UseMiddleware<IgnoreOperationMiddleware>(app.GetAutumnSettings());
            return app;
        }

        public static AutumnDataSettings GetAutumnDataSettings(this IApplicationBuilder app)
        {
            return (AutumnDataSettings) app.ApplicationServices.GetService(typeof(AutumnDataSettings));
        }
    }
}