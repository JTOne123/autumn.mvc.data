using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Autumn.Mvc.Data.Controllers
{
    public class AutumnRespositoryControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var controllerType in AutumnApplication.Current.Routes.Keys)
            {
                feature.Controllers.Add(controllerType.GetTypeInfo());
            }
        }
    }
}