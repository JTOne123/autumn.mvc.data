using System;
using System.Reflection;
using Autumn.Mvc.Data;
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
            result = result.UseSwagger();
            result = result.UseSwaggerUI(c =>
            {
                foreach (var version in AutumnSettings.Instance.ApiVersions)
                {
                    c.SwaggerEndpoint(string.Format("/swagger/{0}/swagger.json", version),
                        string.Format("API {0}", version));
                }
            });

            result = result
                .UseMiddleware(typeof(ErrorHandlingMiddleware));

            foreach (var item in EnableAutoConfigurationAttribute.Configurations)
            {
                result = item.Configure(result, env, loggerFactory);
            }
            return result.UseMvc();
        }
    }
}