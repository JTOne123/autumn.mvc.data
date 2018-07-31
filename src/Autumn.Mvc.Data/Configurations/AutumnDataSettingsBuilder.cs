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
        private Type _repositoryControllerAyncType;

        public AutumnDataSettingsBuilder(AutumnDataSettings settings, Assembly callingAssembly)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _callingAssembly = callingAssembly;
        }

        public AutumnDataSettings Build()
        {
            _settings.RepositoryContollerType = _repositoryControllerAyncType ?? typeof(RepositoryControllerAsync<,,,>);    
            BuildEntitiesInfos(_settings, _callingAssembly, _defaultApiVersion);
            BuildRoutes(_settings);
            return _settings;
        }

        /// <summary>
        /// set default api version
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        public AutumnDataSettingsBuilder ApiVersion(string version)
        {
            _defaultApiVersion = version;
            return this;
        }
     
        /// <summary>
        /// set default repository controller type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidCastException"></exception>
        public AutumnDataSettingsBuilder RepositoryControllerType(Type type)
        {
            if(type == null ) throw new ArgumentNullException(nameof(type));
            if (!typeof(RepositoryControllerAsync<,,,>).IsSubclassOfRawGeneric(type))
                 throw new InvalidCastException(nameof(type));   
            _repositoryControllerAyncType = type;
            return this;

        }

        /// <summary>
        /// set pluralize paths
        /// </summary>
        /// <param name="use"></param>
        /// <returns></returns>
        public AutumnDataSettingsBuilder PluralizeController(bool use = true)
        {
            _settings.PluralizeController = use;
            return this;
        }

        /// <summary>
        /// set assembly how found entities
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
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
            var entities = new Dictionary<Type, EntityInfo>();
            var resources = new Dictionary<Type, ResourceInfo>();

            foreach (var type in (settings.EntityAssembly ?? callingAssembly).GetTypes())
            {
                var resourceAttribute = type.GetCustomAttribute<ResourceAttribute>(false);
                var entityAttribute = type.GetCustomAttribute<EntityAttribute>(false);
                if (entityAttribute == null && resourceAttribute == null) continue;


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

                entities.Add(type,
                    new EntityInfo(
                        settings,
                        type,
                        entityAttribute,
                        keyPropertyInfo,
                        createDatePropertyInfo,
                        lastModifiedDatePropertyInfo,
                        createByPropertyInfo,
                        lastModifiedByPropertyInfo));

                if (resourceAttribute != null)
                {
                    var proxyTypes = DataModelHelper.BuildModelsRequestTypes(type);
                    resources.Add(type, new ResourceInfo(
                        settings,
                        entities[type], resourceAttribute.Version ?? apiVersion, proxyTypes, resourceAttribute));
                }
            }

            Mapper.Reset();

            Mapper.Initialize(c =>
            {
                foreach (var value in resources.Values)
                {
                    foreach (var proxyType in value.ProxyRequestTypes)
                    {
                        c.CreateMap(proxyType.Value, value.EntityInfo.EntityType);
                    }
                }
            });

            settings.EntitiesInfos = new ReadOnlyDictionary<Type, EntityInfo>(entities);
            settings.ResourceInfos = new ReadOnlyDictionary<Type, ResourceInfo>(resources);
            settings.ApiVersions = new ReadOnlyCollection<string>(settings.ResourceInfos.Values
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
            foreach (var entityType in settings.ResourceInfos.Keys)
            {
                var info = settings.ResourceInfos[entityType];
                var name = info.Name;
                if (settings.Parent.NamingStrategy != null)
                {
                    name = settings.Parent.NamingStrategy.GetPropertyName(name, false);
                }

                if (settings.PluralizeController && !name.EndsWith("s"))
                {
                    name = string.Concat(name, "s");
                }

                name = $"{info.ApiVersion}/{name}";
                var entityKeyType = info.EntityInfo.KeyInfo.PropertyType;
                var controllerType = settings.RepositoryContollerType.MakeGenericType(
                    info.EntityInfo.EntityType,
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