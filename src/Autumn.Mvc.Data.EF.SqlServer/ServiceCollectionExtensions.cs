using System;
using System.Data.SqlClient;
using System.Linq;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.EF.Configuration;
using Autumn.Mvc.Data.EF.Repositories;
using Autumn.Mvc.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Autumn.Mvc.Data.EF.SqlServer
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutumnEntityFrameworkCoreSqlServer<TContext>(
            this IServiceCollection services,
            Action<EntityFrameworkCoreSettingsBuilder> autumnSqlServerOptionsAction,
            Action<SqlServerDbContextOptionsBuilder> sqlServerOptionsAction = null,
            ILoggerFactory loggerFactory = null)
            where TContext : DbContext
        {
            
            var service = services.Single(c =>
                c.ServiceType == typeof(AutumnDataSettings) && c.Lifetime == ServiceLifetime.Singleton);
            var dataSettings = (AutumnDataSettings) service.ImplementationInstance;
            
            var builder = new EntityFrameworkCoreSettingsBuilder(dataSettings);
            autumnSqlServerOptionsAction(builder);
            var entityFrameworkCoreSettings = builder.Build();
            services.AddSingleton(entityFrameworkCoreSettings);
            
            if (entityFrameworkCoreSettings.UseEvolve)
            {
                var logger = loggerFactory?.CreateLogger("Evolve");
                Action<string> log = Console.WriteLine;
                if (logger != null)
                {
                    log = (e) =>
                    {
                        logger.LogInformation(e);
                    };
                }

                using (var connection = new SqlConnection((entityFrameworkCoreSettings.ConnectionString)))
                {
                    var evolve = new Evolve.Evolve(connection, log);
                    evolve.Migrate();
                }
            }

            services.AddDbContextPool<TContext>(o =>
            {
                o.UseSqlServer(entityFrameworkCoreSettings.ConnectionString, sqlServerOptionsAction);
            });

            services.AddScoped(typeof(DbContext), (s) => s.GetService(typeof(TContext)));

            services.AddScoped(typeof(ICrudPageableRepositoryAsync<,>),
                typeof(EntityFrameworkCrudPageableRepositoryAsync<,>));

            return services;
        }
    }
}