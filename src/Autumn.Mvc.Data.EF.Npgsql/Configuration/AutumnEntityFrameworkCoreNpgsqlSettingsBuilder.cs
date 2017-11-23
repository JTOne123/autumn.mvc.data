namespace Autumn.Mvc.Data.EF.Npgsql.Configuration
{
    public class AutumnEntityFrameworkCoreNpgsqlSettingsBuilder
    {
        private AutumnEntityFrameworkCoreNpgsqlSettings _settings;

        public AutumnEntityFrameworkCoreNpgsqlSettingsBuilder()
        {
            _settings=new AutumnEntityFrameworkCoreNpgsqlSettings();
        }

        public AutumnEntityFrameworkCoreNpgsqlSettingsBuilder ConnectionString(string connectionString)
        {
            _settings.ConnectionString = connectionString;
            return this;
        }

        public AutumnEntityFrameworkCoreNpgsqlSettingsBuilder Evolve(bool value=true)
        {
            _settings.Evolve = value;
            return this;
        }

        public AutumnEntityFrameworkCoreNpgsqlSettings Build()
        {
            return _settings;
        }
    }
}