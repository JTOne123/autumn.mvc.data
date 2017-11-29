using System;
using Autumn.Mvc.Data.EF.Mysql.Configuration;
using Autumn.Mvc.Data.EF.Repositories;
using Autumn.Mvc.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace Autumn.Mvc.Data.EF.Mysql
{
    public static class AutumnServiceCollectionExtensions
    {
        public static IServiceCollection AddAutumnEntityFrameworkCoreMysql<TContext>(
            this IServiceCollection serviceCollection,
            Action<AutumnEntityFrameworkCoreMysqlSettingsBuilder> autumnMySqlOptionsAction,
            Action<MySqlDbContextOptionsBuilder> mysqlOptionsAction = null,
            ILoggerFactory loggerFactory = null)
            where TContext : DbContext
        {
            var builder = new AutumnEntityFrameworkCoreMysqlSettingsBuilder();
            autumnMySqlOptionsAction(builder);
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

                using (var connection = new MySqlConnection(settings.ConnectionString))
                {
                    var evolve = new Evolve.Evolve(connection, log)
                    {
                        MustEraseOnValidationError = true
                    };
                    evolve.Migrate();
                }
            }

            serviceCollection.AddDbContextPool<TContext>(o =>
            {
                o.UseMySql(settings.ConnectionString, mysqlOptionsAction);
            });

            serviceCollection.AddScoped(typeof(DbContext), (s) => s.GetService(typeof(TContext)));
            serviceCollection.AddScoped(typeof(IAutumnCrudPageableRepositoryAsync<,>),
                typeof(AutumnEntityFrameworkCrudPageableRepositoryAsync<,>));


            return serviceCollection;
        }
    }
}