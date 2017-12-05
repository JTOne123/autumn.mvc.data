using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.RegularExpressions;
using Autumn.Mvc.Data.Annotations;


namespace Autumn.Mvc.Data.Configurations
{
    public class EntityInfo
    {
        public Type EntityType { get; }
        public string ApiVersion { get; }
        public string Name { get; }
        public string ControllerName { get; }
        public AutumnDataSettings Settings { get; set; }
        public EntityKeyInfo KeyInfo { get; }
        public IReadOnlyDictionary<HttpMethod, Type> ProxyRequestTypes { get; }
        public IReadOnlyList<HttpMethod> IgnoreOperations { get; }

        public EntityInfo(AutumnDataSettings dataSettings, Type entityType, IReadOnlyDictionary<HttpMethod, Type> proxyRequestTypes,
            EntityAttribute entityAttribute,
            EntityKeyInfo keyInfo)
        {
            Settings = dataSettings ?? throw new ArgumentNullException(nameof(dataSettings));
            EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
            ProxyRequestTypes = proxyRequestTypes ?? throw new ArgumentNullException(nameof(proxyRequestTypes));
            ApiVersion =
                Regex.Match(entityAttribute.Version ?? string.Empty, "v[0-9]+", RegexOptions.IgnoreCase).Success
                    ? entityAttribute.Version
                    : Settings.ApiVersion;
            Name = entityAttribute.Name ?? entityType.Name;
            KeyInfo = keyInfo;
            if (Settings.Parent.NamingStrategy != null)
            {
                ControllerName = Settings.Parent.NamingStrategy.GetPropertyName(Name, false);
            }
            if (Settings.PluralizeController && !ControllerName.EndsWith("s"))
            {
                ControllerName += "s";
            }
            var ignoreOperations = new List<HttpMethod>();
            if (!entityAttribute.Insertable)
            {
                ignoreOperations.Add(HttpMethod.Post);
            }
            if (!entityAttribute.Updatable)
            {
                ignoreOperations.Add(HttpMethod.Put);
            }
            if (!entityAttribute.Deletable)
            {
                ignoreOperations.Add(HttpMethod.Delete);
            }
            IgnoreOperations = new ReadOnlyCollection<HttpMethod>(ignoreOperations);
        }
    }
}