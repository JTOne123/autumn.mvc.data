using System.Linq;
using Autumn.Mvc.Data.Configurations;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Autumn.Mvc.Data.Controllers
{
    public class RepositoryControllerNameConvention : IControllerModelConvention
    {

        private readonly AutumnDataSettings _settings;

        public RepositoryControllerNameConvention(AutumnDataSettings settings)
        {
            _settings = settings;
        }

        public void Apply(ControllerModel controller)
        {
            if (!_settings.Routes.ContainsKey(controller.ControllerType)) return;
            var defaultSelector = controller.Selectors.First(s => s.AttributeRouteModel == null);
            defaultSelector.AttributeRouteModel = _settings.Routes[controller.ControllerType];
            var entityType = controller.ControllerType.GetGenericArguments()[0];
            var autumnEntityInfo = _settings.ResourceInfos[entityType];
            controller.ApiExplorer.GroupName = autumnEntityInfo.ApiVersion;
            controller.ControllerName = autumnEntityInfo.ControllerName;
        }
    }
}