using System;
using Autumn.Mvc.Data.EF.Configuration;
using Autumn.Mvc.Data.EF.Sqlite.Samples.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace Autumn.Mvc.Data.EF.Sqlite.Samples
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAutumn(config =>
                    config.NamingStrategy(new CamelCaseNamingStrategy())
                )
                .AddAutumnData(
                    config => config.ApiVersion("v0"))
                .AddAutumnEntityFrameworkCoreSqlite<ChinookContext>(config =>
                    config
                        .ConnectionString("Data Source=chinook.db3")
                );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseAutumnData()
                .UseMvc();

            var entityFrameworkCoreSettings =
                (EntityFrameworkCoreSettings) app.ApplicationServices.GetService(typeof(EntityFrameworkCoreSettings));
            var logger = loggerFactory?.CreateLogger("Evolve");
            Action<string> log = Console.WriteLine;
            if (logger != null)
            {
                log = (e) =>
                {
                    logger.LogInformation(e);
                };
            }
            using (var connection = new SqliteConnection(entityFrameworkCoreSettings.ConnectionString))
            {
                var evolve = new Evolve.Evolve(connection, log);
                evolve.Migrate();
            }
        }
    }
}