using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.RegularExpressions;
using Autumn.Mvc.Data.Annotations;


namespace Autumn.Mvc.Data.Configurations
{
    public class AutumnEntityInfo
    {
        public Type EntityType { get; }
        public string ApiVersion { get; }
        public string Name { get; }
        public string ControllerName { get; }
        public AutumnEntityKeyInfo KeyInfo { get; }
        public IReadOnlyDictionary<HttpMethod, Type> ProxyRequestTypes { get; }
        public IReadOnlyList<HttpMethod> IgnoreOperations { get; }

        public AutumnEntityInfo(Type entityType, IReadOnlyDictionary<HttpMethod, Type> proxyRequestTypes,
            AutumnEntityAttribute entityAttribute,
            AutumnEntityKeyInfo keyInfo)
        {
            EntityType = entityType;
            ProxyRequestTypes = proxyRequestTypes;
            ApiVersion =
                Regex.Match(entityAttribute.Version ?? string.Empty, "v[0-9]+", RegexOptions.IgnoreCase).Success
                    ? entityAttribute.Version
                    : AutumnApplication.Current.DefaultApiVersion;
            Name = entityAttribute.Name ?? entityType.Name;
            KeyInfo = keyInfo;
            ControllerName = Name.ToCase(AutumnApplication.Current.NamingStrategy);
            if (AutumnApplication.Current.PluralizeController && !ControllerName.EndsWith("s"))
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