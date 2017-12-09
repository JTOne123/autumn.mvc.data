using Autumn.Mvc.Data.Configurations;

namespace Autumn.Mvc.Data.EF.Configuration
{
    public class AutumnEntityFrameworkCoreSettingsBuilder
    {
        private AutumnEntityFrameworkCoreSettings _settings;

        public AutumnEntityFrameworkCoreSettingsBuilder(AutumnDataSettings parent)
        {
            _settings = new AutumnEntityFrameworkCoreSettings(parent);
        }

        public AutumnEntityFrameworkCoreSettingsBuilder ConnectionString(string connectionString)
        {
            _settings.ConnectionString = connectionString;
            return this;
        }

        public AutumnEntityFrameworkCoreSettings Build()
        {
            return _settings;
        }
    }
}