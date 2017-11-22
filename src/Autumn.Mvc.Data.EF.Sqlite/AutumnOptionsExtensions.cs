using Autumn.Mvc.Data.EF.Repositories;
using Autumn.Mvc.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Autumn.Mvc.Data.EF.Sqlite
{
    public static class AutumnOptionsExtensions
    {
        public static AutumnOptions UseSqlite<TContext>(this AutumnOptions options, string connectionString)
            where TContext : DbContext
        {
            options.ServiceCollection
                .AddEntityFrameworkSqlite().AddDbContext<TContext>((serviceProvider, o) =>
                    o.UseSqlite(connectionString)
                        .UseInternalServiceProvider(serviceProvider));
            options.ServiceCollection.AddScoped(typeof(DbContext), (s) => s.GetService(typeof(TContext)));
            options.ServiceCollection.AddScoped(typeof(ICrudPageableRepositoryAsync<,>),
                typeof(EntityFrameworkCrudPageableRepositoryAsync<,>));

            return options;
        }
    }
}