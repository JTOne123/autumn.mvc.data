using System;
using System.Collections.Generic;
using System.Reflection;
using Autumn.Mvc.Configurations;
using Autumn.Mvc.Data.Configurations;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Autumn.Mvc.Data.Controllers
{
    public class AutumnRespositoryControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly AutumnSettings _settings;

        public AutumnRespositoryControllerFeatureProvider(AutumnSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }
        
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var controllerType in _settings.DataSettings().Routes.Keys)
            {
                feature.Controllers.Add(controllerType.GetTypeInfo());
            }
        }
    }
}