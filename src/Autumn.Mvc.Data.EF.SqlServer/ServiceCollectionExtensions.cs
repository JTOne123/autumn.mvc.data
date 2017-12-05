using System;
using System.Data.SqlClient;
using Autumn.Mvc.Data.EF.Repositories;
using Autumn.Mvc.Data.EF.SqlServer.Configuration;
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
            this IServiceCollection serviceCollection,
            Action<EntityFrameworkCoreSqlServerSettingsBuilder> autumnSqlServerOptionsAction,
            Action<SqlServerDbContextOptionsBuilder> sqlServerOptionsAction = null,
            ILoggerFactory loggerFactory = null)
            where TContext : DbContext
        {
            var builder = new EntityFrameworkCoreSqlServerSettingsBuilder();
            autumnSqlServerOptionsAction(builder);
            var settings = builder.Build();

            if (settings.Evolve)
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

                using (var connection = new SqlConnection((settings.ConnectionString)))
                {
                    var evolve = new Evolve.Evolve(connection, log);
                    evolve.Migrate();
                }
            }

            serviceCollection.AddDbContextPool<TContext>(o =>
            {
                o.UseSqlServer(settings.ConnectionString, sqlServerOptionsAction);
            });

            serviceCollection.AddScoped(typeof(DbContext), (s) => s.GetService(typeof(TContext)));

            serviceCollection.AddScoped(typeof(ICrudPageableRepositoryAsync<,>),
                typeof(EntityFrameworkCrudPageableRepositoryAsync<,>));


            return serviceCollection;
        }
    }
}