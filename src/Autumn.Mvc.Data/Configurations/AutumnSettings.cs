using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Autumn.Mvc.Data.Annotations;
using Autumn.Mvc.Data.Controllers;
using Autumn.Mvc.Data.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Serialization;

namespace Autumn.Mvc.Data.Configurations
{
    public class AutumnSettings
    {
        public static AutumnSettings Instance { get; }

        static AutumnSettings()
        {
            Instance = new AutumnSettings()
            {
                PageSizeFieldName = "PageSize",
                PageNumberFieldName = "PageNumber",
                QueryFieldName = "Query",
                SortFieldName = "Sort",
                ApiVersions = new List<string>(),
                DefaultPageSize = 100
            };
        }

        public IHostingEnvironment Environment { get; set; }
        public ICollection<string> ApiVersions { get; set; }
        public string PageSizeFieldName { get; set; }
        public string SortFieldName { get; set; }
        public string PageNumberFieldName { get; set; }
        public string QueryFieldName { get; set; }
        public NamingStrategy NamingStrategy { get; set; }
        public string DefaultApiVersion { get; set; }
        public int DefaultPageSize { get; set; }
        public bool PluralizeController { get; set; }
        public Dictionary<Type, AttributeRouteModel> Routes { get; set; }
        public Assembly EntityAssembly { get; set; }
        public Dictionary<Type, AutumnEntityInfo> EntitiesInfos { get; set; }
        public List<EnableAutoConfigurationAttribute> AutoConfigurations { get; set; }


        /// <summary>
        /// build AutumnSettings
        /// </summary>
        /// <returns></returns>
        public static AutumnSettings Build(IConfiguration configuration, IHostingEnvironment environment,
            Assembly callingAssembly)
        {
            BuildNamingNamingStrategySettings(configuration);
            BuildApiInfos(configuration);
            BuildQueryInfoSettings(configuration);
            BuildPagingInfoSettings(configuration);
            BuildEntitiesInfosSettings(callingAssembly);
            BuildEnableAutoConfigurationsSettings(callingAssembly);
            BuildRoutes();
            return Instance;
        }

        /// <summary>
        /// build naming strategy settings
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private static void BuildApiInfos(IConfiguration configuration)
        {
            var defaultVersion = configuration.GetSection("Autumn.Data.Mvc:DefaultApiVersion").Value;
            if (!string.IsNullOrWhiteSpace(defaultVersion))
            {
                Instance.DefaultApiVersion = defaultVersion;
            }
            Instance.DefaultApiVersion = Instance.DefaultApiVersion ?? "v1";

            var controllerPluralize = configuration.GetSection("Autumn.Data.Mvc:PluralizeController").Value;
            if (!string.IsNullOrWhiteSpace(controllerPluralize))
            {
                Instance.PluralizeController = bool.Parse(controllerPluralize);
            }
        }

        /// <summary>
        /// build EntitiesInfos
        /// </summary>
        private static void BuildEntitiesInfosSettings(Assembly callingAssembly)
        {
            Instance.EntityAssembly = Instance.EntityAssembly ?? callingAssembly;
            Instance.EntitiesInfos = new Dictionary<Type, AutumnEntityInfo>();
            foreach (var type in Instance.EntityAssembly.GetTypes())
            {
                var entityAttribute = type.GetCustomAttribute<AutumnEntityAttribute>();
                if (entityAttribute == null) continue;
                AutumnEntityKeyInfo keyInfo = null;
                foreach (var property in type.GetProperties())
                {
                    var keyAttribute = property.GetCustomAttribute<AutumnKeyAttribute>();
                    if (keyAttribute == null) continue;
                    keyInfo = new AutumnEntityKeyInfo(property, keyAttribute);
                    break;
                }
                if (keyInfo == null) continue;
                var proxyTypes = AutumnTypeBuilder.CompileResultType(type);
                Instance.EntitiesInfos.Add(type,
                    new AutumnEntityInfo(Instance, type, proxyTypes, entityAttribute, keyInfo));
            }

            Mapper.Initialize(c =>
            {
                foreach (var entityInfo in Instance.EntitiesInfos.Values)
                {
                    foreach (var proxyType in entityInfo.ProxyTypes.Values)
                    {
                        c.CreateMap(proxyType, entityInfo.EntityType);
                    }
                }
            });

        }

        /// <summary>
        /// build naming strategy settings
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private static void BuildNamingNamingStrategySettings(IConfiguration configuration)
        {
            var namingStrategySettings = configuration.GetSection("Autumn.Data.Mvc:NamingStrategy").Value;
            if (string.IsNullOrWhiteSpace(namingStrategySettings)) return;
            if (namingStrategySettings.ToUpperInvariant() != "SNAKE_CASE")
            {
                if (namingStrategySettings.ToUpperInvariant() == "CAMEL_CASE")
                {
                    Instance.NamingStrategy = new CamelCaseNamingStrategy();
                }
            }
            else
            {
                Instance.NamingStrategy = new SnakeCaseNamingStrategy();
            }
        }

        /// <summary>
        /// build query informations settings
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private static void BuildQueryInfoSettings(IConfiguration configuration)
        {
            var queryFieldNameSettings = configuration.GetSection("Autumn.Data.Mvc:QueryFieldName").Value;
            if (!string.IsNullOrWhiteSpace(queryFieldNameSettings))
            {
                Instance.QueryFieldName = queryFieldNameSettings;
            }
            Instance.QueryFieldName = Instance.QueryFieldName.ToCase(Instance.NamingStrategy);

            var sortFieldNameSettings = configuration.GetSection("Autumn.Data.Mvc:SortFieldName").Value;
            if (!string.IsNullOrWhiteSpace(sortFieldNameSettings))
            {
                Instance.SortFieldName = sortFieldNameSettings;
            }
            Instance.SortFieldName = Instance.SortFieldName.ToCase(Instance.NamingStrategy);
        }

        /// <summary>
        /// build pagination settings
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        private static void BuildPagingInfoSettings(IConfiguration configuration)
        {
            var pageSizeFieldNameSettings = configuration.GetSection("Autumn.Data.Mvc:PageSizeFieldName").Value;
            if (!string.IsNullOrWhiteSpace(pageSizeFieldNameSettings))
            {
                Instance.PageSizeFieldName = pageSizeFieldNameSettings;
            }
            Instance.PageSizeFieldName = Instance.PageSizeFieldName.ToCase(Instance.NamingStrategy);

            var pageNumberFieldNameSettings = configuration.GetSection("Autumn.Data.Mvc:PageNumberFieldName").Value;
            if (!string.IsNullOrWhiteSpace(pageNumberFieldNameSettings))
            {
                Instance.SortFieldName = pageNumberFieldNameSettings;
            }
            Instance.PageNumberFieldName = Instance.PageNumberFieldName.ToCase(Instance.NamingStrategy);

            var defaultPageSizeSettings = configuration.GetSection("Autumn.Data.Mvc:DefaultPageSize").Value;
            if (int.TryParse(defaultPageSizeSettings, out var defaultPageSize))
            {
                Instance.DefaultPageSize = defaultPageSize;
            }
        }

        /// <summary>
        /// build routes
        /// </summary>
        /// <returns></returns>
        private static void BuildRoutes()
        {
            Instance.Routes = new Dictionary<Type, AttributeRouteModel>();
            var baseType = typeof(RepositoryControllerAsync<,,,>);
            foreach (var entityType in Instance.EntitiesInfos.Keys)
            {
                var info = Instance.EntitiesInfos[entityType];
                var name = info.Name;
                switch (Instance.NamingStrategy)
                {
                    case SnakeCaseNamingStrategy _:
                        name = name.ToSnakeCase();
                        break;
                    case CamelCaseNamingStrategy _:
                        name = name.ToCamelCase();
                        break;
                }
                if (Instance.PluralizeController && !name.EndsWith("s"))
                {
                    name = name + "s";
                }
                var version = info.Version ?? Instance.DefaultApiVersion;
                if (!string.IsNullOrEmpty(version))
                {
                    name = version + "/" + name;
                    if (!Instance.ApiVersions.Contains(version))
                    {
                        Instance.ApiVersions.Add(version);
                    }
                }
                var entityKeyType = info.KeyInfo.Property.PropertyType;
                var controllerType = baseType.MakeGenericType(
                    info.EntityType,
                    info.ProxyTypes[AutumnIgnoreType.Post], 
                    info.ProxyTypes[AutumnIgnoreType.Put],
                    entityKeyType);
                var attributeRouteModel = new AttributeRouteModel(new RouteAttribute(name));
                Instance.Routes.Add(controllerType, attributeRouteModel);
            }
        }

        private static void BuildEnableAutoConfigurationsSettings(Assembly callingAssembly)
        {
            Instance.AutoConfigurations = new List<EnableAutoConfigurationAttribute>();
            // find autoconfigurations
            foreach (var type in callingAssembly.GetTypes())
            {
                var attributes = type.GetCustomAttributes()
                    .Where(a => a.GetType().IsSubclassOf(typeof(EnableAutoConfigurationAttribute)))
                    .OfType<EnableAutoConfigurationAttribute>();
                Instance.AutoConfigurations.AddRange(attributes);
            }

            Instance.AutoConfigurations = Instance.AutoConfigurations.OrderBy(c => c.Order)
                .ToList();
        }
    }
}