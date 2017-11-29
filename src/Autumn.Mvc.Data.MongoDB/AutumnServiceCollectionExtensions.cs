using System;
using Autumn.Mvc.Data.MongoDB.Configurations;
using Autumn.Mvc.Data.MongoDB.Repositories;
using Autumn.Mvc.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Autumn.Mvc.Data.MongoDB
{
    public static class AutumnServiceCollectionExtensions
    {
        public static IServiceCollection AddAutumnMongo(
            this IServiceCollection serviceCollection,
            Action<AutumnMongoDBSettingsBuilder> autumnMongoDbOptionsAction,
            ILoggerFactory loggerFactory = null)
        {
            var builder = new AutumnMongoDBSettingsBuilder();
            autumnMongoDbOptionsAction(builder);
            var settings = builder.Build();

            serviceCollection.AddSingleton(settings);

            serviceCollection.AddScoped(typeof(IAutumnCrudPageableRepositoryAsync<,>),
                typeof(AutumnMongoDBCrudPageableRepositoryAsync<,>));

            return serviceCollection;
        }
    }
}