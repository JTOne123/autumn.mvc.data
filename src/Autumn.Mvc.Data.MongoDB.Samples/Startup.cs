using Autumn.Mvc.Data.MongoDB;
using Autumn.Mvc.Data.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Autumn.Mvc.Data.Samples
{
    [EnableAutoConfigurationMongo]
    [EnableAutoConfigurationSwagger]
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
            services.AddAutumn( _configuration,_hostingEnvironment);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseAutumn(env, loggerFactory)
                .UseMvc();
        }
    }
}