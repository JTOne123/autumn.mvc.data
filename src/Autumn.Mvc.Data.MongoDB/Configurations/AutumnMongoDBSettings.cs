using System;
using Autumn.Mvc.Data.Configurations;

namespace Autumn.Mvc.Data.MongoDB.Configurations
{
    public class AutumnMongoDBSettings
    {
        public AutumnDataSettings Parent { get; private set; }
        public string ConnectionString { get; set; }
        public string Database { get; set; }

        public AutumnMongoDBSettings(AutumnDataSettings parent)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }
    }
}