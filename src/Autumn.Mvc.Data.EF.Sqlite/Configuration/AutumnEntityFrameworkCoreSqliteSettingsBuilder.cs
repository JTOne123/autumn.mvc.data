namespace Autumn.Mvc.Data.EF.Sqlite.Configuration
{
    public class AutumnEntityFrameworkCoreSqliteSettingsBuilder
    {
        private AutumnEntityFrameworkCoreSqliteSettings _settings;

        public AutumnEntityFrameworkCoreSqliteSettingsBuilder()
        {
            _settings=new AutumnEntityFrameworkCoreSqliteSettings();
        }

        public AutumnEntityFrameworkCoreSqliteSettingsBuilder ConnectionString(string connectionString)
        {
            _settings.ConnectionString = connectionString;
            return this;
        }

        public AutumnEntityFrameworkCoreSqliteSettingsBuilder Evolve(bool value=true)
        {
            _settings.Evolve = value;
            return this;
        }

        public AutumnEntityFrameworkCoreSqliteSettings Build()
        {
            return _settings;
        }
    }
}