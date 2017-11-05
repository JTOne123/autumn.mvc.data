using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autumn.Data.Rest.Configurations;
using Autumn.Data.Rest.Entities;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Logging;

namespace Autumn.Data.Rest.Controllers
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