using Autumn.Mvc.Data.MongoDB.Configurations;
using Autumn.Mvc.Data.MongoDB.Repositories;
using Autumn.Mvc.Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Autumn.Mvc.Data.MongoDB
{
    public class EnableAutoConfigurationMongoAttribute : EnableAutoConfigurationAttribute
    {
        public override void ConfigureServices(IServiceCollection serviceCollection, ILoggerFactory loggerFactory,
            IConfiguration configuration)
        {
           
            var connectionString = configuration.GetSection("Autumn.Data.Mvc:MongoDB:ConnectionString").Value;
            var databaseName = configuration.GetSection("Autumn.Data.Mvc:MongoDB:DatabaseName").Value;

            serviceCollection.AddSingleton(new AutumnMongoSettings()
            {
                ConnectionString = connectionString,
                DatabaseName = databaseName
            });
                
            serviceCollection.AddScoped(typeof(ICrudPageableRepositoryAsync<,>),
                typeof(MongoCrudPageableRepositoryAsync<,>));
        }
    }
}