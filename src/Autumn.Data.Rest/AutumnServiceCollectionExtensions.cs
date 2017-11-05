using System.Reflection;
using Autumn.Data.Rest.Configurations;
using Autumn.Data.Rest.Controllers;
using Autumn.Data.Rest.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Serialization;


namespace Microsoft.Extensions.DependencyInjection
{
    public static class AutumnServiceCollectionExtensions
    {
        /// <summary>
        /// add autumn configuration
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddAutumn(this IServiceCollection services, IConfiguration configuration)
        {
            var settings = BuildSettings(configuration);
            RepositoryControllerNameConvention.Settings = settings;

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

            mvcBuilder.ConfigureApplicationPartManager(p =>
            {
                p.FeatureProviders.Add(new RespositoryControllerFeatureProvider(Assembly.GetEntryAssembly(), settings));
            });

            services.AddSingleton(settings);
        }

        /// <summary>
        /// build autumn settings
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private static AutumnSettings BuildSettings(IConfiguration configuration)
        {
            AutumnSettings.Instance.ConnectionString =
                configuration.GetSection("Autumn.Data.Rest:ConnectionString").Value;

            AutumnSettings.Instance.DatabaseName =
                configuration.GetSection("Autumn.Data.Rest:DatabaseName").Value;

            var namingStrategySettings = configuration.GetSection("Autumn.Data.Rest:NamingStrategy").Value;
            if (!string.IsNullOrWhiteSpace(namingStrategySettings))
            {
                if (namingStrategySettings.ToUpperInvariant() == "SNAKE_CASE")
                {
                    AutumnSettings.Instance.NamingStrategy = new SnakeCaseNamingStrategy();
                }
                else if (namingStrategySettings.ToUpperInvariant() == "CAMEL_CASE")
                {
                    AutumnSettings.Instance.NamingStrategy = new CamelCaseNamingStrategy();
                }
            }

            var defaultVersion = configuration.GetSection("Autumn.Data.Rest:ApiVersion").Value;
            if (!string.IsNullOrWhiteSpace(defaultVersion))
            {
                AutumnSettings.Instance.ApiVersion = defaultVersion;
            }

            var controllerPluralize = configuration.GetSection("Autumn.Data.Rest:PluralizeController").Value;
            if (!string.IsNullOrWhiteSpace(controllerPluralize))
            {
                AutumnSettings.Instance.PluralizeController = bool.Parse(controllerPluralize);
            }
            return AutumnSettings.Instance;
        }
    }
}
