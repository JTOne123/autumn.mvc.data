namespace Autumn.Mvc.Data.EF.SqlServer.Configuration
{
    public class EntityFrameworkCoreSqlServerSettingsBuilder
    {
        private EntityFrameworkCoreSqlServerSettings _settings;

        public EntityFrameworkCoreSqlServerSettingsBuilder()
        {
            _settings=new EntityFrameworkCoreSqlServerSettings();
        }

        public EntityFrameworkCoreSqlServerSettingsBuilder ConnectionString(string connectionString)
        {
            _settings.ConnectionString = connectionString;
            return this;
        }

        public EntityFrameworkCoreSqlServerSettingsBuilder Evolve(bool value=true)
        {
            _settings.Evolve = value;
            return this;
        }

        public EntityFrameworkCoreSqlServerSettings Build()
        {
            return _settings;
        }
    }
}