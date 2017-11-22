using System;
using Autumn.Mvc.Data.EF.Repositories;
using Autumn.Mvc.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.EntityFrameworkCore.Extensions;
using MySql.Data.EntityFrameworkCore.Infraestructure;

namespace Autumn.Mvc.Data.EF.Mysql
{
    public static class AutumnOptionsExtensions
    {
        public static AutumnOptions UseMySQL<TContext>(this AutumnOptions options, string connectionString,
            Action<MySQLDbContextOptionsBuilder> mysqlOptionsAction = null)
            where TContext : DbContext
        {
            options.ServiceCollection
                .AddEntityFrameworkMySQL()
                .AddDbContext<TContext>(o => o.UseMySQL(connectionString, mysqlOptionsAction));
            options.ServiceCollection.AddScoped(typeof(DbContext), (s) => s.GetService(typeof(TContext)));

            options.ServiceCollection.AddScoped(typeof(ICrudPageableRepositoryAsync<,>),
                typeof(EntityFrameworkCrudPageableRepositoryAsync<,>));

            return options;
        }
    }
}