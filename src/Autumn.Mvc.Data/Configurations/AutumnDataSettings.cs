using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using Autumn.Mvc.Configurations;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Autumn.Mvc.Data.Configurations
{
    public class AutumnDataSettings
    {
        public AutumnSettings Parent { get; private set; }
        public IReadOnlyDictionary<Type, AttributeRouteModel> Routes { get; set; }
        public IReadOnlyDictionary<Type, EntityInfo> EntitiesInfos { get; set; }
        public IReadOnlyDictionary<Type, ResourceInfo> ResourceInfos { get; set; }
        public IReadOnlyDictionary<string, IReadOnlyList<HttpMethod>> IgnoreOperations { get; set; }
        public bool PluralizeController { get; set; } = true;
        public Assembly EntityAssembly { get; set; }
        public Type RepositoryContollerType { get; set; }
        public IReadOnlyCollection<string> ApiVersions { get; set; }

        public AutumnDataSettings(AutumnSettings parent)
        {
            Parent = parent;
        }
    }
}