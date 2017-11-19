using System.Collections.Generic;
using System.Reflection;
using Autumn.Mvc.Data.Configurations;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Autumn.Mvc.Data.Controllers
{
    public class RespositoryControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly AutumnSettings _settings;

        public RespositoryControllerFeatureProvider(AutumnSettings settings)
        {
            _settings = settings;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var controllerType in _settings.Routes.Keys)
            {
                feature.Controllers.Add(controllerType.GetTypeInfo());
            }
        }
    }
}