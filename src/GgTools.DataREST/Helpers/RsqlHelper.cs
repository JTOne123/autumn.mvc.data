using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Antlr4.Runtime.Tree;
using GgTools.DataREST.Rsql;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GgTools.DataREST.Helpers
{
    public static class RsqlHelper
    {
        private static readonly string[] NoArgument = new string[]
        {
            "=is-null=", "=is-not-null=", "=is-true=", "=is-false="
        };
        
        private static readonly string[] MonoArgument = new string[]
        {
            "=is-null=", "=is-not-null=", "=is-true=", "=is-false=", "==", "!=", "=neq", "=eq=", "<", "=lt=", ">", "=gt=", "<=", "=le=", ">=", "=ge=", "=lk="
        };

        private static readonly string[] MultiArguments = new string[] {"=in=", "=out="};
        
        
        private static object ParseNumber(IParseTree node)
        {
            if (node == null) return null;
            var text = node.GetText();
            return JsonConvert.DeserializeObject<double>(text);
        }

        private static object ParseDateTime(IParseTree node)
        {
            if (node == null) return null;
            var text = node.GetText();
            return JsonConvert.DeserializeObject<DateTime>(text);
        }
        
        private static readonly MethodInfo Contains=typeof(string).GetMethod("Contains",new[]{typeof(string)});
        private static readonly MethodInfo StartsWith=typeof(string).GetMethod("StartsWith",new[]{typeof(string)});
        private static readonly MethodInfo EndsWith=typeof(string).GetMethod("EndsWith",new[]{typeof(string)});

        /// <summary>
        /// create eq expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="entityProperty"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression GetEqExpression(ParameterExpression parameter, PropertyInfo entityProperty,
            object value)
        {
            return Expression.Equal(
                Expression.Property(parameter, entityProperty),
                Expression.Constant(value));
        }
        
        /// <summary>
        /// create is-null expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="entityProperty"></param>
        /// <returns></returns>
        public static Expression GetIsNullExpression(ParameterExpression parameter, PropertyInfo entityProperty)
        {
            return Expression.Equal(
                Expression.Property(parameter, entityProperty),
                Expression.Constant(null, typeof(object)));
        }
        
        /// <summary>
        /// create not-is-null expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="entityProperty"></param>
        /// <returns></returns>
        public static Expression GetNotIsNullExpression(ParameterExpression parameter, PropertyInfo entityProperty)
        {
            return Expression.NotEqual(
                Expression.Property(parameter, entityProperty),
                Expression.Constant(null, typeof(object)));
        }
        
        /// <summary>
        /// create neq expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="entityProperty"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression GetNeqExpression(ParameterExpression parameter, PropertyInfo entityProperty,
            object value)
        {
            return Expression.NotEqual(
                Expression.Property(parameter, entityProperty),
                Expression.Constant(value));
        }
        
        /// <summary>
        /// create lt expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="entityProperty"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression GetLtExpression(ParameterExpression parameter, PropertyInfo entityProperty,
            object value)
        {
            return Expression.LessThan(
                Expression.Property(parameter, entityProperty),
                Expression.Constant(value));
        }
        
        /// <summary>
        /// create le expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="entityProperty"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression GetLeExpression(ParameterExpression parameter, PropertyInfo entityProperty,
            object value)
        {
            return Expression.LessThanOrEqual(
                Expression.Property(parameter, entityProperty),
                Expression.Constant(value));
        }
        
        /// <summary>
        /// create gt expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="entityProperty"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression GetGtExpression(ParameterExpression parameter, PropertyInfo entityProperty,
            object value)
        {
            return Expression.GreaterThan(
                Expression.Property(parameter, entityProperty),
                Expression.Constant(value));
        }
        
        /// <summary>
        /// create ge expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="entityProperty"></param>
        /// <returns></returns>
        public static Expression GetIsTrueExpression(ParameterExpression parameter, PropertyInfo entityProperty)
        {
            return Expression.Equal(
                Expression.Property(parameter, entityProperty),
                Expression.Constant(true));
        }
        
        /// <summary>
        /// create ge expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="entityProperty"></param>
        /// <returns></returns>
        public static Expression GetIsFalseExpression(ParameterExpression parameter, PropertyInfo entityProperty)
        {
            return Expression.Equal(
                Expression.Property(parameter, entityProperty),
                Expression.Constant(false));
        }
        
        /// <summary>
        /// create ge expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="entityProperty"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression GetGeExpression(ParameterExpression parameter, PropertyInfo entityProperty,
            object value)
        {
            return Expression.GreaterThanOrEqual(
                Expression.Property(parameter, entityProperty),
                Expression.Constant(value));
        }
        
        /// <summary>
        /// create like expression
        /// </summary>
        /// <returns></returns>
        public static Expression GetLkExpression(ParameterExpression parameter, PropertyInfo entityProperty,
            string value)
        {
            var criteria = value;
            var maskStar = "{" + Guid.NewGuid().ToString() + "}";
            criteria = criteria.Replace(@"\*", maskStar);
            MethodInfo method;
            if (criteria.IndexOf('*') == -1)
            {
                criteria = criteria + '*';
            }
            if (criteria.StartsWith("*") && criteria.EndsWith("*"))
            {
                method = Contains;
            }
            else if (criteria.StartsWith("*"))
            {
                method = EndsWith;
            }
            else
            {
                method = StartsWith;
            }
            criteria = criteria.Replace("*", "").Replace(maskStar, "*");
            return Expression.Equal(
                Expression.Call(Expression.Property(parameter, entityProperty),
                    method,
                    Expression.Constant(criteria)),
                Expression.Constant(true));
        }

        private static object ParseString(RsqlParser.Single_quoteContext quoteContext)
        {
            if (quoteContext == null)
            {
                return null;
            }
            var text = quoteContext.GetText();
            if (text.Length == 2)
            {
                return string.Empty;
            }
            text = text.Substring(1, text.Length - 2);
            return text;
        }
        
        public static Expression<Func<T, bool>> True<T> ()  { return f => true;  }
        
        private static  readonly  Dictionary<string,PropertyInfo> MappingJson2PropertyInfo=new Dictionary<string, PropertyInfo>();
        
        public static readonly MemoryCache QueriesCache = new MemoryCache(new MemoryCacheOptions(){ExpirationScanFrequency = TimeSpan.FromMinutes(5)});
        
        public static  object GetArgumentValue(string comparator, RsqlParser.ArgumentsContext argumentsContext)
        {
            object result = null;
            var argumentType = GetArgumentType(comparator, argumentsContext);
            switch (argumentType)
            {
                case ArgumentType.No:
                    return null;
                case ArgumentType.Mono:
                    var value = argumentsContext.value()[0];
                    if (value.single_quote() != null)
                    {
                        result = ParseString(value.single_quote());
                    }
                    if (value.DATE() != null)
                    {
                        result = ParseDateTime(value.DATE());
                    }
                    if (value.NUMBER() != null)
                    {
                        result = ParseNumber(value.NUMBER());
                    }
                    break;
                case ArgumentType.Multi:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return result;
        }
        
        private static ArgumentType GetArgumentType(string comparator, RsqlParser.ArgumentsContext argumentsContext)
        {
            if (NoArgument.Contains(comparator))
            {
                return (argumentsContext == null)
                    ? ArgumentType.No
                    : throw new Exception();
            }
            if (MonoArgument.Contains(comparator))
            {
                return (!(argumentsContext?.value() == null || argumentsContext.value().Length != 1))
                    ? ArgumentType.Mono
                    : throw new Exception();
            }
            if (MultiArguments.Contains(comparator))
            {
                return (!(argumentsContext?.value() == null || argumentsContext.value().Length == 0))
                    ? ArgumentType.Mono
                    : throw new Exception();
            }
            throw new Exception();
        }

        public static PropertyInfo GetProperty(Type type, string name, NamingStrategy namingStrategy)
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
    }
}