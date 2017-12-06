using System;
using System.Linq;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.EF.Configuration;
using Autumn.Mvc.Data.EF.Repositories;
using Autumn.Mvc.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Autumn.Mvc.Data.EF.Npgsql
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutumnEntityFrameworkCoreNgsql<TContext>(
            this IServiceCollection services,
            Action<EntityFrameworkCoreSettingsBuilder> autumnEntityFrameworkSettingsAction,
            Action<NpgsqlDbContextOptionsBuilder> npgsqlOptionsAction = null,
            ILoggerFactory loggerFactory = null)
            where TContext : DbContext
        {
            if( services==null) throw  new ArgumentNullException(nameof(services));
            if(autumnEntityFrameworkSettingsAction==null) throw  new ArgumentNullException(nameof(autumnEntityFrameworkSettingsAction));
            var service = services.Single(c =>
                c.ServiceType == typeof(AutumnDataSettings) && c.Lifetime == ServiceLifetime.Singleton);
            var dataSettings = (AutumnDataSettings) service.ImplementationInstance;
            
            var builder = new EntityFrameworkCoreSettingsBuilder(dataSettings);
            autumnEntityFrameworkSettingsAction(builder);
            var settings = builder.Build();

            if (settings.UseEvolve)
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

                using (var connection = new NpgsqlConnection(settings.ConnectionString))
                {
                    var evolve = new Evolve.Evolve(connection, log)
                    {
                        MustEraseOnValidationError = true
                    };
                    evolve.Migrate();
                }
            }

            services.AddDbContextPool<TContext>(o =>
            {
                o.UseNpgsql(settings.ConnectionString, npgsqlOptionsAction);
            });

            services.AddScoped(typeof(DbContext), (s) => s.GetService(typeof(TContext)));
            services.AddScoped(typeof(ICrudPageableRepositoryAsync<,>),
                typeof(EntityFrameworkCrudPageableRepositoryAsync<,>));


            return services;
        }
    }
}