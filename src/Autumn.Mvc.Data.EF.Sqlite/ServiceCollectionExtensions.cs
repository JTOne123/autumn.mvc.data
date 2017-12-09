using System;
using System.Linq;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.EF.Configuration;
using Autumn.Mvc.Data.EF.Repositories;
using Autumn.Mvc.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Autumn.Mvc.Data.EF.Sqlite
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAutumnEntityFrameworkCoreSqlite<TContext>(
            this IServiceCollection services,
            Action<AutumnEntityFrameworkCoreSettingsBuilder> autumnEntityFrameworkSettingsAction,
            Action<SqliteDbContextOptionsBuilder> sqliteOptionsAction = null)
            where TContext : DbContext
        {
            if(services==null) throw  new ArgumentNullException(nameof(services));
            if(autumnEntityFrameworkSettingsAction==null) throw  new ArgumentNullException(nameof(autumnEntityFrameworkSettingsAction));
            var service = services.Single(c =>
                c.ServiceType == typeof(AutumnDataSettings) && c.Lifetime == ServiceLifetime.Singleton);
            var dataSettings = (AutumnDataSettings) service.ImplementationInstance;
            
            var builder = new AutumnEntityFrameworkCoreSettingsBuilder(dataSettings);
            autumnEntityFrameworkSettingsAction(builder);
            var entityFrameworkCoreSettings = builder.Build();
            services.AddSingleton(entityFrameworkCoreSettings);
            
            services.AddDbContextPool<TContext>(o =>
            {
                o.UseSqlite(entityFrameworkCoreSettings.ConnectionString, sqliteOptionsAction);
            });

            services.AddScoped(typeof(DbContext), (s) => s.GetService(typeof(TContext)));
            services.AddScoped(typeof(ICrudPageableRepositoryAsync<,>),
                typeof(EntityFrameworkCrudPageableRepositoryAsync<,>));


            return services;
        }
    }
}