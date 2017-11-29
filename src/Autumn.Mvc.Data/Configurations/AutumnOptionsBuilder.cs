using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Serialization;

namespace Autumn.Mvc.Data.Configurations
{
    public class AutumnOptionsBuilder
    {

        private readonly AutumnOptions _autumnSettings;

        /// <summary>
        /// class initializer 
        /// </summary>
        public AutumnOptionsBuilder()
        {
            _autumnSettings = new AutumnOptions();
        }

        /// <summary>
        /// build result
        /// </summary>
        /// <returns></returns>
        public AutumnOptions Build()
        {
            return _autumnSettings;
        }
        
        /// <summary>
        /// configuration pluralize controllers
        /// </summary>
        /// <param name="pluralized"></param>
        /// <returns></returns>
        public AutumnOptionsBuilder Pluralized(bool pluralized = true)
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
        public AutumnOptionsBuilder PageSizeFieldName(string parameterName,
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
        public AutumnOptionsBuilder PageNumberFieldName(string pageNumberFieldName)
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
        public AutumnOptionsBuilder SortFieldName(string sortFieldName)
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
        public AutumnOptionsBuilder QueryFieldName(string queryFieldName)
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
        public AutumnOptionsBuilder EntityAssembly(Assembly entityAssembly)
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
        /// <exception cref="ArgumentException"></exception>
        public AutumnOptionsBuilder DefaultApiVersion(string defaultApiVersion)
        {
            if (string.IsNullOrWhiteSpace(defaultApiVersion))
            {
                throw new ArgumentNullException(nameof(defaultApiVersion));
            }
            if (!Regex.IsMatch(defaultApiVersion, "v[0-9]+"))
            {
                throw new ArgumentException("invalid version see regular expression v[0-9]+");
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
        public AutumnOptionsBuilder NamingStrategy(NamingStrategy namingStrategy)
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
        public AutumnOptionsBuilder Swagger(bool value=true)
        {
            _autumnSettings.UseSwagger = value;
            return this;
        }
    }
}