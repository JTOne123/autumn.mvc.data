using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Newtonsoft.Json.Serialization;

namespace Autumn.Mvc.Data.Configurations
{
    public class AutumnSettings
    {
        public static AutumnSettings Instance { get; }

        static AutumnSettings()
        {
            Instance = new AutumnSettings();
        }

        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public NamingStrategy NamingStrategy { get; set; }
        public string ApiVersion { get; set; }
        public bool PluralizeController { get; set; }
        public Dictionary<Type,AttributeRouteModel> Routes { get; set; }
        public Assembly EntityAssembly { get; set; }

    }
}