using System;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.Middlewares;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Builder
{
    public static class AutumnApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseAutumn(this IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));

            var result = app;
            result = result
                .UseMiddleware(typeof(ErrorHandlingMiddleware));
           

            foreach (var configuration in AutumnSettings.Instance.AutoConfigurations)
            {
                configuration.Configure(app, env, loggerFactory);
            }
            
            return result;
        }
    }
}