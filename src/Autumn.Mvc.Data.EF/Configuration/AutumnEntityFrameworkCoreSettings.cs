using System;
using Autumn.Mvc.Data.Configurations;

namespace Autumn.Mvc.Data.EF.Configuration
{
    public class AutumnEntityFrameworkCoreSettings
    {
        public AutumnDataSettings Parent { get; private set; }
        public string ConnectionString { get; set; }

        public AutumnEntityFrameworkCoreSettings(AutumnDataSettings parent)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }
    }
}