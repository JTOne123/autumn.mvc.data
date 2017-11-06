using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.Models;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Autumn.Mvc.Data.Controllers
{
    public class RespositoryControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {

        private readonly Assembly _assembly;
        private readonly AutumnSettings _settings;

        public RespositoryControllerFeatureProvider(Assembly assembly, AutumnSettings settings)
        {
            _assembly = assembly;
            _settings = settings;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            foreach (var type in _assembly.GetTypes())
            {
                var entityAttribute = type.GetCustomAttribute<EntityAttribute>();
                if (entityAttribute == null) continue;
                var propertyType = type.GetProperties().Single(p => p.GetCustomAttribute<IdAttribute>() != null)
                    .PropertyType;
                var controllerType = typeof(RepositoryControllerAsync<,>).MakeGenericType(type, propertyType)
                    .GetTypeInfo();
                feature.Controllers.Add(controllerType);
            }
        }
    }
}