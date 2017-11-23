namespace Autumn.Mvc.Data.EF.Mysql.Configuration
{
    public class AutumnEntityFrameworkCoreMysqlSettingsBuilder
    {
        private AutumnEntityFrameworkCoreMysqlSettings _settings;

        public AutumnEntityFrameworkCoreMysqlSettingsBuilder()
        {
            _settings = new AutumnEntityFrameworkCoreMysqlSettings();
        }

        public AutumnEntityFrameworkCoreMysqlSettingsBuilder ConnectionString(string connectionString)
        {
            _settings.ConnectionString = connectionString;
            return this;
        }

        public AutumnEntityFrameworkCoreMysqlSettingsBuilder Evolve(bool value = true)
        {
            _settings.Evolve = value;
            return this;
        }

        public AutumnEntityFrameworkCoreMysqlSettings Build()
        {
            return _settings;
        }
    }
}