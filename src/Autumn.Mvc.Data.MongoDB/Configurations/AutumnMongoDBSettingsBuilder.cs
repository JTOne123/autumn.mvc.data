using Autumn.Mvc.Data.Configurations;
using MongoDB.Bson.Serialization.Conventions;

namespace Autumn.Mvc.Data.MongoDB.Configurations
{
    public class AutumnMongoDBSettingsBuilder
    {
        private  AutumnMongoDBSettings _settings;

        public AutumnMongoDBSettingsBuilder(AutumnDataSettings parent)
        {
            _settings = new AutumnMongoDBSettings(parent);
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
        
        public AutumnMongoDBSettingsBuilder Convention(IConvention convention)
        {
            _settings.Convention = convention;
            return this;
        }

        public AutumnMongoDBSettings Build()
        {
            return _settings;
        }

    }
}