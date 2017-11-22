using Autumn.Mvc.Data.MongoDB.Configurations;
using Autumn.Mvc.Data.MongoDB.Repositories;
using Autumn.Mvc.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Autumn.Mvc.Data.MongoDB
{
    public static class AutumnOptionsExtensions
    {
        public static AutumnOptions UseMongo(this AutumnOptions options, string database, string connectionString)
        {
            options.ServiceCollection.AddSingleton(new AutumnMongoSettings()
            {
                ConnectionString = connectionString,
                DatabaseName = database
            });

            options.ServiceCollection.AddScoped(typeof(ICrudPageableRepositoryAsync<,>),
                typeof(MongoCrudPageableRepositoryAsync<,>));

            return options;
        }
    }
}