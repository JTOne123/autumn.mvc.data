using System;
using System.Collections.Generic;
using System.Linq;
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
        private static readonly List<Type> PrimitiveTypes =
            new List<Type>(new[]
                {
                    typeof(bool),
                    typeof(bool?),
                    typeof(bool[]),
                    typeof(bool?[]),
                    typeof(string),
                    typeof(string[]),
                    typeof(short),
                    typeof(short[]),
                    typeof(short?),
                    typeof(short?[]),
                    typeof(int),
                    typeof(int[]),
                    typeof(int?),
                    typeof(int?[]),
                    typeof(long),
                    typeof(long[]),
                    typeof(long?),
                    typeof(long?[]),
                    typeof(float),
                    typeof(float[]),
                    typeof(float?),
                    typeof(float?[]),
                    typeof(double),
                    typeof(double[]),
                    typeof(double?),
                    typeof(double?[]),
                    typeof(decimal),
                    typeof(decimal[]),
                    typeof(decimal?),
                    typeof(decimal?[]),
                    typeof(DateTime),
                    typeof(DateTime[]),
                    typeof(DateTime?),
                    typeof(DateTime?[]),
                    typeof(DateTimeOffset),
                    typeof(DateTimeOffset[]),
                    typeof(DateTimeOffset?),
                    typeof(DateTimeOffset?[]),
                    typeof(TimeSpan),
                    typeof(TimeSpan[]),
                    typeof(TimeSpan?),
                    typeof(TimeSpan?[]),
                    typeof(char),
                    typeof(char?),
                    typeof(char[]),
                    typeof(char?[])
                }
            );

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

        public static PropertyInfo GetProperty<T>(string name, NamingStrategy strategy = null)
        {
            return GetProperty(typeof(T), name, strategy);
        }


        private static Dictionary<string, PropertyInfo> Build(Type type, string prefixPropertyName,
            ICollection<Type> knowtypes, NamingStrategy namingStrategy = null)
        {
            if (knowtypes.Contains(type)) return new Dictionary<string, PropertyInfo>();
            knowtypes.Add(type);
            var result = new Dictionary<string, PropertyInfo>();
            foreach (var property in type.GetProperties(BindingFlags.Instance|BindingFlags.Public))
            {
                var jsonExclude = property.GetCustomAttribute<JsonIgnoreAttribute>();
                if (jsonExclude != null) continue;
                var jsonPropertyName = GetJsonPropertyName(property, prefixPropertyName, namingStrategy);
                if (PrimitiveTypes.Contains(property.PropertyType))
                {
                    result.Add(jsonPropertyName, property);
                }
                // TODO voir comment traité expression sub entity
                /*
                else
                {
                    var subBuild = Build(property.PropertyType, (prefixPropertyName!=    string.Empty?".":"") + jsonPropertyName, knowtypes,
                        namingStrategy);

                    foreach (var subJsonPropertyName in subBuild.Keys)
                    {
                        result.Add(subJsonPropertyName, subBuild[subJsonPropertyName]);
                    }
                }*/
            }
            return result;
        }


        private static string GetJsonPropertyName(MemberInfo propertyInfo, string prefixPropertyName,
            NamingStrategy namingStrategy = null)
        {
            var propertyName = propertyInfo.Name;
            var attribute = propertyInfo.GetCustomAttribute<JsonPropertyAttribute>();
            if (attribute != null)
            {
                propertyName = attribute.PropertyName;
            }
            if (namingStrategy == null) return string.Format(prefixPropertyName, propertyName);
            switch (namingStrategy)
            {
                case SnakeCaseNamingStrategy _:
                    propertyName = propertyName.Split(new[] {"_"}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(s => char.ToUpperInvariant(s[0]) + s.Substring(1, s.Length - 1))
                        .Aggregate(string.Empty, (s1, s2) => s1 + s2).ToLowerInvariant();
                    break;
                case CamelCaseNamingStrategy _:
                    propertyName = propertyName[0].ToString().ToUpperInvariant() + propertyName.Substring(1);
                    break;
            }
            return prefixPropertyName + (prefixPropertyName != string.Empty ? "." : "") + propertyName;
        }


        public static PropertyInfo GetProperty(Type type, string name, NamingStrategy namingStrategy = null)
        {
            if (MappingJson2PropertyInfo.ContainsKey(type)) return MappingJson2PropertyInfo[type][name];
            lock (MappingJson2PropertyInfo)
            {
                var knownTypes = new List<Type>();
                MappingJson2PropertyInfo[type] = Build(type, string.Empty, knownTypes, namingStrategy);
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
    }
}