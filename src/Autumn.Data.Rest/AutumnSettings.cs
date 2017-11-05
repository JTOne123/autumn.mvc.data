using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Autumn.Data.Rest.Configurations
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
        [JsonIgnore]
        public NamingStrategy NamingStrategy { get; set; }
        public string ApiVersion { get; set; }
        public bool PluralizeController { get; set; }
    }
}