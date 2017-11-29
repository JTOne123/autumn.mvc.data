using System;
using Autumn.Mvc.Data.EF.Npgsql.Configuration;
using Autumn.Mvc.Data.EF.Repositories;
using Autumn.Mvc.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Autumn.Mvc.Data.EF.Npgsql
{
    public static class AutumnServiceCollectionExtensions
    {
        public static IServiceCollection AddAutumnEntityFrameworkCoreNgsql<TContext>(
            this IServiceCollection serviceCollection,
            Action<AutumnEntityFrameworkCoreNpgsqlSettingsBuilder> autumnNpgsqlOptionsAction,
            Action<NpgsqlDbContextOptionsBuilder> npgsqlOptionsAction = null,
            ILoggerFactory loggerFactory = null)
            where TContext : DbContext
        {
            var builder = new AutumnEntityFrameworkCoreNpgsqlSettingsBuilder();
            autumnNpgsqlOptionsAction(builder);
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

                using (var connection = new NpgsqlConnection((settings.ConnectionString)))
                {
                    var evolve = new Evolve.Evolve(connection, log);
                    evolve.Migrate();
                }
            }

            serviceCollection.AddDbContextPool<TContext>(o =>
            {
                o.UseNpgsql(settings.ConnectionString, npgsqlOptionsAction);
            });

            serviceCollection.AddScoped(typeof(DbContext), (s) => s.GetService(typeof(TContext)));
            serviceCollection.AddScoped(typeof(IAutumnCrudPageableRepositoryAsync<,>),
                typeof(AutumnEntityFrameworkCrudPageableRepositoryAsync<,>));


            return serviceCollection;
        }
    }
}