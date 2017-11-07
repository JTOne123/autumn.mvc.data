using System;
using System.Linq;
using Autumn.Mvc.Data.Configurations;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Autumn.Mvc.Data.Controllers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RepositoryControllerNameConvention : Attribute, IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if(!AutumnSettings.Instance.Routes.ContainsKey(controller.ControllerType)) return;
            var defaultSelector = controller.Selectors.First(s => s.AttributeRouteModel == null);
            defaultSelector.AttributeRouteModel = AutumnSettings.Instance.Routes[controller.ControllerType];
            controller.ControllerName = controller.ControllerType.GetGenericArguments()[0].Name.ToLowerInvariant();
        }
    }
}