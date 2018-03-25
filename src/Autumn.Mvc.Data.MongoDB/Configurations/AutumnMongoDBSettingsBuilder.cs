using System;
using System.Collections.Generic;
using System.Reflection;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.MongoDB.Annotations;
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
             _settings.CollectionInfos = new Dictionary<Type, CollectionInfo>();
            foreach (var type in _settings.Parent.EntitiesInfos.Keys)
            {
                var collectionAttribute = type.GetCustomAttribute(typeof(CollectionAttribute)) as CollectionAttribute;
                var entityInfo = _settings.Parent.EntitiesInfos[type];
                _settings.CollectionInfos.Add(type,
                    collectionAttribute != null
                        ? new CollectionInfo(entityInfo, collectionAttribute.Name)
                        : new CollectionInfo(entityInfo, entityInfo.Name));
            }
            return _settings;
        }

    }
}