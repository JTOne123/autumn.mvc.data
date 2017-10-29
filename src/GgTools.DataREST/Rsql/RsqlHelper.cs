using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GgTools.DataREST.Rsql.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Serialization;

namespace GgTools.DataREST.Rsql
{
    public static class RsqlHelper
    {
        
        private static readonly MethodInfo MethodStringContains=typeof(string).GetMethod("Contains",new[]{typeof(string)});
        private static readonly MethodInfo MethodStringStartsWith=typeof(string).GetMethod("StartsWith",new[]{typeof(string)});
        private static readonly MethodInfo MethodStringEndsWith=typeof(string).GetMethod("EndsWith",new[]{typeof(string)});
        private static readonly MethodInfo MethodListContains = typeof(List<object>).GetMethod("Contains", new[] {typeof(object)});
        private static readonly Dictionary<string,PropertyInfo> MappingJson2PropertyInfo=new Dictionary<string, PropertyInfo>();
        
        public static Expression<Func<T, bool>> True<T> ()  { return f => true;  }
       
        public static readonly MemoryCache QueriesCache = new MemoryCache(new MemoryCacheOptions(){ExpirationScanFrequency = TimeSpan.FromMinutes(5)});

        #region GetExpression 
        
        /// <summary>
        /// create is-null expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="selector"></param>
        /// <param name="namingStrategy"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression GetIsNullExpression<T>(ParameterExpression parameter, string selector,
            NamingStrategy namingStrategy
            , RsqlParser.ArgumentsContext arguments)
        {
            var property = GetProperty(typeof(T), selector, namingStrategy);
            return Expression.Equal(
                Expression.Property(parameter, property),
                Expression.Constant(null, typeof(object)));
        }

        /// <summary>
        /// create not-is-null expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="selector"></param>
        /// <param name="namingStrategy"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public static Expression GetNotIsNullExpression<T>(ParameterExpression parameter, string selector,
            NamingStrategy namingStrategy
            , RsqlParser.ArgumentsContext arguments)
        {
            var property = GetProperty(typeof(T), selector, namingStrategy);
            return Expression.NotEqual(
                Expression.Property(parameter, property),
                Expression.Constant(null, typeof(object)));
        }

        /// <summary>
        /// create eq expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="selector"></param>
        /// <param name="namingStrategy"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="RsqlNotEnoughtArgumentException"></exception>
        /// <exception cref="RsqlTooManyArgumentException"></exception>
        public static Expression GetEqExpression<T>(ParameterExpression parameter, string selector,
            NamingStrategy namingStrategy
            , RsqlParser.ArgumentsContext arguments)
        {
            var property = GetProperty(typeof(T), selector, namingStrategy);
            var values = GetValues(property.PropertyType, arguments);
            if (values == null || values.Count == 0) throw new RsqlNotEnoughtArgumentException(arguments);
            if (values.Count > 1) throw new RsqlTooManyArgumentException(arguments);

            return Expression.Equal(
                Expression.Property(parameter, property),
                Expression.Constant(values[0]));
        }

        /// <summary>
        /// create neq expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="selector"></param>
        /// <param name="namingStrategy"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="RsqlNotEnoughtArgumentException"></exception>
        /// <exception cref="RsqlTooManyArgumentException"></exception>
        public static Expression GetNeqExpression<T>(ParameterExpression parameter, string selector,
            NamingStrategy namingStrategy
            , RsqlParser.ArgumentsContext arguments)
        {
            var property = GetProperty(typeof(T), selector, namingStrategy);
            var values = GetValues(property.PropertyType, arguments);
            if (values == null || values.Count == 0) throw new RsqlNotEnoughtArgumentException(arguments);
            if (values.Count > 1) throw new RsqlTooManyArgumentException(arguments);

            return Expression.NotEqual(
                Expression.Property(parameter, property),
                Expression.Constant(values[0]));
        }

        /// <summary>
        /// create lt expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="selector"></param>
        /// <param name="namingStrategy"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="RsqlNotEnoughtArgumentException"></exception>
        /// <exception cref="RsqlTooManyArgumentException"></exception>
        public static Expression GetLtExpression<T>(ParameterExpression parameter, string selector, NamingStrategy namingStrategy
            ,RsqlParser.ArgumentsContext arguments)
        {
            var property = GetProperty(typeof(T), selector, namingStrategy);
            var values = GetValues(property.PropertyType, arguments);
            if (values == null || values.Count == 0) throw new RsqlNotEnoughtArgumentException(arguments);
            if (values.Count > 1) throw new RsqlTooManyArgumentException(arguments);
            
            return Expression.LessThan(
                Expression.Property(parameter, property),
                Expression.Constant(values[0]));
        }
        
        /// <summary>
        /// create le expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="selector"></param>
        /// <param name="namingStrategy"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="RsqlNotEnoughtArgumentException"></exception>
        /// <exception cref="RsqlTooManyArgumentException"></exception>
        public static Expression GetLeExpression<T>(ParameterExpression parameter, string selector, NamingStrategy namingStrategy
            ,RsqlParser.ArgumentsContext arguments)
        {
            var property = GetProperty(typeof(T), selector, namingStrategy);
            var values = GetValues(property.PropertyType, arguments);
            if (values == null || values.Count == 0) throw new RsqlNotEnoughtArgumentException(arguments);
            if (values.Count > 1) throw new RsqlTooManyArgumentException(arguments);
            
            return Expression.LessThanOrEqual(
                Expression.Property(parameter, property),
                Expression.Constant(values[0]));
        }
        
        /// <summary>
        /// create gt expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="selector"></param>
        /// <param name="namingStrategy"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="RsqlNotEnoughtArgumentException"></exception>
        /// <exception cref="RsqlTooManyArgumentException"></exception>
        public static Expression GetGtExpression<T>(ParameterExpression parameter, string selector, NamingStrategy namingStrategy
            ,RsqlParser.ArgumentsContext arguments)
        {
            var property = GetProperty(typeof(T), selector, namingStrategy);
            var values = GetValues(property.PropertyType, arguments);
            if (values == null || values.Count == 0) throw new RsqlNotEnoughtArgumentException(arguments);
            if (values.Count > 1) throw new RsqlTooManyArgumentException(arguments);
     
            return Expression.GreaterThan(
                Expression.Property(parameter, property),
                Expression.Constant(values[0]));
        }


        /// <summary>
        /// create ge expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="selector"></param>
        /// <param name="namingStrategy"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="RsqlNotEnoughtArgumentException"></exception>
        /// <exception cref="RsqlTooManyArgumentException"></exception>
        public static Expression GetGeExpression<T>(ParameterExpression parameter, string selector,
            NamingStrategy namingStrategy
            , RsqlParser.ArgumentsContext arguments)
        {
            var property = GetProperty(typeof(T), selector, namingStrategy);
            var values = GetValues(property.PropertyType, arguments);
            if (values == null || values.Count == 0) throw new RsqlNotEnoughtArgumentException(arguments);
            if (values.Count > 1) throw new RsqlTooManyArgumentException(arguments);

            return Expression.GreaterThanOrEqual(
                Expression.Property(parameter, property),
                Expression.Constant(values[0]));
        }

        /// <summary>
        /// create is-true expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="selector"></param>
        /// <param name="namingStrategy"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression GetIsTrueExpression<T>(ParameterExpression parameter, string selector, NamingStrategy namingStrategy
            ,RsqlParser.ArgumentsContext arguments)
        {
            var property = GetProperty(typeof(T), selector, namingStrategy);
            return Expression.Equal(
                Expression.Property(parameter, property),
                Expression.Constant(true));
        }

        /// <summary>
        /// create is-false expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="selector"></param>
        /// <param name="namingStrategy"></param>
        /// <param name="arguments"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression GetIsFalseExpression<T>(ParameterExpression parameter, string selector,
            NamingStrategy namingStrategy
            , RsqlParser.ArgumentsContext arguments)
        {
            var property = GetProperty(typeof(T), selector, namingStrategy);
            return Expression.Equal(
                Expression.Property(parameter, property),
                Expression.Constant(false));
        }

        
        /// <summary>
        /// create like expression
        /// </summary>
        /// <returns></returns>
        public static Expression GetLkExpression<T>(ParameterExpression parameter, string selector,
            NamingStrategy namingStrategy
            , RsqlParser.ArgumentsContext arguments)
        {
            var property = GetProperty(typeof(T), selector, namingStrategy);
            var values = GetValues(property.PropertyType, arguments);
            if (values == null || values.Count == 0) throw new RsqlNotEnoughtArgumentException(arguments);
            if (values.Count > 1) throw new RsqlTooManyArgumentException(arguments);

            var criteria = Convert.ToString(values[0]);
            var maskStar = "{" + Guid.NewGuid().ToString() + "}";
            criteria = criteria.Replace(@"\*", maskStar);
            MethodInfo method;
            if (criteria.IndexOf('*') == -1)
            {
                criteria = criteria + '*';
            }
            if (criteria.StartsWith("*") && criteria.EndsWith("*"))
            {
                method = MethodStringContains;
            }
            else if (criteria.StartsWith("*"))
            {
                method = MethodStringEndsWith;
            }
            else
            {
                method = MethodStringStartsWith;
            }
            criteria = criteria.Replace("*", "").Replace(maskStar, "*");
            return Expression.Equal(
                Expression.Call(Expression.Property(parameter, property),
                    method,
                    Expression.Constant(criteria)),
                Expression.Constant(true));
        }

        /// <summary>
        /// create in expression
        /// </summary>
        /// <returns></returns>
        public static Expression GetInExpression<T>(ParameterExpression parameter, string selector,
            NamingStrategy namingStrategy
            , RsqlParser.ArgumentsContext arguments)
        {
            var property = GetProperty(typeof(T), selector, namingStrategy);
            var values = GetValues(property.PropertyType, arguments);
            if (values == null || values.Count == 0) throw new RsqlNotEnoughtArgumentException(arguments);
            return Expression.Call(Expression.Constant(values), MethodListContains,
                Expression.Property(parameter, property));
        }

        /// <summary>
        /// create out expression
        /// </summary>
        /// <returns></returns>
        public static Expression GetOutExpression<T>(ParameterExpression parameter, string selector,
            NamingStrategy namingStrategy
            , RsqlParser.ArgumentsContext arguments)
        {
            return Expression.Not(GetInExpression<T>(parameter, selector, namingStrategy, arguments));
        }

        #endregion
      
        #region GetValues
        
        private static List<object> GetStringValues(RsqlParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {
                if (valueContext.single_quote() == null) continue;
                var text = valueContext.single_quote().GetText();
                text = text.Length == 2 ? string.Empty : text.Substring(1, text.Length - 2);
                items.Add(text);
            }
            return items;
        }
        
        private static List<object> GetShorts(RsqlParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {
                if (valueContext.NUMBER() != null)
                {
                    items.Add(short.Parse(valueContext.NUMBER().GetText()));
                }
            }
            return items;
        }
        
        private static List<object> GetInts(RsqlParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {
                if (valueContext.NUMBER() != null)
                {
                    items.Add(int.Parse(valueContext.NUMBER().GetText()));
                }
            }
            return items;
        }
        
        private static List<object> GetLongs(RsqlParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {
                if (valueContext.NUMBER() != null)
                {
                    items.Add(long.Parse(valueContext.NUMBER().GetText()));
                }
            }
            return items;
        }
        
        private static List<object> GetDoubles(RsqlParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {
                if (valueContext.NUMBER() != null)
                {
                    items.Add(double.Parse(valueContext.NUMBER().GetText()));
                }
            }
            return items;
        }

        private static List<object> GetDecimals(RsqlParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {
                if (valueContext.NUMBER() != null)
                {
                    items.Add(decimal.Parse(valueContext.NUMBER().GetText()));
                }
            }
            return items;
        }
        
        private static List<object> GetDateTimes(RsqlParser.ArgumentsContext argumentsContext)
        {
            // TODO
            return null;
        }

        private static List<object> GetDateTimeOffsets(RsqlParser.ArgumentsContext argumentsContext)
        {
            // TODO
            return null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="argumentsContext"></param>
        /// <returns></returns>
        private static List<object> GetValues(Type type, RsqlParser.ArgumentsContext argumentsContext)
        {
            if (argumentsContext?.value() == null || argumentsContext.value().Length == 0) return null;
            if (type == typeof(string))
            {
                return GetStringValues(argumentsContext);
            }

            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return GetDateTimes(argumentsContext);
            }

            if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?))
            {
                return GetDateTimeOffsets(argumentsContext);
            }
           
            if (type == typeof(short) || type == typeof(short?))
            {
                return GetShorts(argumentsContext);
            }
            
            if (type == typeof(int) || type == typeof(int?))
            {
                return GetInts(argumentsContext);
            }
            
            if (type == typeof(long) || type == typeof(long?))
            {
                return GetLongs(argumentsContext);
            }
            
            if (type == typeof(double) || type == typeof(double?))
            {
                return GetDoubles(argumentsContext);
            }
            
            if (type == typeof(decimal) || type == typeof(decimal?))
            {
                return GetDecimals(argumentsContext);
            }
            return null;
        }

        #endregion

        #region GetProperty 
        
        private static PropertyInfo GetProperty(Type type, string name, NamingStrategy namingStrategy)
        {
            var key = string.Format("{0}|{1}", type.FullName, name);
            if (!MappingJson2PropertyInfo.ContainsKey(key))
            {
                var propertyName = name;
                if (namingStrategy != null)
                {
                    if (namingStrategy is SnakeCaseNamingStrategy)
                    {
                        propertyName = name.Split(new[] {"_"}, StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => char.ToUpperInvariant(s[0]) + s.Substring(1, s.Length - 1))
                            .Aggregate(string.Empty, (s1, s2) => s1 + s2);
                    }
                    if (namingStrategy is CamelCaseNamingStrategy)
                    {
                        propertyName = name[0].ToString().ToUpperInvariant() + name.Substring(1);
                    }
                }
                MappingJson2PropertyInfo[key] = type.GetProperty(propertyName);
            }
            return MappingJson2PropertyInfo[key];
        }
        
        #endregion
    }
}