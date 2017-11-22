using System;
using Autumn.Mvc.Data.EF.Repositories;
using Autumn.Mvc.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Autumn.Mvc.Data.EF.SqlServer
{
    public static class AutumnOptionsExtensions
    {
        public static AutumnOptions UseSqlServer<TContext>(this AutumnOptions options, string connectionString,
            Action<SqlServerDbContextOptionsBuilder> sqlServerOptionsAction = null)
            where TContext : DbContext
        {
            options.ServiceCollection.AddDbContextPool<TContext>(o =>
            {
                o.UseSqlServer(connectionString, sqlServerOptionsAction);
            });

            options.ServiceCollection.AddScoped(typeof(DbContext), (s) => s.GetService(typeof(TContext)));

            options.ServiceCollection.AddScoped(typeof(ICrudPageableRepositoryAsync<,>),
                typeof(EntityFrameworkCrudPageableRepositoryAsync<,>));

            return options;
        }
    }
}