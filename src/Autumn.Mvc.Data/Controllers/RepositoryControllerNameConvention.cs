using System;
using System.Linq;
using System.Reflection;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.Models;
using Autumn.Mvc.Data.Models.Helpers;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Autumn.Mvc.Data.Controllers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RepositoryControllerNameConvention : Attribute, IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (!AutumnSettings.Instance.Routes.ContainsKey(controller.ControllerType)) return;
            var defaultSelector = controller.Selectors.First(s => s.AttributeRouteModel == null);
            defaultSelector.AttributeRouteModel = AutumnSettings.Instance.Routes[controller.ControllerType];
            var entityType = controller.ControllerType.GetGenericArguments()[0];
            var entityAttribute = entityType.GetCustomAttribute<EntityAttribute>();
            var apiVersion = entityAttribute.Version ?? AutumnSettings.Instance.DefaultApiVersion;
            controller.ControllerName = entityAttribute.Name.ToCase(AutumnSettings.Instance.NamingStrategy);
            if (AutumnSettings.Instance.PluralizeController && !controller.ControllerName.EndsWith("s"))
            {
                controller.ControllerName = controller.ControllerName + "s";
            }
            controller.ApiExplorer.GroupName = apiVersion;
        }
    }
}