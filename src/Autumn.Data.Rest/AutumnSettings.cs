using System;
using Newtonsoft.Json.Serialization;

namespace Autumn.Data.Rest.Configurations
{
    public class AutumnSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public NamingStrategy NamingStrategy { get; set; }
        public Type RepositoryType { get; set; }
    }
}