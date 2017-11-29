using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Autumn.Mvc.Data.Controllers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AutumnRepositoryControllerNameConvention : Attribute, IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (!AutumnApplication.Current.Routes.ContainsKey(controller.ControllerType)) return;
            var defaultSelector = controller.Selectors.First(s => s.AttributeRouteModel == null);
            defaultSelector.AttributeRouteModel = AutumnApplication.Current.Routes[controller.ControllerType];
            var entityType = controller.ControllerType.GetGenericArguments()[0];
            var autumnEntityInfo = AutumnApplication.Current.EntitiesInfos[entityType];
            controller.ApiExplorer.GroupName = autumnEntityInfo.ApiVersion;
            controller.ControllerName = autumnEntityInfo.ControllerName;
        }
    }
}