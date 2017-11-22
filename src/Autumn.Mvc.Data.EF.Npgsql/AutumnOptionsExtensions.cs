using System;
using Autumn.Mvc.Data.EF.Repositories;
using Autumn.Mvc.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Autumn.Mvc.Data.EF.Npgsql
{
    public static class AutumnOptionsExtensions
    {
        public static AutumnOptions UseNpgsql<TContext>(this AutumnOptions options, string connectionString,
            Action<NpgsqlDbContextOptionsBuilder> npgsqlOptionsAction = null)
            where TContext : DbContext
        {
            options.ServiceCollection.AddEntityFrameworkNpgsql()
                .AddDbContext<TContext>(o => o.UseNpgsql(connectionString, npgsqlOptionsAction));
            options.ServiceCollection.AddScoped(typeof(DbContext), (s) => s.GetService(typeof(TContext)));

            options.ServiceCollection.AddScoped(typeof(ICrudPageableRepositoryAsync<,>),
                typeof(EntityFrameworkCrudPageableRepositoryAsync<,>));

            return options;
        }
    }
}