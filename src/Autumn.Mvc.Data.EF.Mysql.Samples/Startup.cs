using Autumn.Mvc.Data.EF.Mysql.Samples.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Autumn.Mvc.Data.EF.Mysql.Samples
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            _hostingEnvironment = env;
        }

        private IHostingEnvironment _hostingEnvironment;
       
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAutumn(options =>
                    options.Pluralized()
                        .Swagger())
                .AddAutumnEntityFrameworkCoreMysql<ChinookContext>(config =>
                    config
                        .ConnectionString("server=localhost;port=3306;database=chinook;uid=root;password=@utumn")
                        .Evolve()
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
                .UseAutumn(loggerFactory)
                .UseMvc();
        }
    }
}