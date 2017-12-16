using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using AutoMapper;
using Autumn.Mvc.Data.Annotations;
using Autumn.Mvc.Data.Controllers;
using Autumn.Mvc.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Autumn.Mvc.Data.Configurations
{
    public class AutumnDataSettingsBuilder
    {
        private readonly AutumnDataSettings _settings;
        private readonly Assembly _callingAssembly;
        private string _defaultApiVersion = "v1";

        public AutumnDataSettingsBuilder(AutumnDataSettings settings, Assembly callingAssembly)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _callingAssembly = callingAssembly;
        }

        public AutumnDataSettings Build()
        {
            BuildEntitiesInfos(_settings, _callingAssembly, _defaultApiVersion);
            BuildRoutes(_settings);
            return _settings;
        }

        public AutumnDataSettingsBuilder ApiVersion(string version = "v1")
        {
            _defaultApiVersion = version;
            return this;
        }

        public AutumnDataSettingsBuilder PluralizeController(bool use = true)
        {
            _settings.PluralizeController = use;
            return this;
        }

        public AutumnDataSettingsBuilder EntityAssembly(Assembly assembly)
        {
            _settings.EntityAssembly = assembly;
            return this;
        }

        /// <summary>
        /// check if property is AuditableDate
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        private static bool IsAuditableDateProperty(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null) return false;
            if (propertyInfo.PropertyType == typeof(DateTime)) return true;
            if (propertyInfo.PropertyType == typeof(DateTime?)) return true;
            if (propertyInfo.PropertyType == typeof(DateTimeOffset)) return true;
            return propertyInfo.PropertyType == typeof(DateTimeOffset?);
        }

        /// <summary>
        /// check if property is AuditableBy
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <returns></returns>
        private static bool IsAuditableByProperty(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null) return false;
            return (propertyInfo.PropertyType == typeof(string));
        }

        /// <summary>         
        /// build EntitiesInfos
        /// </summary>
        private static void BuildEntitiesInfos(AutumnDataSettings settings, Assembly callingAssembly, string apiVersion)
        {
            var items = new Dictionary<Type, EntityInfo>();

            foreach (var type in (settings.EntityAssembly ?? callingAssembly).GetTypes())
            {
                var entityAttribute = type.GetCustomAttribute<EntityAttribute>(false);
                if (entityAttribute == null) continue;

                PropertyInfo keyPropertyInfo = null;
                PropertyInfo createDatePropertyInfo = null;
                PropertyInfo lastModifiedDatePropertyInfo = null;
                PropertyInfo createByPropertyInfo = null;
                PropertyInfo lastModifiedByPropertyInfo = null;
                foreach (var property in type.GetProperties())
                {
                    var keyAttribute = property.GetCustomAttribute<IdAttribute>();
                    if (keyAttribute != null)
                    {
                        keyPropertyInfo = property;
                    }
                    if (property.GetCustomAttribute<CreatedDateAttribute>(true) != null &&
                        IsAuditableDateProperty(property))
                    {
                        createDatePropertyInfo = property;
                    }
                    if (property.GetCustomAttribute<LastModifiedDateAttribute>(true) != null &&
                        IsAuditableDateProperty(property))
                    {
                        lastModifiedDatePropertyInfo = property;
                    }
                    if (property.GetCustomAttribute<CreatedByAttribute>(true) != null &&
                        IsAuditableByProperty(property))
                    {
                        createByPropertyInfo = property;
                    }
                    if (property.GetCustomAttribute<LastModifiedByAttribute>(true) != null &&
                        IsAuditableByProperty(property))
                    {
                        lastModifiedByPropertyInfo = property;
                    }
                }
                var proxyTypes = DataModelHelper.BuildModelsRequestTypes(type);
                items.Add(type,
                    new EntityInfo(
                        settings,
                        apiVersion,
                        type,
                        proxyTypes,
                        entityAttribute,
                        keyPropertyInfo,
                        createDatePropertyInfo,
                        lastModifiedDatePropertyInfo,
                        createByPropertyInfo,
                        lastModifiedByPropertyInfo));
            }

            Mapper.Reset();

            Mapper.Initialize(c =>
            {
                foreach (var entityInfo in items.Values)
                {
                    foreach (var proxyType in entityInfo.ProxyRequestTypes)
                    {
                        c.CreateMap(proxyType.Value, entityInfo.EntityType);
                    }
                }
            });

            settings.EntitiesInfos = new ReadOnlyDictionary<Type, EntityInfo>(items);
            settings.ApiVersions = new ReadOnlyCollection<string>(settings.EntitiesInfos.Values
                .Select(e => e.ApiVersion)
                .Distinct().OrderBy(e => e).ToList());
        }

        /// <summary>
        /// build routes
        /// </summary>
        /// <returns></returns>
        private static void BuildRoutes(AutumnDataSettings settings)
        {
            var routes = new Dictionary<Type, AttributeRouteModel>();
            var ignoreOperations = new Dictionary<string, IReadOnlyList<HttpMethod>>();
            var baseType = typeof(RepositoryControllerAsync<,,,>);
            foreach (var entityType in settings.EntitiesInfos.Keys)
            {
                var info = settings.EntitiesInfos[entityType];
                var name = info.Name;
                if (settings.Parent.NamingStrategy != null)
                {
                    name = settings.Parent.NamingStrategy.GetPropertyName(name, false);
                }
                if (settings.PluralizeController && !name.EndsWith("s"))
                {
                    name = name + "s";
                }
                name = string.Format("{0}/{1}", info.ApiVersion, name);
                var entityKeyType = info.KeyInfo.PropertyType;
                var controllerType = baseType.MakeGenericType(
                    info.EntityType,
                    info.ProxyRequestTypes[HttpMethod.Post],
                    info.ProxyRequestTypes[HttpMethod.Put],
                    entityKeyType);
                var attributeRouteModel = new AttributeRouteModel(new RouteAttribute(name));
                routes.Add(controllerType, attributeRouteModel);
                ignoreOperations.Add("/" + name, info.IgnoreOperations);
            }
            settings.Routes = new ReadOnlyDictionary<Type, AttributeRouteModel>(routes);
            settings.IgnoreOperations =
                new ReadOnlyDictionary<string, IReadOnlyList<HttpMethod>>(ignoreOperations);
        }
    }
}