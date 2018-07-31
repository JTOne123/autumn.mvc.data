using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.RegularExpressions;
using Autumn.Mvc.Data.Annotations;

namespace Autumn.Mvc.Data.Configurations
{
    public class ResourceInfo
    {
        public string ApiVersion { get; }
        public string Name { get; }
        public string ControllerName { get; }
        public IReadOnlyDictionary<HttpMethod, Type> ProxyRequestTypes { get; }
        public IReadOnlyList<HttpMethod> IgnoreOperations { get; }
        public EntityInfo EntityInfo { get; }

        public ResourceInfo(
            AutumnDataSettings dataSettings,
            EntityInfo entityInfo,
            string defaultApiVersion,
            IReadOnlyDictionary<HttpMethod, Type> proxyRequestTypes,
            ResourceAttribute resourceAttribute)
        {
            EntityInfo = entityInfo;
            ProxyRequestTypes = proxyRequestTypes ?? throw new ArgumentNullException(nameof(proxyRequestTypes));
            ApiVersion =
                Regex.Match(resourceAttribute.Version ?? string.Empty, "v[0-9]+", RegexOptions.IgnoreCase).Success
                    ? resourceAttribute.Version
                    : defaultApiVersion;
            Name = resourceAttribute.Name ?? entityInfo.EntityType.Name;
            if (dataSettings.Parent.NamingStrategy != null)
            {
                ControllerName = dataSettings.Parent.NamingStrategy.GetPropertyName(Name, false);
            }
            if (dataSettings.PluralizeController && !ControllerName.EndsWith("s"))
            {
                string.Concat(ControllerName, "s");
            }
            var ignoreOperations = new List<HttpMethod>();
            if (!resourceAttribute.Insertable)
            {
                ignoreOperations.Add(HttpMethod.Post);
            }
            if (!resourceAttribute.Updatable)
            {
                ignoreOperations.Add(HttpMethod.Put);
            }
            if (!resourceAttribute.Deletable)
            {
                ignoreOperations.Add(HttpMethod.Delete);
            }
            IgnoreOperations = new ReadOnlyCollection<HttpMethod>(ignoreOperations);
        }

    }
}