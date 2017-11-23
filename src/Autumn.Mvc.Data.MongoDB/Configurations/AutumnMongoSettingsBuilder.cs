namespace Autumn.Mvc.Data.MongoDB.Configurations
{
    public class AutumnMongoDBSettingsBuilder
    {
        private AutumnMongoSettings _settings;

        public AutumnMongoDBSettingsBuilder()
        {
            _settings=new AutumnMongoSettings();
        }

        public AutumnMongoDBSettingsBuilder ConnectionString(string connectionString)
        {
            _settings.ConnectionString = connectionString;
            return this;
        }

        public AutumnMongoDBSettingsBuilder Database(string database)
        {
            _settings.Database = database;
            return this;
        }

        public AutumnMongoSettings Build()
        {
            return _settings;
        }
    }
}