using System;
using System.Collections.Generic;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.MongoDB.Annotations;
using MongoDB.Bson.Serialization.Conventions;

namespace Autumn.Mvc.Data.MongoDB.Configurations
{
    public class AutumnMongoDBSettings
    {
        public AutumnDataSettings Parent { get;  }
        public string ConnectionString { get; set; }
        public string Database { get; set; }
        public IConvention Convention { get; set; }
        public Dictionary<Type,CollectionInfo> CollectionInfos { get; set; }

        public AutumnMongoDBSettings(AutumnDataSettings parent)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }
    }
}