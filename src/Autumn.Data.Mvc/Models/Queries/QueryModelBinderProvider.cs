using System.Linq.Expressions;
using System.Reflection;
using Autumn.Data.Mvc.Models.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Serialization;

namespace Autumn.Data.Mvc.Models.Queries
{
    public class QueryModelBinderProvider : IModelBinderProvider
    {
        
        private readonly string _queryField;
        private readonly NamingStrategy _namingStrategy;
       
        public QueryModelBinderProvider(IConfiguration configuration, NamingStrategy namingStrategy = null)
        {
            _queryField = configuration.GetSection("Autumn.Data.Mvc:QueryParameterName").Value ?? "Query";
            _namingStrategy = namingStrategy;
            if (_namingStrategy == null) return;
            var method = _namingStrategy.GetType().GetMethod("ResolvePropertyName",BindingFlags.Default|BindingFlags.Instance|BindingFlags.NonPublic);
            if (method == null) return;
            _queryField = (string) method.Invoke(_namingStrategy,new object[]{_queryField});
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (!context.Metadata.ModelType.IsGenericType ||
                context.Metadata.ModelType.GetGenericTypeDefinition() != typeof(Expression<>)) return null;
            return CommonHelper.GetExpressionModelBinder(context.Metadata.ModelType, _queryField, _namingStrategy);
        }
    }
}