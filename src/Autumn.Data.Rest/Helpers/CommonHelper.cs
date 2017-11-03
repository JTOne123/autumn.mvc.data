using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Autumn.Data.Rest.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Autumn.Data.Rest.Helpers
{
    public static class CommonHelper
    {
        private static readonly Dictionary<Type, Dictionary<string,PropertyInfo>> MappingJson2PropertyInfo =
            new Dictionary<Type, Dictionary<string,PropertyInfo>> ();

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

        private static PropertyInfo GetProperty(Type type, string name, NamingStrategy namingStrategy = null)
        {
            lock (MappingJson2PropertyInfo)
            {
                if (MappingJson2PropertyInfo.ContainsKey(type)) return MappingJson2PropertyInfo[type][name];
                MappingJson2PropertyInfo[type] = Build(type, namingStrategy);
                return MappingJson2PropertyInfo[type][name];
            }
        }

        #endregion

        #region GetExpressionModelBinder

        public static IModelBinder GetExpressionModelBinder(Type type, string queryField,
            NamingStrategy namingStrategy)
        {
            lock (ExpressionModelBinders)
            {
                if (ExpressionModelBinders.ContainsKey(type)) return ExpressionModelBinders[type];
                var entityType = type
                    .GetGenericArguments()[0]
                    .GetGenericArguments()[0];

                var binderBaseType = typeof(QueryModelBinder<>);

                var binderTypeName = string.Format("{0}[[{2}, {3}]], {1}",
                    binderBaseType.FullName,
                    binderBaseType.Assembly.FullName,
                    entityType.FullName,
                    entityType.Assembly.FullName);

                var modelBinderType = Type.GetType(binderTypeName);
                var constructorInfo = modelBinderType.GetConstructor(new[]
                    {typeof(string), typeof(NamingStrategy)});


                ExpressionModelBinders.Add(
                    type,
                    (IModelBinder) constructorInfo.Invoke(new object[]
                    {
                        queryField,
                        namingStrategy
                    })
                );

                return ExpressionModelBinders[type];
            }
        }

        #endregion

        #region GetPageableModelBinder

        public static IModelBinder GetPageableModelBinder(Type type, string pageSizeField,
            string pageNumberField, string sortField,
            NamingStrategy namingStrategy)
        {
            lock (PageableModelBinders)
            {
                if (PageableModelBinders.ContainsKey(type)) return PageableModelBinders[type];
                var entityType = type
                    .GetGenericArguments()[0];

                var binderBaseType = typeof(PageableModelBinder<>);

                var binderTypeName = string.Format("{0}[[{2}, {3}]], {1}",
                    binderBaseType.FullName,
                    binderBaseType.Assembly.FullName,
                    entityType.FullName,
                    entityType.Assembly.FullName);

                var modelBinderType = Type.GetType(binderTypeName);
                var constructorInfo = modelBinderType.GetConstructor(new[]
                    {typeof(string), typeof(string), typeof(string), typeof(NamingStrategy)});

                PageableModelBinders.Add(type,
                    (IModelBinder) constructorInfo.Invoke(new object[]
                    {
                        pageSizeField,
                        pageNumberField, sortField,
                        namingStrategy
                    })
                );

                return PageableModelBinders[type];
            }
        }

        #endregion
        
        public class ExpressionValue
        {
            public PropertyInfo Property { get; set; }
            public Expression Expression { get; set; }
        }

        public static ExpressionValue GetMemberExpressionValue<T>(ParameterExpression parameter, string selector,
            NamingStrategy namingStrategy)
        {
            Expression lastMember = parameter;
            PropertyInfo property = null;
            var type = typeof(T);
            if (selector.IndexOf(".", StringComparison.InvariantCulture) != -1)
            {
                foreach (var item in selector.Split('.'))
                {
                    property = GetProperty(type, item, namingStrategy);
                    type = property.PropertyType;
                    lastMember = Expression.Property(lastMember, property);
                }
            }
            else
            {
                property = GetProperty(type, selector, namingStrategy);
                lastMember = Expression.Property(lastMember, property);
            }

            return new ExpressionValue()
            {
                Property = property,
                Expression = lastMember
            };
        }
    }
}