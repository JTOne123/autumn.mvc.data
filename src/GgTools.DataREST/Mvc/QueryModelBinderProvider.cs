using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Serialization;

namespace GgTools.DataREST.Mvc
{
    public class QueryModelBinderProvider : IModelBinderProvider
    {
        private static readonly Dictionary<Type, ConstructorInfo> _binderCacheTypes = new Dictionary<Type, ConstructorInfo>();
        private readonly string _queryField;
        private readonly NamingStrategy _namingStrategy;
       
        public QueryModelBinderProvider(IConfiguration configuration, NamingStrategy namingStrategy = null)
        {
            _queryField = configuration.GetSection("Ggtools.DataREST.Settings:Query_Name").Value ?? "Query";
            _namingStrategy = namingStrategy;
            if (_namingStrategy == null) return;
            var method = _namingStrategy.GetType().GetMethod("ResolvePropertyName",BindingFlags.Default|BindingFlags.Instance|BindingFlags.NonPublic);
            if (method == null) return;
            _queryField = (string) method.Invoke(_namingStrategy,new object[]{_queryField});
        }
        
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType.IsGenericType && context.Metadata.ModelType.GetGenericTypeDefinition()==typeof(Expression<>))
            {
                if (!_binderCacheTypes.ContainsKey(context.Metadata.ModelType))
                {
                    var entityType = context.Metadata.ModelType
                        .GetGenericArguments()[0]
                        .GetGenericArguments()[0];
                    var binderTypeName = string.Format("{0}[[{2}, {3}]], {1}",
                        typeof(QueryModelBinder<>).FullName,
                        typeof(QueryModelBinder<>).Assembly.FullName,
                        entityType.FullName,
                        entityType.Assembly.FullName);
                    var binderType =
                        Type.GetType(binderTypeName);
                    _binderCacheTypes[context.Metadata.ModelType] = binderType.GetConstructor(new Type[]{typeof(string),typeof(NamingStrategy)});
                }
                return (IModelBinder) _binderCacheTypes[context.Metadata.ModelType].Invoke(new object[]{_queryField,_namingStrategy});
            }
            return null;
        }
    }
}