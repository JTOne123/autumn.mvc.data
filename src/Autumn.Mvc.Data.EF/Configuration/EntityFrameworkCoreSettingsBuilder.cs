using Autumn.Mvc.Data.Configurations;

namespace Autumn.Mvc.Data.EF.Configuration
{
    public class EntityFrameworkCoreSettingsBuilder
    {
        private EntityFrameworkCoreSettings _settings;

        public EntityFrameworkCoreSettingsBuilder(AutumnDataSettings parent)
        {
            _settings = new EntityFrameworkCoreSettings(parent);
        }

        public EntityFrameworkCoreSettingsBuilder ConnectionString(string connectionString)
        {
            _settings.ConnectionString = connectionString;
            return this;
        }

        public EntityFrameworkCoreSettingsBuilder Evolve(bool use=true)
        {
            _settings.UseEvolve = use;
            return this;
        }

        public EntityFrameworkCoreSettings Build()
        {
            return _settings;
        }
    }
}