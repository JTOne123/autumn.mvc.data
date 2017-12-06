using System;
using Autumn.Mvc.Data.Configurations;

namespace Autumn.Mvc.Data.EF.Configuration
{
    public class EntityFrameworkCoreSettings
    {
        public AutumnDataSettings Parent { get; private set; }
        public string ConnectionString { get; set; }

        public EntityFrameworkCoreSettings(AutumnDataSettings parent)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }
    }
}