using System;
using System.Linq;
using System.Reflection;
using Autumn.Data.Rest.Configurations;
using Autumn.Data.Rest.Entities;
using Autumn.Data.Rest.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Newtonsoft.Json.Serialization;

namespace Autumn.Data.Rest.Controllers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RepositoryControllerNameConvention : Attribute, IControllerModelConvention
    {
        internal static AutumnSettings Settings { private get; set; }

        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.GetGenericTypeDefinition() != typeof(RepositoryControllerAsync<,>))
            {
                return;
            }
            var entityType = controller.ControllerType.GetGenericArguments()[0];
            var entityAttribute = entityType.GetCustomAttribute<EntityAttribute>();
            var name = entityAttribute.Name ?? entityType.Name;
            if (Settings.NamingStrategy is SnakeCaseNamingStrategy)
            {
                name = name.ToSnakeCase();
            }
            if (Settings.NamingStrategy is CamelCaseNamingStrategy)
            {
                name = name.ToCamelCase();
            }
            if (Settings.PluralizeController && !name.EndsWith("s"))
            {
                name = name + "s";
            }
            var version = entityAttribute.Version ?? Settings.ApiVersion;
            if (!string.IsNullOrEmpty(version))
            {
                name = version + "/" + name;
            }
            var defaultSelector = controller.Selectors.First(s => s.AttributeRouteModel == null);

            defaultSelector.AttributeRouteModel = new AttributeRouteModel(new RouteAttribute(name));
            controller.ControllerName = controller.ControllerType.GetGenericArguments()[0].Name.ToLowerInvariant();
        }
    }
}