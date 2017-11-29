using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Autumn.Mvc.Data.Models.Queries.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Autumn.Mvc.Data.Models.Queries
{
    public static class AutumnQueryHelper
    {
        private static readonly Dictionary<Type, Dictionary<string, PropertyInfo>> MappingJson2PropertyInfo =
            new Dictionary<Type, Dictionary<string, PropertyInfo>>();

        private static readonly MethodInfo MethodStringContains =
            typeof(string).GetMethod("Contains", new[] {typeof(string)});

        private static readonly MethodInfo MethodStringStartsWith =
            typeof(string).GetMethod("StartsWith", new[] {typeof(string)});

        private static readonly MethodInfo MethodStringEndsWith =
            typeof(string).GetMethod("EndsWith", new[] {typeof(string)});

        private static readonly Dictionary<Type,AutumnMethodContainsInfo> MethodListContains=new Dictionary<Type, AutumnMethodContainsInfo>();
    
        private static readonly string MaskLk = string.Format("[{0}]", Guid.NewGuid().ToString());
        
        public static Expression<Func<T, bool>> True<T>()
        {
            return f => true;
        }

        public static readonly MemoryCache QueriesCache =
            new MemoryCache(new MemoryCacheOptions() {ExpirationScanFrequency = TimeSpan.FromMinutes(5)});


        #region GetExpression 

        /// <summary>
        /// create and expression ( operator ";" ) 
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="context"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetAndExpression<T>(
            IAutumnQueryVisitor<Expression<Func<T, bool>>> visitor, AutumnQueryParser.AndContext context)
        {
            if (visitor == null) throw new ArgumentException("visitor");
            if (context == null) throw new ArgumentException("context");
            if (context.constraint().Length == 0) return True<T>();
            var right = context.constraint()[0].Accept(visitor);
            if (context.constraint().Length == 1) return right;
            for (var i = 1; i < context.constraint().Length; i++)
            {
                var left = context.constraint()[i].Accept(visitor);
                right = Expression.Lambda<Func<T, bool>>(Expression.And(left.Body, right.Body), left.Parameters);
            }
            return right;
        }

        /// <summary>
        /// create or expression ( operator "," ) 
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="context"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetOrExpression<T>(
            IAutumnQueryVisitor<Expression<Func<T, bool>>> visitor, AutumnQueryParser.OrContext context)
        {
            if (visitor == null) throw new ArgumentException("visitor");
            if (context == null) throw new ArgumentException("context");
            if (context.and().Length == 0) return True<T>();
            var right = context.and()[0].Accept(visitor);
            if (context.and().Length == 1) return right;
            for (var i = 1; i < context.and().Length; i++)
            {
                var left = context.and()[i].Accept(visitor);
                right = Expression.Lambda<Func<T, bool>>(Expression.Or(left.Body, right.Body), left.Parameters);
            }
            return right;
        }

        /// <summary>
        /// create is null expression ( operator "=is-null=" or "=nil=" ) 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        /// <param name="namingStrategy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetIsNullExpression<T>(ParameterExpression parameter,
            AutumnQueryParser.ComparisonContext context,
            NamingStrategy namingStrategy = null)
        {
            if (parameter == null) throw new ArgumentException("parameter");
            if (context == null) throw new ArgumentException("context");
            var expressionValue =
                GetMemberExpressionValue<T>(parameter, context, namingStrategy);
            if (expressionValue.Property.PropertyType.IsValueType && !(expressionValue.Property.PropertyType.IsGenericType &&
                expressionValue.Property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                throw new AutumnQueryComparisonInvalidComparatorSelectionException(context);
            
            var values = GetValues(typeof(bool), context.arguments());
            if (values == null || values.Count == 0) throw new AutumnQueryComparisonNotEnoughtArgumentException(context);
            if (values.Count > 1) throw new AutumnQueryComparisonTooManyArgumentException(context);

            var result = Expression.Lambda<Func<T, bool>>(Expression.Equal(
                expressionValue.Expression,
                Expression.Constant(null, typeof(object))), parameter);
            if ((bool) values[0]) return result;
            var body = Expression.Not(result.Body);
            result = Expression.Lambda<Func<T, bool>>(body, parameter);
            return result;
        }

        /// <summary>
        /// create equal expression ( operator "==" or "=eq=" ) 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        /// <param name="namingStrategy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetEqExpression<T>(ParameterExpression parameter,
            AutumnQueryParser.ComparisonContext context,
            NamingStrategy namingStrategy=null)
        {
            if (parameter == null) throw new ArgumentException("parameter");
            if (context == null) throw new ArgumentException("context");
            
            var expressionValue =
                GetMemberExpressionValue<T>(parameter, context, namingStrategy);
            var values = GetValues(expressionValue.Property.PropertyType, context.arguments());
            if (values == null || values.Count == 0) throw new AutumnQueryComparisonNotEnoughtArgumentException(context);
            if (values.Count > 1) throw new AutumnQueryComparisonTooManyArgumentException(context);

            var value = values[0];
            if (expressionValue.Property.PropertyType != typeof(string))
                return Expression.Lambda<Func<T, bool>>(Expression.Equal(
                    expressionValue.Expression,
                    Expression.Constant(value, expressionValue.Property.PropertyType)), parameter);
            var v = ((string) value).Replace(@"\*", MaskLk);
            if (v.IndexOf('*') != -1)
            {
                return GetLkExpression<T>(parameter, context, namingStrategy);
            }
            value = v.Replace(MaskLk, "*");

            return Expression.Lambda<Func<T, bool>>(Expression.Equal(
                expressionValue.Expression,
                Expression.Constant(value, expressionValue.Property.PropertyType)), parameter);
        }

        /// <summary>
        /// create neq expression ( operator "!=" or "=neq=" ) 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        /// <param name="namingStrategy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetNeqExpression<T>(ParameterExpression parameter,
            AutumnQueryParser.ComparisonContext context,
            NamingStrategy namingStrategy=null)
        {
            var expression = GetEqExpression<T>(parameter, context, namingStrategy);
            var body = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        /// <summary>
#pragma warning disable 1570
        /// create les than expression ( operator "<" or "=lt=" ) 
#pragma warning restore 1570
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        /// <param name="namingStrategy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetLtExpression<T>(ParameterExpression parameter,
            AutumnQueryParser.ComparisonContext context,
            NamingStrategy namingStrategy=null)
        {
            if (parameter == null) throw new ArgumentException("parameter");
            if (context == null) throw new ArgumentException("context");
            
            var expressionValue =
                GetMemberExpressionValue<T>(parameter, context, namingStrategy);
            if (expressionValue.Property.PropertyType == typeof(string) ||
                expressionValue.Property.PropertyType == typeof(bool) ||
                expressionValue.Property.PropertyType == typeof(char) ||
                expressionValue.Property.PropertyType == typeof(char?) ||
                expressionValue.Property.PropertyType == typeof(bool?))
            {
                throw new AutumnQueryComparisonInvalidComparatorSelectionException(context);
            }

            var values = GetValues(expressionValue.Property.PropertyType, context.arguments());
            if (values == null || values.Count == 0) throw new AutumnQueryComparisonNotEnoughtArgumentException(context);
            if (values.Count > 1) throw new AutumnQueryComparisonTooManyArgumentException(context);

            return Expression.Lambda<Func<T, bool>>(Expression.LessThan(
                expressionValue.Expression,
                Expression.Constant(values[0], expressionValue.Property.PropertyType)), parameter);
        }

        /// <summary>
#pragma warning disable 1570
        /// create less than or equal expression ( operator "<=" or "=le=" ) 
#pragma warning restore 1570
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        /// <param name="namingStrategy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetLeExpression<T>(ParameterExpression parameter,
            AutumnQueryParser.ComparisonContext context,
            NamingStrategy namingStrategy=null)
        {
            if (parameter == null) throw new ArgumentException("parameter");
            if (context == null) throw new ArgumentException("context");
            
            var expressionValue =
                GetMemberExpressionValue<T>(parameter, context, namingStrategy);
            if (expressionValue.Property.PropertyType == typeof(string) ||
                expressionValue.Property.PropertyType == typeof(bool) ||
                expressionValue.Property.PropertyType == typeof(char) ||
                expressionValue.Property.PropertyType == typeof(char?)||
                expressionValue.Property.PropertyType == typeof(bool?))
            {
                throw new AutumnQueryComparisonInvalidComparatorSelectionException(context);
            }

            var values = GetValues(expressionValue.Property.PropertyType, context.arguments());
            if (values == null || values.Count == 0) throw new AutumnQueryComparisonNotEnoughtArgumentException(context);
            if (values.Count > 1) throw new AutumnQueryComparisonTooManyArgumentException(context);

            return Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(
                expressionValue.Expression,
                Expression.Constant(values[0], expressionValue.Property.PropertyType)), parameter);
        }

        /// <summary>
        /// create greater than expression ( operator ">" or "=gt=" ) 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        /// <param name="namingStrategy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetGtExpression<T>(ParameterExpression parameter,
            AutumnQueryParser.ComparisonContext context,
            NamingStrategy namingStrategy=null)
        {
            if (parameter == null) throw new ArgumentException("parameter");
            if (context == null) throw new ArgumentException("context");
            
            var expressionValue =
                GetMemberExpressionValue<T>(parameter, context, namingStrategy);
            if (expressionValue.Property.PropertyType == typeof(string) ||
                expressionValue.Property.PropertyType == typeof(bool) ||
                expressionValue.Property.PropertyType == typeof(char) ||
                expressionValue.Property.PropertyType == typeof(char?)||
                expressionValue.Property.PropertyType == typeof(bool?))
            {
                throw new AutumnQueryComparisonInvalidComparatorSelectionException(context);
            }
            var values = GetValues(expressionValue.Property.PropertyType, context.arguments());
            if (values == null || values.Count == 0) throw new AutumnQueryComparisonNotEnoughtArgumentException(context);
            if (values.Count > 1) throw new AutumnQueryComparisonTooManyArgumentException(context);

            return Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(
                expressionValue.Expression,
                Expression.Constant(values[0], expressionValue.Property.PropertyType)), parameter);
        }
        
        /// <summary>
        /// create greater than or equal expression ( operator ">=" or "=ge=" ) 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        /// <param name="namingStrategy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetGeExpression<T>(ParameterExpression parameter,
            AutumnQueryParser.ComparisonContext context,
            NamingStrategy namingStrategy=null)
        {
            if (parameter == null) throw new ArgumentException("parameter");
            if (context == null) throw new ArgumentException("context");
            
            var expressionValue =
                GetMemberExpressionValue<T>(parameter, context, namingStrategy);
            if (expressionValue.Property.PropertyType == typeof(string) ||
                expressionValue.Property.PropertyType == typeof(bool) ||
                expressionValue.Property.PropertyType == typeof(char) ||
                expressionValue.Property.PropertyType == typeof(char?)||
                expressionValue.Property.PropertyType == typeof(bool?))
            {
                throw new AutumnQueryComparisonInvalidComparatorSelectionException(context);
            }
            var values = GetValues(expressionValue.Property.PropertyType, context.arguments());
            if (values == null || values.Count == 0) throw new AutumnQueryComparisonNotEnoughtArgumentException(context);
            if (values.Count > 1) throw new AutumnQueryComparisonTooManyArgumentException(context);

            return Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(
                expressionValue.Expression,
                Expression.Constant(values[0], expressionValue.Property.PropertyType)), parameter);
        }

        /// <summary>
        /// create like expression
        /// </summary>
        /// <returns></returns>
        private static Expression<Func<T, bool>> GetLkExpression<T>(ParameterExpression parameter,
            AutumnQueryParser.ComparisonContext context,
            NamingStrategy namingStrategy=null)
        {
            var expressionValue =
                GetMemberExpressionValue<T>(parameter, context, namingStrategy);
            if (expressionValue.Property.PropertyType != typeof(string))
            {
                throw new AutumnQueryComparisonInvalidComparatorSelectionException(context);
            }
            var values = GetValues(expressionValue.Property.PropertyType, context.arguments());
            if (values == null || values.Count == 0) throw new AutumnQueryComparisonNotEnoughtArgumentException(context);
            if (values.Count > 1) throw new AutumnQueryComparisonTooManyArgumentException(context);

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
            return Expression.Lambda<Func<T, bool>>(Expression.Call(expressionValue.Expression,
                method,
                Expression.Constant(criteria, expressionValue.Property.PropertyType)), parameter);
        }

        /// <summary>
        /// create in expression ( operator "=in=" ) 
        /// </summary>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetInExpression<T>(ParameterExpression parameter,
            AutumnQueryParser.ComparisonContext context,
            NamingStrategy namingStrategy = null)
        {
            var expressionValue =
                GetMemberExpressionValue<T>(parameter, context, namingStrategy);
            var values = GetValues(expressionValue.Property.PropertyType, context.arguments());
            if (values == null || values.Count == 0) throw new AutumnQueryComparisonNotEnoughtArgumentException(context);

            var methodContainsInfo = GetOrRegistryContainsMethodInfo(expressionValue.Property.PropertyType);

            return Expression.Lambda<Func<T, bool>>(
                Expression.Call(Expression.Constant(methodContainsInfo.Convert(values)),
                    methodContainsInfo.ContainsMethod,
                    expressionValue.Expression), parameter);
        }

        /// <summary>
        /// find Contains méthode
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static AutumnMethodContainsInfo GetOrRegistryContainsMethodInfo(Type type)
        {
            lock (MethodListContains)
            {
                if (!MethodListContains.ContainsKey(type))
                {
                    MethodListContains.Add(type, new AutumnMethodContainsInfo(type));
                }
                return MethodListContains[type];
            }
        }

        /// <summary>
        /// create not in expression ( operator "=out=" or "=nin=" ) 
        /// </summary>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetOutExpression<T>(ParameterExpression parameter,
            AutumnQueryParser.ComparisonContext context,
            NamingStrategy namingStrategy=null)
        {
            var expression = GetInExpression<T>(parameter, context, namingStrategy);
            var body = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        #endregion

        #region GetValues

        private static List<object> GetStringValues(AutumnQueryParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {
                if (valueContext.single_quote() != null || valueContext.double_quote() != null)
                {
                    var replace = valueContext.single_quote() != null ? "'" : "\"";
                    var value = valueContext.GetText();
                    if (value.Length == 2)
                    {
                        items.Add(string.Empty);
                    }
                    items.Add(value.Substring(1, value.Length - 2).Replace("\\" + replace, replace));
                }
                else
                {
                    items.Add(valueContext.GetText());
                }
            }
            return items;
        }

        private static List<object> GetShorts(AutumnQueryParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {
                items.Add(short.Parse(valueContext.GetText()));
            }
            return items;
        }

        private static List<object> GetInts(AutumnQueryParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {
                items.Add(int.Parse(valueContext.GetText()));
            }
            return items;
        }

        private static List<object> GetLongs(AutumnQueryParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {
                items.Add(long.Parse(valueContext.GetText()));
            }
            return items;
        }

        private static List<object> GetDoubles(AutumnQueryParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {
                items.Add(double.Parse(valueContext.GetText(), CultureInfo.InvariantCulture));
            }
            return items;
        }

        private static List<object> GetFloats(AutumnQueryParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {
                items.Add(float.Parse(valueContext.GetText(), CultureInfo.InvariantCulture));
            }
            return items;
        }

        private static List<object> GetDecimals(AutumnQueryParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {
                items.Add(decimal.Parse(valueContext.GetText(), CultureInfo.InvariantCulture));
            }
            return items;
        }

        private static List<object> GetDateTimes(AutumnQueryParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {
                items.Add(DateTime.Parse(valueContext.GetText(), CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind));
            }
            return items;
        }
        
        private static List<object> GetDateTimeOffsets(AutumnQueryParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {
                items.Add(DateTimeOffset.Parse(valueContext.GetText(), CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind));
            }
            return items;
        }
        
        
        private static List<object> GetBooleans(AutumnQueryParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {
                items.Add(bool.Parse(valueContext.GetText()));
            }
            return items;
        }

        private static List<object> GetChars(AutumnQueryParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {
                items.Add(char.Parse(valueContext.GetText()));
            }
            return items;
        }
        
        private static List<object> GetBytes(AutumnQueryParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {
                items.Add(byte.Parse(valueContext.GetText()));
            }
            return items;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="argumentsContext"></param>
        /// <returns></returns>
        private static List<object> GetValues(Type type, AutumnQueryParser.ArgumentsContext argumentsContext)
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

            if (type == typeof(float) || type == typeof(float?))
            {
                return GetFloats(argumentsContext);
            }

            if (type == typeof(double) || type == typeof(double?))
            {
                return GetDoubles(argumentsContext);
            }

            if (type == typeof(decimal) || type == typeof(decimal?))
            {
                return GetDecimals(argumentsContext);
            }

            if (type == typeof(bool) || type == typeof(bool?))
            {
                return GetBooleans(argumentsContext);
            }

            if (type == typeof(char) || type == typeof(char?))
            {
                return GetChars(argumentsContext);
            }

            if (type == typeof(byte) || type == typeof(byte?))
            {
                return GetBytes(argumentsContext);
            }
            
            if (type == typeof(DateTimeOffset) || type == typeof(DateTimeOffset?))
            {
                return GetDateTimeOffsets(argumentsContext);
            }
            return null;
        }

        #endregion
        
        #region GetMemberExpressionValue
        
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

        private static ExpressionValue GetMemberExpressionValue<T>(ParameterExpression parameter,
            AutumnQueryParser.ComparisonContext context,
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
                throw new AutumnQueryComparisonInvalidComparatorSelectionException(context, e);
            }
        }
        
        #endregion

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

    }
}