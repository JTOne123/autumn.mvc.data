﻿using System;
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
using Swashbuckle.AspNetCore.Swagger;


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
                    new PageableModelBinderProvider(settings));
                config.ModelBinderProviders.Insert(1,
                    new QueryModelBinderProvider(settings));
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

            services.AddSwaggerGen(c =>
            {
                foreach (var version in settings.ApiVersions)
                {
                    c.SwaggerDoc(version,new Info {Title = "", Version = version});
                }
                //c.SwaggerDoc("v1", new Info {Title = "My API", Version = "v1"});
                c.OperationFilter<AutumnOperationFilter>();
              });
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
                var version = entityAttribute.Version ?? settings.DefaultApiVersion;
                if (!string.IsNullOrEmpty(version))
                {
                    name = version + "/" + name;
                    if (!settings.ApiVersions.Contains(version))
                    {
                        settings.ApiVersions.Add(version);
                    }
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

            var settings = AutumnSettings.Instance;
            
            settings.ConnectionString =
                configuration.GetSection("Autumn.Data.Mvc:ConnectionString").Value;

            settings.DatabaseName =
                configuration.GetSection("Autumn.Data.Mvc:DatabaseName").Value;

            var namingStrategySettings = configuration.GetSection("Autumn.Data.Mvc:NamingStrategy").Value;
            if (!string.IsNullOrWhiteSpace(namingStrategySettings))
            {
                if (namingStrategySettings.ToUpperInvariant() == "SNAKE_CASE")
                {
                    settings.NamingStrategy = new SnakeCaseNamingStrategy();
                }
                else if (namingStrategySettings.ToUpperInvariant() == "CAMEL_CASE")
                {
                    settings.NamingStrategy = new CamelCaseNamingStrategy();
                }
            }
            
            var queryFieldNameSettings = configuration.GetSection("Autumn.Data.Mvc:QueryFieldName").Value;
            if (!string.IsNullOrWhiteSpace(queryFieldNameSettings))
            {
                settings.QueryFieldName = queryFieldNameSettings;
            }
            settings.QueryFieldName = settings.QueryFieldName.ToCase(settings.NamingStrategy);
            
            var sortFieldNameSettings = configuration.GetSection("Autumn.Data.Mvc:SortFieldName").Value;
            if (!string.IsNullOrWhiteSpace(sortFieldNameSettings))
            {
                settings.SortFieldName = sortFieldNameSettings;
            }
            settings.SortFieldName = settings.SortFieldName.ToCase(settings.NamingStrategy);
            
            var pageSizeFieldNameSettings = configuration.GetSection("Autumn.Data.Mvc:PageSizeFieldName").Value;
            if (!string.IsNullOrWhiteSpace(pageSizeFieldNameSettings))
            {
                settings.PageSizeFieldName = pageSizeFieldNameSettings;
            }
            settings.PageSizeFieldName = settings.PageSizeFieldName.ToCase(settings.NamingStrategy);
            
            var pageNumberFieldNameSettings = configuration.GetSection("Autumn.Data.Mvc:PageNumberFieldName").Value;
            if (!string.IsNullOrWhiteSpace(pageNumberFieldNameSettings))
            {
                settings.SortFieldName = pageNumberFieldNameSettings;
            }
            settings.PageNumberFieldName = settings.PageNumberFieldName.ToCase(settings.NamingStrategy);

            var defaultPageSizeSettings = configuration.GetSection("Autumn.Data.Mvc:DefaultPageSize").Value;
            if (int.TryParse(defaultPageSizeSettings, out var defaultPageSize))
            {
                settings.DefaultPageSize = defaultPageSize;
            }
            
            var defaultVersion = configuration.GetSection("Autumn.Data.Mvc:DefaultApiVersion").Value;
            if (!string.IsNullOrWhiteSpace(defaultVersion))
            {
                AutumnSettings.Instance.DefaultApiVersion = defaultVersion;
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
                    DefaultApiVersion = AutumnSettings.Instance.DefaultApiVersion,
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
