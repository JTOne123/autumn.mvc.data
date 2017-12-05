using System;
using Autumn.Mvc.Data.EF.Repositories;
using Autumn.Mvc.Data.EF.Sqlite.Configuration;
using Autumn.Mvc.Data.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Autumn.Mvc.Data.EF.Sqlite
{
    public static class AutumnServiceCollectionExtensions
    {
        public static IServiceCollection AddAutumnEntityFrameworkCoreNgsql<TContext>(
            this IServiceCollection serviceCollection,
            Action<AutumnEntityFrameworkCoreSqliteSettingsBuilder> autumnSqliteOptionsAction,
            Action<SqliteDbContextOptionsBuilder> sqliteOptionsAction = null,
            ILoggerFactory loggerFactory = null)
            where TContext : DbContext
        {
            var builder = new AutumnEntityFrameworkCoreSqliteSettingsBuilder();
            autumnSqliteOptionsAction(builder);
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

                using (var connection = new SqliteConnection(settings.ConnectionString))
                {
                    var evolve = new Evolve.Evolve(connection, log);
                    evolve.Migrate();
                }
            }

            serviceCollection.AddDbContextPool<TContext>(o =>
            {
                o.UseSqlite(settings.ConnectionString, sqliteOptionsAction);
            });

            serviceCollection.AddScoped(typeof(DbContext), (s) => s.GetService(typeof(TContext)));
            serviceCollection.AddScoped(typeof(IAutumnCrudPageableRepositoryAsync<,>),
                typeof(EntityFrameworkCrudPageableRepositoryAsync<,>));


            return serviceCollection;
        }
    }
}