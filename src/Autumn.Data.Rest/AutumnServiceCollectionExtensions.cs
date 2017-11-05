using Autumn.Data.Rest.Configurations;
using Autumn.Data.Rest.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Serialization;


namespace Microsoft.Extensions.DependencyInjection
{
    public static class AutumnServiceCollectionExtensions
    {
        public static void AddAutumn(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = BuildSettings(configuration);

            var mvcBuilder = services.AddMvc(config =>
            {
                config.ModelBinderProviders.Insert(0,
                    new PageableModelBinderProvider(configuration, settings.NamingStrategy));
                config.ModelBinderProviders.Insert(1,
                    new QueryModelBinderProvider(configuration, settings.NamingStrategy));
            });

            var contractResolver = new DefaultContractResolver() {NamingStrategy = settings.NamingStrategy};

            mvcBuilder.AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = contractResolver;
            });

            services.AddSingleton(settings);
        }

        private static AutumnSettings BuildSettings(IConfiguration configuration)
        {
            var result = new AutumnSettings()
            {
                ConnectionString = configuration.GetSection("Autumn.Data.Rest:ConnectionString").Value,
                DatabaseName = configuration.GetSection("Autumn.Data.Rest:DatabaseName").Value
            };

            var namingStrategySettings = configuration.GetSection("Autumn.Data.Rest:NamingStrategy").Value;
            if (!string.IsNullOrWhiteSpace(namingStrategySettings))
            {
                switch (namingStrategySettings.ToUpperInvariant())
                {
                    case "SNAKE_CASE":
                        result.NamingStrategy = new SnakeCaseNamingStrategy();
                        break;
                    case "CAMEL_CASE":
                        result.NamingStrategy = new CamelCaseNamingStrategy();
                        break;
                }
            }
            
            return result;
        }
    }
}
