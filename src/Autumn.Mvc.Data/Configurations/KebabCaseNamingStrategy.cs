using Newtonsoft.Json.Serialization;

namespace Autumn.Mvc.Data.Configurations
{
    public class KebabCaseNamingStrategy : SnakeCaseNamingStrategy
    {
        public KebabCaseNamingStrategy(bool processDictionaryKeys, bool overrideSpecifiedNames) : base(
            processDictionaryKeys, overrideSpecifiedNames)
        {
        }

        public KebabCaseNamingStrategy(bool processDictionaryKeys, bool overrideSpecifiedNames, bool processExtensionDataNames)
            : base(processDictionaryKeys, overrideSpecifiedNames,processExtensionDataNames)
        {
        }

        public KebabCaseNamingStrategy()
        {
        }
        
        /// <summary>Resolves the specified property name.</summary>
        /// <param name="name">The property name to resolve.</param>
        /// <returns>The resolved property name.</returns>
        protected override string ResolvePropertyName(string name)
        {
            return base.ResolvePropertyName(name).Replace('_', '-');
        }
    }
}