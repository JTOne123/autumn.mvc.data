using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace Autumn.Mvc.Data.MongoDB.Samples
{
    public class Startup
    {
        public Startup(IHostingEnvironment env,IConfiguration configuration)
        {
            _configuration = configuration;
            _hostingEnvironment = env;
        }

        private IConfiguration _configuration;
        private IHostingEnvironment _hostingEnvironment;
       
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAutumn(config =>
                    config
                        .QueryFieldName("Search")
                        .NamingStrategy(new SnakeCaseNamingStrategy()))
                .AddAutumnData(config => config
                    .Swagger())
                .AddAutumnMongo(config =>
                    config
                        .ConnectionString(
                            "mongodb://jason.garnier-family.lan:27017,ulysse.garnier-family.lan:27017,achille.garnier-family.lan:27017?readPreference=primary&replicaSet=rs0")
                        .Database("sample")
                );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseAutumnData()
                .UseMvc();
        }
    }
}