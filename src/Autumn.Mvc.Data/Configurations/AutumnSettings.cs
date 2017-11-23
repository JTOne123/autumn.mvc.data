using System;
using System.Collections.Generic;
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
        public bool UseSwagger { get; set; }
        public string DefaultApiVersion { get; set; }
        public Assembly EntityAssembly { get; set; }
        public static AutumnSettings Current { get; private set; }
        public Dictionary<Type, AttributeRouteModel> Routes { get; private set; }
        public Dictionary<string,AutumnIgnoreOperationType> IgnoresPaths { get; set; }
        public Dictionary<Type, AutumnEntityInfo> EntitiesInfos { get; private set; }

        static AutumnSettings()
        {
            Current = new AutumnSettings()
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
        public static void Build(Assembly callingAssembly)
        {
            Current.NamingStrategy = Current.NamingStrategy ?? new DefaultNamingStrategy();
            Current.PageSizeFieldName = (Current.PageSizeFieldName ?? "PageSize").ToCase(Current.NamingStrategy);
            Current.PageNumberFieldName = (Current.PageNumberFieldName ?? "PageNumber").ToCase(Current.NamingStrategy);
            Current.QueryFieldName = (Current.QueryFieldName ?? "Query").ToCase(Current.NamingStrategy);
            Current.SortFieldName = (Current.SortFieldName ?? "Sort").ToCase(Current.NamingStrategy);
            BuildEntitiesInfosSettings(callingAssembly);
            BuildRoutes();
        }

        /// <summary>
        /// build EntitiesInfos
        /// </summary>
        private static void BuildEntitiesInfosSettings(Assembly callingAssembly)
        {
            Current.EntitiesInfos = new Dictionary<Type, AutumnEntityInfo>();
            Current.EntityAssembly = Current.EntityAssembly ?? callingAssembly;
            foreach (var type in Current.EntityAssembly.GetTypes())
            {
                var entityAttribute = type.GetCustomAttribute<AutumnEntityAttribute>(false);
                if (entityAttribute == null) continue;
                var ignoreOperationAttribute = type.GetCustomAttribute<AutumnIgnoreOperationAttribute>(false);
                AutumnEntityKeyInfo entityKeyInfo = null;
                foreach (var property in type.GetProperties())
                {
                    var keyAttribute = property.GetCustomAttribute<AutumnKeyAttribute>();
                    if (keyAttribute == null) continue;
                    entityKeyInfo = new AutumnEntityKeyInfo(property, keyAttribute);
                    break;
                }
                if (entityKeyInfo == null) continue;
                var proxyTypes = AutumnTypeBuilder.CompileResultType(type);
                Current.EntitiesInfos.Add(type,
                    new AutumnEntityInfo(type, proxyTypes, entityAttribute, entityKeyInfo, ignoreOperationAttribute));
            }

            Mapper.Initialize(c =>
            {
                foreach (var entityInfo in Current.EntitiesInfos.Values)
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
            Current.Routes = new Dictionary<Type, AttributeRouteModel>();
            Current.IgnoresPaths = new Dictionary<string, AutumnIgnoreOperationType>();
            var baseType = typeof(RepositoryControllerAsync<,,,>);
            foreach (var entityType in Current.EntitiesInfos.Keys)
            {
                var info = Current.EntitiesInfos[entityType];
                var name = info.Name;
                switch (Current.NamingStrategy)
                {
                    case SnakeCaseNamingStrategy _:
                        name = name.ToSnakeCase();
                        break;
                    case CamelCaseNamingStrategy _:
                        name = name.ToCamelCase();
                        break;
                }
                if (Current.PluralizeController && !name.EndsWith("s"))
                {
                    name = name + "s";
                }
                name = string.Format("{0}/{1}", info.ApiVersion, name);
                var entityKeyType = info.KeyInfo.Property.PropertyType;
                var controllerType = baseType.MakeGenericType(
                    info.EntityType,
                    info.ProxyTypes[AutumnIgnoreOperationPropertyType.Insert],
                    info.ProxyTypes[AutumnIgnoreOperationPropertyType.Update],
                    entityKeyType);
                var attributeRouteModel = new AttributeRouteModel(new RouteAttribute(name));
                Current.Routes.Add(controllerType, attributeRouteModel);
                if (info.IgnoreOperations != null)
                {
                    Current.IgnoresPaths.Add("/"+name, info.IgnoreOperations.Value);
                }
            }
        }
    }
}