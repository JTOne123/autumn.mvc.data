using System;
using Autumn.Mvc.Data.EF.Configuration;
using Autumn.Mvc.Data.EF.Mysql.Samples.Models;
using Autumn.Mvc.Data.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Swashbuckle.AspNetCore.Swagger;

namespace Autumn.Mvc.Data.EF.Mysql.Samples
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddAutumn(config =>
                    config
                        .QueryFieldName("search"))
                .AddAutumnData()
                .AddAutumnEntityFrameworkCoreMysql<ChinookContext>(config =>
                    config
                        .ConnectionString("server=localhost;database=chinook;port=3306;Uid=chinook;password=@utUmn_mvc_D@t@!")
                        
                ).AddSwaggerGen(c =>
                {
                 
                    foreach (var version in services.GetAutumnDataSettings().ApiVersions)
                    {
                        c.SwaggerDoc(version, new Info {Title = "api", Version = version});
                    }
                    c.DocumentFilter<SwaggerDocumentFilter>();
                    c.OperationFilter<SwaggerOperationFilter>();

                });
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
                .UseSwagger()
                .UseSwaggerUI(c =>
                {
                    foreach (var version in app.GetAutumnDataSettings().ApiVersions)
                    {
                        c.SwaggerEndpoint(string.Format("/swagger/{0}/swagger.json", version),
                            string.Format("API {0}", version));
                    }
                })
                .UseMvc();

            var entityFrameworkCoreSettings = app.GetAutumnEntityFrameworkCoreSettings();
            
            var logger = loggerFactory?.CreateLogger("Evolve");
            Action<string> log = Console.WriteLine;
            if (logger != null)
            {
                log = (e) =>
                {
                    logger.LogInformation(e);
                };
            }

            using (var connection = new MySqlConnection(entityFrameworkCoreSettings.ConnectionString))
            {
                #region bug Evolve   

                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"CREATE TABLE IF NOT EXISTS `changelog` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `type` tinyint(4) unsigned  DEFAULT NULL,
  `version` varchar(50) DEFAULT NULL,
  `description` varchar(200) NOT NULL,
  `name` varchar(300) NOT NULL,
  `checksum` varchar(32) DEFAULT NULL,
  `installed_by` varchar(100) NOT NULL,
  `installed_on` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `success` tinyint(1) NOT NULL,
  PRIMARY KEY (`id`)
) ";
                    command.ExecuteNonQuery();
                }

                #endregion

                var evolve = new Evolve.Evolve(connection, log);
                evolve.Migrate();
            }
        }
    }
}