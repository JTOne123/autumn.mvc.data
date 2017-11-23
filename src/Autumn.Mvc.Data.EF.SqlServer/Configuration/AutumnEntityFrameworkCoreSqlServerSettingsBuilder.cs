namespace Autumn.Mvc.Data.EF.SqlServer.Configuration
{
    public class AutumnEntityFrameworkCoreSqlServerSettingsBuilder
    {
        private AutumnEntityFrameworkCoreSqlServerSettings _settings;

        public AutumnEntityFrameworkCoreSqlServerSettingsBuilder()
        {
            _settings=new AutumnEntityFrameworkCoreSqlServerSettings();
        }

        public AutumnEntityFrameworkCoreSqlServerSettingsBuilder ConnectionString(string connectionString)
        {
            _settings.ConnectionString = connectionString;
            return this;
        }

        public AutumnEntityFrameworkCoreSqlServerSettingsBuilder Evolve(bool value=true)
        {
            _settings.Evolve = value;
            return this;
        }

        public AutumnEntityFrameworkCoreSqlServerSettings Build()
        {
            return _settings;
        }
    }
}