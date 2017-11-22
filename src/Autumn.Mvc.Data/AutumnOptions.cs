using System.Reflection;
using Autumn.Mvc.Data.Configurations;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace Autumn.Mvc.Data
{
    public class AutumnOptions
    {
        public IServiceCollection ServiceCollection { get; private set; }
        
        public AutumnOptions(IServiceCollection collection)
        {
            ServiceCollection = collection;
        }
        
        public AutumnOptions UsePluralizationController(bool pluralized)
        {
            AutumnSettings.Instance.PluralizeController = pluralized;
            return this;
        }

        /// <summary>
        /// configuration of page size
        /// </summary>
        /// <param name="parameterName">query parameter id for page size</param>
        /// <param name="pageSize">default page size</param>
        /// <returns></returns>
        public AutumnOptions UsePageSizeFieldDescription(string parameterName,
            int? pageSize = null)
        {
            AutumnSettings.Instance.PageSizeFieldName = parameterName ?? AutumnSettings.Instance.PageSizeFieldName;
            AutumnSettings.Instance.DefaultPageSize = pageSize ?? AutumnSettings.Instance.DefaultPageSize;
            return this;
        }


        /// <summary>
        /// configuration of page number
        /// </summary>
        /// <param name="parameterName">query parameter id for page number</param>
        /// <returns></returns>
        public AutumnOptions UsePageNumberFieldDescription(string parameterName)
        {
            AutumnSettings.Instance.PageNumberFieldName = parameterName ?? AutumnSettings.Instance.PageNumberFieldName;
            return this;
        }

        public AutumnOptions UseSortFieldDescription(string parameterName)
        {
            AutumnSettings.Instance.SortFieldName = parameterName ?? AutumnSettings.Instance.SortFieldName;
            return this;
        }
        
        public AutumnOptions UseQueryFieldDescription(string parameterName)
        {
            AutumnSettings.Instance.QueryFieldName = parameterName ?? AutumnSettings.Instance.QueryFieldName;
            return this;
        }

        public AutumnOptions UseEntityAssembly(Assembly assembly)
        {
            AutumnSettings.Instance.EntityAssembly = assembly;
            return this;
        }

        public AutumnOptions UseApiVersion(string defaultApiVersion)
        {
            AutumnSettings.Instance.DefaultApiVersion = defaultApiVersion ?? AutumnSettings.Instance.DefaultApiVersion;
            return this;
        }
      
        public AutumnOptions UseNamingStrategy(NamingStrategy namingStrategy)
        {
            AutumnSettings.Instance.NamingStrategy = namingStrategy ?? AutumnSettings.Instance.NamingStrategy;
            return this;
        }
    
        
    }
}