using System.Linq;
using Autumn.Mvc.Data.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;

namespace Autumn.Mvc.Data.Swagger
{
    public class EnableAutoConfigurationSwaggerAttribute : EnableAutoConfigurationAttribute
    {
       
        public override void ConfigureServices(IServiceCollection serviceCollection, ILoggerFactory loggerFactory,
            IConfiguration configuration)
        {
            serviceCollection.AddSwaggerGen(c =>
            {
                foreach (var version in AutumnSettings.Instance.EntitiesInfos.Values.Select(e=>e.ApiVersion).Distinct())
                {
                    c.SwaggerDoc(version, new Info {Title = "api", Version = version});
                }
                c.OperationFilter<AutumnOperationFilter>();
            });
        }

        public override IApplicationBuilder Configure(IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            var result = app.UseSwagger();
            result = result.UseSwaggerUI(c =>
            {
                foreach (var version in AutumnSettings.Instance.EntitiesInfos.Values.Select(e=>e.ApiVersion).Distinct())
                {
                    c.SwaggerEndpoint(string.Format("/swagger/{0}/swagger.json", version),
                        string.Format("API {0}", version));
                }
            });
            return result;
        }
    }
}