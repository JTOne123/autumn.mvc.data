using Autumn.Data.Rest.Configurations;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AutumnServiceCollectionExtensions
    {
        public static void AddAutumn(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = new AutumnSettings()
            {
                ConnectionString = configuration.GetSection("Autumn.Data.Rest:ConnectionString").Value,
                DatabaseName = configuration.GetSection("Autumn.Data.Rest:DatabaseName").Value
            };

            services.AddSingleton(settings);
        }
    }
}