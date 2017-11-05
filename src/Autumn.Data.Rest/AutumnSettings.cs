using Newtonsoft.Json.Serialization;

namespace Autumn.Data.Rest.Configurations
{
    public class AutumnSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public NamingStrategy NamingStrategy { get; set; }
        public string ApiVersion { get; set; }
        public bool PluralizeController { get; set; }

        public AutumnSettings()
        {
            PluralizeController = true;
        }
    }
}