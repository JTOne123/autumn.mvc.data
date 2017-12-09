using System;
using System.Collections.Concurrent;
using Autumn.Mvc.Configurations;

namespace Autumn.Mvc.Data.Configurations
{
    public static class AutumnSettingsExtentions
    {
        private static readonly ConcurrentDictionary<AutumnSettings, AutumnDataSettings> dataSettings =
            new ConcurrentDictionary<AutumnSettings, AutumnDataSettings>();


        public static AutumnDataSettings DataSettings(this AutumnSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            return dataSettings.GetOrAdd(settings, new AutumnDataSettings(settings));
        }

    }
}