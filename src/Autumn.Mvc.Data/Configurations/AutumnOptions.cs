using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace Autumn.Mvc.Data.Configurations
{
    public class AutumnOptions
    {
        public string PageSizeFieldName { get; set; }
        public string SortFieldName { get; set; }
        public string PageNumberFieldName { get; set; }
        public string QueryFieldName { get; set; }
        public int DefaultPageSize { get; set; }
        public bool PluralizeController { get; set; }
        public NamingStrategy NamingStrategy { get; set; }
        public bool UseSwagger { get; set; }
        public string DefaultApiVersion { get; set; }
        public Assembly EntityAssembly { get; set; }
    }
}