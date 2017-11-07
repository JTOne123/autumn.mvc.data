using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autumn.Mvc.Data;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.Controllers;
using Autumn.Mvc.Data.Models;
using Autumn.Mvc.Data.Models.Helpers;
using Autumn.Mvc.Data.Models.Paginations;
using Autumn.Mvc.Data.Models.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace Microsoft.Extensions.DependencyInjection
{
    public static class AutumnServiceCollectionExtensions
    {

        private static ILogger _logger;

        
        private static string Logo()
        {
            return string.Format(@"               //               
             `+syo`             
     .`     `ossyys`     `.     
     /o+-` .osssyyys- `:oy/     
     +ssssosssssyyyyyyyyyyo     
    /sssssssssssyyyyyyyyyyy/    
 -/ossssssssssshdyyyyyyyyyyys+-           ___      __    __  .___________. __    __  .___  ___. .__   __. 
  :ossssssssyssNNyyhyyyyyyyys:`          /   \    |  |  |  | |           ||  |  |  | |   \/   | |  \ |  | 
    :osssssdNmhNNdmNdyyyyys:            /  ^  \   |  |  |  | `---|  |----`|  |  |  | |  \  /  | |   \|  |    
     .osssssydNNNNmhyyyyys.            /  /_\  \  |  |  |  |     |  |     |  |  |  | |  |\/|  | |  . `  | 
   `/ossssssssyNNhyyyyyyyyy/`         /  _____  \ |  `--'  |     |  |     |  `--'  | |  |  |  | |  |\   | 
  `/osssssssss+NNoyyyyyyyyys+`       /__/     \__\ \______/      |__|      \______/  |__|  |__| |__| \__| 
     .:ossss/..NN..oyyyyy/.     
       `+s/.  .NN.  .oyo`       
         `    .NN.    .         							Version : {0}
              .NN.","0.0.1");
        }

        /// <summary>
        /// add autumn configuration
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void AddAutumn(this IServiceCollection services, IConfiguration configuration)
        {
            _logger = ApplicationLogging.CreateLogger<AutumnSettings>();
            _logger.LogInformation(Logo());
            var settings = BuildSettings(configuration);
            settings.EntityAssembly = settings.EntityAssembly ?? Assembly.GetCallingAssembly();

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
                p.FeatureProviders.Add(new RespositoryControllerFeatureProvider(settings));
            });

            BuildRoutes(settings);

            services.AddSingleton(settings);
        }

        private static void BuildRoutes(AutumnSettings settings)
        {
            settings.Routes =new Dictionary<Type, AttributeRouteModel>();
            var baseType = typeof(RepositoryControllerAsync<,>);
            foreach (var type in settings.EntityAssembly.GetTypes())
            {
                var entityAttribute = type.GetCustomAttribute<EntityAttribute>();
                if (entityAttribute == null) continue;
                var idType = type.GetProperties().SingleOrDefault(p => p.GetCustomAttribute<IdAttribute>() != null)
                    ?.PropertyType;
                if (idType == null) continue;
                var name = entityAttribute.Name;
                switch (settings.NamingStrategy)
                {
                    case SnakeCaseNamingStrategy _:
                        name = name.ToSnakeCase();
                        break;
                    case CamelCaseNamingStrategy _:
                        name = name.ToCamelCase();
                        break;
                }
                if (settings.PluralizeController && !name.EndsWith("s"))
                {
                    name = name + "s";
                }
                var version = entityAttribute.Version ?? settings.ApiVersion;
                if (!string.IsNullOrEmpty(version))
                {
                    name = version + "/" + name;
                }
                var controllerType = baseType.MakeGenericType(type, idType);
                var attributeRouteModel = new AttributeRouteModel(new RouteAttribute(name));
                settings.Routes.Add(controllerType, attributeRouteModel);
                _logger.LogInformation("Autumn Path Controller : {0} => [ {1}.{2}, {3}.{4}  ]", name, type.Namespace,
                    type.Name, idType.Namespace, idType.Name);
            }
        }

        /// <summary>
        /// build autumn settings
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private static AutumnSettings BuildSettings(IConfiguration configuration)
        {

            AutumnSettings.Instance.ConnectionString =
                configuration.GetSection("Autumn.Data.Mvc:ConnectionString").Value;

            AutumnSettings.Instance.DatabaseName =
                configuration.GetSection("Autumn.Data.Mvc:DatabaseName").Value;

            var namingStrategySettings = configuration.GetSection("Autumn.Data.Mvc:NamingStrategy").Value;
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

            var defaultVersion = configuration.GetSection("Autumn.Data.Mvc:ApiVersion").Value;
            if (!string.IsNullOrWhiteSpace(defaultVersion))
            {
                AutumnSettings.Instance.ApiVersion = defaultVersion;
            }

            var controllerPluralize = configuration.GetSection("Autumn.Data.Mvc:PluralizeController").Value;
            if (!string.IsNullOrWhiteSpace(controllerPluralize))
            {
                AutumnSettings.Instance.PluralizeController = bool.Parse(controllerPluralize);
            }

            _logger.LogInformation(JsonConvert.SerializeObject(
                new
                {
                    ConnectionString=AutumnSettings.Instance.ConnectionString,
                    Database = AutumnSettings.Instance.DatabaseName,
                    ApiVersion = AutumnSettings.Instance.ApiVersion,
                    PluralizeController = AutumnSettings.Instance.PluralizeController,
                    NamingStrategy = (AutumnSettings.Instance.NamingStrategy != null)
                        ? AutumnSettings.Instance.NamingStrategy.GetType().Name
                        : ""
                }
                , Formatting.Indented));
            return AutumnSettings.Instance;
        }
    }
}
