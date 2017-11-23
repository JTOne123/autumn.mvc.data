using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Autumn.Mvc.Data.Annotations;
using Autumn.Mvc.Data.Helpers;

namespace Autumn.Mvc.Data.Configurations
{
    public class AutumnEntityInfo
    {
        public Type EntityType { get; }
        public string ApiVersion { get; }
        public string Name { get; }
        public string ControllerName { get; }
        public AutumnEntityKeyInfo KeyInfo { get; }
        public AutumnIgnoreOperationType? IgnoreOperations { get;  }
        public Dictionary<AutumnIgnoreOperationPropertyType, Type> ProxyTypes { get; }

        public AutumnEntityInfo(Type entityType, Dictionary<AutumnIgnoreOperationPropertyType, Type> proxyTypes,
            AutumnEntityAttribute entityAttribute,
            AutumnEntityKeyInfo keyInfo,
            AutumnIgnoreOperationAttribute ignoreOperationAttribute = null)
        {
            IgnoreOperations = ignoreOperationAttribute?.OperationTypes;
            EntityType = entityType;
            ProxyTypes = proxyTypes;
            ApiVersion =
                Regex.Match(entityAttribute.Version ?? string.Empty, "v[0-9]+", RegexOptions.IgnoreCase).Success
                    ? entityAttribute.Version
                    : AutumnSettings.Current.DefaultApiVersion;
            Name = entityAttribute.Name ?? entityType.Name;
            KeyInfo = keyInfo;
            ControllerName = Name.ToCase(AutumnSettings.Current.NamingStrategy);
            if (AutumnSettings.Current.PluralizeController && !ControllerName.EndsWith("s"))
            {
                ControllerName += "s";
            }
        }
    }
}