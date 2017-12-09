using System;
using System.Linq;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.MongoDB.Configurations;
using Autumn.Mvc.Data.MongoDB.Repositories;
using Autumn.Mvc.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Autumn.Mvc.Data.MongoDB
{
    public static class AutumnServiceCollectionExtensions
    {
        public static IServiceCollection AddAutumnMongo(
            this IServiceCollection services,
            Action<AutumnMongoDBSettingsBuilder> mongoDbSettingsAction)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (mongoDbSettingsAction == null) throw new ArgumentNullException(nameof(mongoDbSettingsAction));
            
            var builder = new AutumnMongoDBSettingsBuilder(services.GetAutumnDataSettings());
            mongoDbSettingsAction(builder);
            var settings = builder.Build();

            services.AddSingleton(settings);

            services.AddScoped(typeof(ICrudPageableRepositoryAsync<,>),
                typeof(MongoDBCrudPageableRepositoryAsync<,>));

            return services;
        }
    }
}