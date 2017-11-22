using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Autumn.Mvc.Data.Annotations;
using Autumn.Mvc.Data.Controllers;
using Autumn.Mvc.Data.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Newtonsoft.Json.Serialization;

namespace Autumn.Mvc.Data.Configurations
{
    public class AutumnSettings
    {
        public string PageSizeFieldName { get; set; }
        public string SortFieldName { get; set; }
        public string PageNumberFieldName { get; set; }
        public string QueryFieldName { get; set; }
        public int DefaultPageSize { get; set; }
        public bool PluralizeController { get; set; }
        public NamingStrategy NamingStrategy { get; set; }
        public string DefaultApiVersion { get; set; }
        public Assembly EntityAssembly { get; set; }
        public static AutumnSettings Instance { get; private set; }
        public Dictionary<Type, AttributeRouteModel> Routes { get; private set; }
        public Dictionary<Type, AutumnEntityInfo> EntitiesInfos { get; private set;}
        public List<EnableAutoConfigurationAttribute> AutoConfigurations { get;  private set;}

        static AutumnSettings()
        {
            Instance = new AutumnSettings()
            {
                PageSizeFieldName = "PageSize",
                PageNumberFieldName = "PageNumber",
                QueryFieldName = "Query",
                SortFieldName = "Sort",
                DefaultPageSize = 100,
                PluralizeController = true,
                DefaultApiVersion = "v1",
                NamingStrategy = new DefaultNamingStrategy()
            };

        }

        /// <summary>
        /// build AutumnSettings
        /// </summary>
        /// <returns></returns>
        public static AutumnSettings Build(AutumnOptions options, Assembly callingAssembly)
        {
            Instance.PageSizeFieldName = Instance.PageSizeFieldName.ToCase(Instance.NamingStrategy);
            Instance.PageNumberFieldName = Instance.PageNumberFieldName.ToCase(Instance.NamingStrategy);
            Instance.QueryFieldName = Instance.QueryFieldName.ToCase(Instance.NamingStrategy);
            Instance.SortFieldName = Instance.SortFieldName.ToCase(Instance.NamingStrategy);
            BuildEntitiesInfosSettings(callingAssembly);
            BuildEnableAutoConfigurationsSettings(callingAssembly);
            BuildRoutes();
            return Instance;
        }

        /// <summary>
        /// build EntitiesInfos
        /// </summary>
        private static void BuildEntitiesInfosSettings(Assembly callingAssembly)
        {
            Instance.EntitiesInfos = new Dictionary<Type, AutumnEntityInfo>();
            Instance.EntityAssembly = Instance.EntityAssembly ?? callingAssembly;
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
                name = string.Format("{0}/{1}", info.ApiVersion, name);
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