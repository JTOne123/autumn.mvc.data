﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Autumn.Data.Mvc.EntityFramework.Repositories;
using Autumn.Data.Mvc.EntityFramework.Samples.Models;
using Autumn.Mvc.Data;
using Autumn.Mvc.Data.Repositories;
using Autumn.Mvc.Data.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Autumn.Data.Mvc.EntityFramework.Samples
{
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

            services.AddDbContextPool<ChinookContext>(options =>
            {
                options.UseSqlServer(_configuration.GetSection("Autumn.Data.Mvc:EntityFrameworkCore:ConnectionString")
                    .Value);
                
                
            });

            services.AddScoped(typeof(DbContext), (s) => s.GetService(typeof(ChinookContext))    );
            
            services.AddScoped(typeof(ICrudPageableRepositoryAsync<,>),
                typeof(EntityFrameworkCrudPageableRepositoryAsync<,>));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {

            const string commandText = @"
                    USE master;

                    IF NOT EXISTS (SELECT name FROM master.dbo.sysdatabases WHERE name = N'Chinook')
                        BEGIN
	                        CREATE DATABASE [Chinook];
                        END";


            var connectionString =
                _configuration.GetSection("Autumn.Data.Mvc:EntityFrameworkCore:ConnectionString").Value;
            var databaseName = Regex.Match(connectionString, @".*Database\s*=\s*([^;]+);.*", RegexOptions.IgnoreCase)
                .Groups[1].Value;

            using (var connection = new SqlConnection(connectionString.Replace(databaseName, "master")))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    command.ExecuteNonQuery();
                }
            }

            using (var connection = new SqlConnection((connectionString)))
            {
                var evolve = new Evolve.Evolve(connection, msg => Console.WriteLine(msg))
                {
                    Locations = new List<string>() {"Scripts"},
                    MustEraseOnValidationError = true
                };

                evolve.Migrate();
            }





            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }
            app
                .UseAutumn(_hostingEnvironment, ApplicationLogging.LoggerFactory)
                .UseMvc();
        }
    }
}