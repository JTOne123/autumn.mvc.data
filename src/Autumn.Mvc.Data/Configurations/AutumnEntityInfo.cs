using System;
using System.Collections.Generic;
using Autumn.Mvc.Data.Annotations;
using Autumn.Mvc.Data.Helpers;

namespace Autumn.Mvc.Data.Configurations
{
    public class AutumnEntityInfo
    {
        public Type EntityType { get; }
        public string Version { get;  }
        public string Name { get; }
        public string ControllerName { get; }
        public AutumnEntityKeyInfo KeyInfo { get;  }
        public Dictionary<AutumnIgnoreType,Type> ProxyTypes { get;  }

        public AutumnEntityInfo(AutumnSettings settings, Type entityType, Dictionary<AutumnIgnoreType, Type> proxyTypes,
            AutumnEntityAttribute entityAttribute,
            AutumnEntityKeyInfo keyInfo)
        {
            EntityType = entityType;
            ProxyTypes = proxyTypes;
            Version = entityAttribute.Version ?? settings.DefaultApiVersion;
            Name = entityAttribute.Name;
            KeyInfo = keyInfo;
            ControllerName = Name.ToCase(settings.NamingStrategy);
            if (settings.PluralizeController && !ControllerName.EndsWith("s"))
            {
                ControllerName += "s";
            }
        }
    }
}