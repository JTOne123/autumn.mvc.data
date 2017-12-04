using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public AutumnDataSettingsBuilder(AutumnDataSettings settings, Assembly callingAssembly)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _callingAssembly = callingAssembly;
        }

        public AutumnDataSettings Build()
        {
            BuildEntitiesInfos(_settings, _callingAssembly);
            BuildRoutes(_settings);
            return _settings;
        }

        /// <summary>         
        /// build EntitiesInfos
        /// </summary>
        private static void BuildEntitiesInfos(AutumnDataSettings settings,Assembly callingAssembly)
        {
            var items = new Dictionary<Type, AutumnEntityInfo>();
            foreach (var type in (settings.EntityAssembly ?? callingAssembly).GetTypes())
            {
                var entityAttribute = type.GetCustomAttribute<AutumnEntityAttribute>(false);
                if (entityAttribute == null) continue;
                AutumnEntityKeyInfo entityKeyInfo = null;
                foreach (var property in type.GetProperties())
                {
                    var keyAttribute = property.GetCustomAttribute<AutumnKeyAttribute>();
                    if (keyAttribute == null) continue;
                    entityKeyInfo = new AutumnEntityKeyInfo(property, keyAttribute);
                    break;
                }
                if (entityKeyInfo == null) continue;
                var proxyTypes = DataModelHelper.BuildModelsRequestTypes(type);
                items.Add(type,
                    new AutumnEntityInfo(settings, type, proxyTypes, entityAttribute, entityKeyInfo));
            }

            Mapper.Reset();

            Mapper.Initialize(c =>
            {
                foreach (var entityInfo in items.Values)
                {
                    foreach (var proxyType in entityInfo.ProxyRequestTypes.Values)
                    {
                        c.CreateMap(proxyType, entityInfo.EntityType);
                    }
                }
            });

            settings.EntitiesInfos = new ReadOnlyDictionary<Type, AutumnEntityInfo>(items);
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
                var entityKeyType = info.KeyInfo.Property.PropertyType;
                var controllerType = baseType.MakeGenericType(
                    info.EntityType,
                    info.ProxyRequestTypes[HttpMethod.Post],
                    info.ProxyRequestTypes[HttpMethod.Put],
                    entityKeyType);
                var attributeRouteModel = new AttributeRouteModel(new RouteAttribute(name));
                routes.Add(controllerType, attributeRouteModel);
                ignoreOperations.Add("/"+name, info.IgnoreOperations);
            }
            settings.Routes = new ReadOnlyDictionary<Type, AttributeRouteModel>(routes);
            settings.IgnoreOperations =
                new ReadOnlyDictionary<string, IReadOnlyList<HttpMethod>>(ignoreOperations);
        }
    }
}