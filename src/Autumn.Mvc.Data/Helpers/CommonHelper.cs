using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Autumn.Mvc.Data.Models.Paginations;
using Autumn.Mvc.Data.Models.Queries;
using Autumn.Mvc.Data.Models.Queries.Exceptions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Autumn.Mvc.Data.Helpers
{
    public static class CommonHelper
    {
        private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> MappingJson2PropertyInfo =
            new Dictionary<Type, Dictionary<string, PropertyInfo>>();

        private static readonly Dictionary<Type, IModelBinder> ExpressionModelBinders =
            new Dictionary<Type, IModelBinder>();

        private static readonly Dictionary<Type, IModelBinder> PageableModelBinders =
            new Dictionary<Type, IModelBinder>();

        public static Expression<Func<T, bool>> True<T>()
        {
            return f => true;
        }

        public static readonly MemoryCache QueriesCache =
            new MemoryCache(new MemoryCacheOptions() {ExpirationScanFrequency = TimeSpan.FromMinutes(5)});

        #region GetProperty 

        private static Dictionary<string, PropertyInfo> Build(IReflect type, NamingStrategy namingStrategy = null)
        {
            var result = new Dictionary<string, PropertyInfo>();
            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var jsonExclude = property.GetCustomAttribute<JsonIgnoreAttribute>();
                if (jsonExclude != null) continue;
                var jsonPropertyName = GetJsonPropertyName(property, namingStrategy);
                result.Add(jsonPropertyName, property);
            }
            return result;
        }


        private static string GetJsonPropertyName(MemberInfo propertyInfo,
            NamingStrategy namingStrategy = null)
        {
            var propertyName = propertyInfo.Name;
            var attribute = propertyInfo.GetCustomAttribute<JsonPropertyAttribute>();
            if (attribute != null)
            {
                propertyName = attribute.PropertyName;
            }
            if (namingStrategy == null) return propertyName;
            switch (namingStrategy)
            {
                case SnakeCaseNamingStrategy _:
                    propertyName = propertyName.ToSnakeCase();
                    break;

                case CamelCaseNamingStrategy _:
                    propertyName = propertyName.ToCamelCase();
                    break;
            }
            return propertyName;
        }

        private static PropertyInfo GetOrRegistryProperty(Type type, string name, NamingStrategy namingStrategy = null)
        {
            lock (MappingJson2PropertyInfo)
            {
                if (MappingJson2PropertyInfo.ContainsKey(type)) return MappingJson2PropertyInfo[type][name];
                MappingJson2PropertyInfo[type] = Build(type, namingStrategy);
                return MappingJson2PropertyInfo[type].ContainsKey(name) ? MappingJson2PropertyInfo[type][name] : null;
            }
        }

        #endregion

        #region GetExpressionModelBinder

        public static IModelBinder GetExpressionModelBinder(Type type)
        {
            lock (ExpressionModelBinders)
            {
                if (ExpressionModelBinders.ContainsKey(type)) return ExpressionModelBinders[type];
                var entityType = type
                    .GetGenericArguments()[0]
                    .GetGenericArguments()[0];
                var modelBinderType = typeof(QueryModelBinder<>).MakeGenericType(entityType);
                ExpressionModelBinders.Add(type,(IModelBinder)Activator.CreateInstance(modelBinderType));
                return ExpressionModelBinders[type];
            }
        }

        #endregion

        #region GetPageableModelBinder

        public static IModelBinder GetPageableModelBinder(Type type)
        {
            lock (PageableModelBinders)
            {
                if (PageableModelBinders.ContainsKey(type)) return PageableModelBinders[type];
                var entityType = type
                    .GetGenericArguments()[0];
                var modelBinderType = typeof(PageableModelBinder<>).MakeGenericType(entityType);
                PageableModelBinders.Add(type,(IModelBinder) Activator.CreateInstance(modelBinderType));
                return PageableModelBinders[type];
            }
        }

        #endregion

        public class ExpressionValue
        {
            public PropertyInfo Property { get; set; }
            public Expression Expression { get; set; }
        }

        public static ExpressionValue GetMemberExpressionValue<T>(ParameterExpression parameter,
            string selector,
            NamingStrategy namingStrategy = null)
        {
            if (parameter == null) throw new ArgumentException("parameter");
            if (selector == null) throw new ArgumentException("selector");

            Expression lastMember = parameter;
            PropertyInfo property = null;
            var type = typeof(T);
            if (selector.IndexOf(".", StringComparison.InvariantCulture) != -1)
            {
                foreach (var item in selector.Split('.'))
                {
                    property = GetOrRegistryProperty(type, item, namingStrategy);
                    if (property == null)
                    {
                        throw new Exception(string.Format("Invalid property {0}", selector));
                    }
                    type = property.PropertyType;
                    lastMember = Expression.Property(lastMember, property);
                }
            }
            else
            {
                property = GetOrRegistryProperty(type, selector, namingStrategy);
                if (property == null)
                {
                    throw new Exception(string.Format("Invalid property {0}", selector));
                }
                lastMember = Expression.Property(lastMember, property);
            }

            return new ExpressionValue()
            {
                Property = property,
                Expression = lastMember
            };
        }

        public static ExpressionValue GetMemberExpressionValue<T>(ParameterExpression parameter,
            QueryParser.ComparisonContext context,
            NamingStrategy namingStrategy = null)
        {
            if (parameter == null) throw new ArgumentException("parameter");
            if (context == null) throw new ArgumentException("context");
            try
            {
                return GetMemberExpressionValue<T>(parameter, context.selector().GetText(), namingStrategy);
            }
            catch (Exception e)
            {
                throw new QueryComparisonInvalidComparatorSelectionException(context, e);
            }
        }
    }
}