using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Autumn.Mvc.Data.Configurations.Exceptions;
using Newtonsoft.Json.Serialization;

namespace Autumn.Mvc.Data.Configurations
{
    public class AutumnSettingsBuilder
    {

        private readonly AutumnSettings _autumnSettings;

        /// <summary>
        /// class initializer 
        /// </summary>
        public AutumnSettingsBuilder()
        {
            _autumnSettings = new AutumnSettings();
        }

        /// <summary>
        /// build result
        /// </summary>
        /// <returns></returns>
        public AutumnSettings Build()
        {
            return _autumnSettings;
        }
        
        /// <summary>
        /// configuration pluralize controllers
        /// </summary>
        /// <param name="pluralized"></param>
        /// <returns></returns>
        public AutumnSettingsBuilder Pluralized(bool pluralized = true)
        {
            _autumnSettings.PluralizeController = pluralized;
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
            _autumnSettings.PageSizeFieldName = parameterName;
            return this;
        }

        /// <summary>
        /// configuration of page number
        /// </summary>
        /// <param name="pageNumberFieldName">query parameter id for page number</param>
        /// <returns></returns>
        public AutumnSettingsBuilder PageNumberFieldName(string pageNumberFieldName)
        {
            if (string.IsNullOrWhiteSpace(pageNumberFieldName))
            {
                throw new ArgumentNullException(nameof(pageNumberFieldName));
            }
            _autumnSettings.PageNumberFieldName = pageNumberFieldName;
            return this;
        }


        /// <summary>
        /// configuraion of sort field
        /// </summary>
        /// <param name="sortFieldName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public AutumnSettingsBuilder SortFieldName(string sortFieldName)
        {
            if (string.IsNullOrWhiteSpace(sortFieldName))
            {
                throw new ArgumentNullException(nameof(sortFieldName));
            }
            _autumnSettings.SortFieldName = sortFieldName;
            return this;
        }
        
        /// <summary>
        /// configuration of query field name
        /// </summary>
        /// <param name="queryFieldName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public AutumnSettingsBuilder QueryFieldName(string queryFieldName)
        {
            if (string.IsNullOrWhiteSpace(queryFieldName))
            {
                throw new ArgumentNullException(nameof(queryFieldName));
            }
            _autumnSettings.QueryFieldName = queryFieldName;
            return this;
        }

        /// <summary>
        /// configuration of entity assembly
        /// </summary>
        /// <param name="entityAssembly"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public AutumnSettingsBuilder EntityAssembly(Assembly entityAssembly)
        {
            if (entityAssembly==null)
            {
                throw new ArgumentNullException(nameof(entityAssembly));
            }
            _autumnSettings.EntityAssembly = entityAssembly;
            return this;
        }

        /// <summary>
        /// configuration default api version
        /// </summary>
        /// <param name="defaultApiVersion"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="AutumnSettingsException"></exception>
        public AutumnSettingsBuilder DefaultApiVersion(string defaultApiVersion)
        {
            if (string.IsNullOrWhiteSpace(defaultApiVersion))
            {
                throw new ArgumentNullException(nameof(defaultApiVersion));
            }
            if (!Regex.IsMatch(defaultApiVersion, "v[0-9]+"))
            {
                throw new AutumnSettingsException("invalid version see regular expression v[0-9]+");
            }
            _autumnSettings.DefaultApiVersion = defaultApiVersion;
            return this;
        }

        /// <summary>
        /// configuration of naming strategy
        /// </summary>
        /// <param name="namingStrategy"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public AutumnSettingsBuilder NamingStrategy(NamingStrategy namingStrategy)
        {
            if (namingStrategy==null)
            {
                throw new ArgumentNullException(nameof(namingStrategy));
            }
            _autumnSettings.NamingStrategy = namingStrategy;
            return this;
        }

        /// <summary>
        /// configuration swagger
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public AutumnSettingsBuilder Swagger(bool value=true)
        {
            _autumnSettings.UseSwagger = value;
            return this;
        }
    }
}