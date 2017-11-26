using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace Autumn.Mvc.Data.Configurations
{
    public class AutumnSettingsBuilder
    {
        public void Build(Assembly callingAssembly)
        {
            AutumnSettings.Build(callingAssembly);
        }

        public AutumnSettingsBuilder Pluralized(bool pluralized = true)
        {
            AutumnSettings.Current.PluralizeController = pluralized;
            return this;
        }

        /// <summary>
        /// configuration of page size
        /// </summary>
        /// <param name="parameterName">query parameter id for page size</param>
        /// <param name="pageSize">default page size</param>
        /// <returns></returns>
        public AutumnSettingsBuilder PageSizeFieldName(string parameterName,
            int? pageSize = null)
        {
            AutumnSettings.Current.PageSizeFieldName = parameterName;
            AutumnSettings.Current.DefaultPageSize = pageSize ?? AutumnSettings.Current.DefaultPageSize;
            return this;
        }

        /// <summary>
        /// configuration of page number
        /// </summary>
        /// <param name="parameterName">query parameter id for page number</param>
        /// <returns></returns>
        public AutumnSettingsBuilder PageNumberFieldName(string parameterName)
        {
            AutumnSettings.Current.PageNumberFieldName = parameterName;
            return this;
        }

        public AutumnSettingsBuilder SortFieldName(string parameterName)
        {
            AutumnSettings.Current.SortFieldName = parameterName;
            return this;
        }
        
        public AutumnSettingsBuilder QueryFieldName(string parameterName)
        {
            AutumnSettings.Current.QueryFieldName = parameterName;
            return this;
        }

        public AutumnSettingsBuilder EntityAssembly(Assembly assembly)
        {
            AutumnSettings.Current.EntityAssembly = assembly;
            return this;
        }

        public AutumnSettingsBuilder DefaultApiVersion(string defaultApiVersion)
        {
            AutumnSettings.Current.DefaultApiVersion = defaultApiVersion ?? AutumnSettings.Current.DefaultApiVersion;
            return this;
        }

        public AutumnSettingsBuilder NamingStrategy(NamingStrategy namingStrategy)
        {
            AutumnSettings.Current.NamingStrategy = namingStrategy ?? AutumnSettings.Current.NamingStrategy;
            return this;
        }

        public AutumnSettingsBuilder Swagger(bool value=true)
        {
            AutumnSettings.Current.UseSwagger = value;
            return this;
        }
    }
}