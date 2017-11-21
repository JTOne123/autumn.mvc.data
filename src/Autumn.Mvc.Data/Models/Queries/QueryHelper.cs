using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Autumn.Mvc.Data.Helpers;
using Autumn.Mvc.Data.Models.Queries.Exceptions;
using Newtonsoft.Json.Serialization;

namespace Autumn.Mvc.Data.Models.Queries
{
    public static class QueryHelper
    {
        private static readonly MethodInfo MethodStringContains =
            typeof(string).GetMethod("Contains", new[] {typeof(string)});

        private static readonly MethodInfo MethodStringStartsWith =
            typeof(string).GetMethod("StartsWith", new[] {typeof(string)});

        private static readonly MethodInfo MethodStringEndsWith =
            typeof(string).GetMethod("EndsWith", new[] {typeof(string)});

        private static readonly MethodInfo MethodListContains =
            typeof(List<object>).GetMethod("Contains", new[] {typeof(object)});

        private static string maskLk = string.Format("[{0}]", Guid.NewGuid().ToString());

        #region GetExpression 

        /// <summary>
        /// create and expression
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="context"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetAndExpression<T>(
            IQueryVisitor<Expression<Func<T, bool>>> visitor, QueryParser.AndContext context)
        {
            if (visitor == null) throw new ArgumentException("visitor");
            if (context == null) throw new ArgumentException("context");
            if (context.constraint().Length == 0) return CommonHelper.True<T>();
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
        /// create or expression
        /// </summary>
        /// <param name="visitor"></param>
        /// <param name="context"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetOrExpression<T>(
            IQueryVisitor<Expression<Func<T, bool>>> visitor, QueryParser.OrContext context)
        {
            if (visitor == null) throw new ArgumentException("visitor");
            if (context == null) throw new ArgumentException("context");
            if (context.and().Length == 0) return CommonHelper.True<T>();
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
        /// create is-null expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        /// <param name="namingStrategy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetIsNullExpression<T>(ParameterExpression parameter,
            QueryParser.ComparisonContext context,
            NamingStrategy namingStrategy = null)
        {
            if (parameter == null) throw new ArgumentException("parameter");
            if (context == null) throw new ArgumentException("context");
            var expressionValue =
                CommonHelper.GetMemberExpressionValue<T>(parameter, context, namingStrategy);
            if (expressionValue.Property.PropertyType.IsValueType || (expressionValue.Property.PropertyType.IsGenericType &&
                expressionValue.Property.PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>)))
                throw new QueryComparisonInvalidComparatorSelectionException(context);
            
            var values = GetValues(typeof(bool), context.arguments());
            if (values == null || values.Count == 0) throw new QueryComparisonNotEnoughtArgumentException(context);
            if (values.Count > 1) throw new QueryComparisonTooManyArgumentException(context);

            var result = Expression.Lambda<Func<T, bool>>(Expression.Equal(
                expressionValue.Expression,
                Expression.Constant(null, typeof(object))), parameter);
            if ((bool) values[0]) return result;
            var body = Expression.Not(result.Body);
            result = Expression.Lambda<Func<T, bool>>(body, parameter);
            return result;
        }

        /// <summary>
        /// create not-is-null expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        /// <param name="namingStrategy"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetNotIsNullExpression<T>(ParameterExpression parameter,
            QueryParser.ComparisonContext context,
            NamingStrategy namingStrategy=null)
        {
            if (parameter == null) throw new ArgumentException("parameter");
            if (context == null) throw new ArgumentException("context");
            var expression = GetIsNullExpression<T>(parameter, context, namingStrategy);
            var body = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        /// <summary>
        /// create eq expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        /// <param name="namingStrategy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetEqExpression<T>(ParameterExpression parameter,
            QueryParser.ComparisonContext context,
            NamingStrategy namingStrategy=null)
        {
            if (parameter == null) throw new ArgumentException("parameter");
            if (context == null) throw new ArgumentException("context");
            
            var expressionValue =
                CommonHelper.GetMemberExpressionValue<T>(parameter, context, namingStrategy);
            var values = GetValues(expressionValue.Property.PropertyType, context.arguments());
            if (values == null || values.Count == 0) throw new QueryComparisonNotEnoughtArgumentException(context);
            if (values.Count > 1) throw new QueryComparisonTooManyArgumentException(context);

            var value = values[0];
            if (expressionValue.Property.PropertyType == typeof(string))
            {
                var v = ((string) value).Replace(@"\*", maskLk);
                if (v.IndexOf('*') != -1)
                {
                    return GetLkExpression<T>(parameter, context, namingStrategy);
                }
                value = v.Replace(maskLk, "*");
            }

            return Expression.Lambda<Func<T, bool>>(Expression.Equal(
                expressionValue.Expression,
                Expression.Constant(value, expressionValue.Property.PropertyType)), parameter);
        }

        /// <summary>
        /// create neq expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        /// <param name="namingStrategy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetNeqExpression<T>(ParameterExpression parameter,
            QueryParser.ComparisonContext context,
            NamingStrategy namingStrategy=null)
        {
            var expression = GetEqExpression<T>(parameter, context, namingStrategy);
            var body = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        /// <summary>
        /// create lt expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        /// <param name="namingStrategy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetLtExpression<T>(ParameterExpression parameter,
            QueryParser.ComparisonContext context,
            NamingStrategy namingStrategy=null)
        {
            if (parameter == null) throw new ArgumentException("parameter");
            if (context == null) throw new ArgumentException("context");
            
            var expressionValue =
                CommonHelper.GetMemberExpressionValue<T>(parameter, context, namingStrategy);
            if (expressionValue.Property.PropertyType == typeof(string) ||
                expressionValue.Property.PropertyType == typeof(bool) ||
                expressionValue.Property.PropertyType == typeof(char) ||
                expressionValue.Property.PropertyType == typeof(char?) ||
                expressionValue.Property.PropertyType == typeof(bool?))
            {
                throw new QueryComparisonInvalidComparatorSelectionException(context);
            }

            var values = GetValues(expressionValue.Property.PropertyType, context.arguments());
            if (values == null || values.Count == 0) throw new QueryComparisonNotEnoughtArgumentException(context);
            if (values.Count > 1) throw new QueryComparisonTooManyArgumentException(context);

            return Expression.Lambda<Func<T, bool>>(Expression.LessThan(
                expressionValue.Expression,
                Expression.Constant(values[0], expressionValue.Property.PropertyType)), parameter);
        }

        /// <summary>
        /// create le expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        /// <param name="namingStrategy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetLeExpression<T>(ParameterExpression parameter,
            QueryParser.ComparisonContext context,
            NamingStrategy namingStrategy=null)
        {
            if (parameter == null) throw new ArgumentException("parameter");
            if (context == null) throw new ArgumentException("context");
            
            var expressionValue =
                CommonHelper.GetMemberExpressionValue<T>(parameter, context, namingStrategy);
            if (expressionValue.Property.PropertyType == typeof(string) ||
                expressionValue.Property.PropertyType == typeof(bool) ||
                expressionValue.Property.PropertyType == typeof(char) ||
                expressionValue.Property.PropertyType == typeof(char?)||
                expressionValue.Property.PropertyType == typeof(bool?))
            {
                throw new QueryComparisonInvalidComparatorSelectionException(context);
            }

            var values = GetValues(expressionValue.Property.PropertyType, context.arguments());
            if (values == null || values.Count == 0) throw new QueryComparisonNotEnoughtArgumentException(context);
            if (values.Count > 1) throw new QueryComparisonTooManyArgumentException(context);

            return Expression.Lambda<Func<T, bool>>(Expression.LessThanOrEqual(
                expressionValue.Expression,
                Expression.Constant(values[0], expressionValue.Property.PropertyType)), parameter);
        }

        /// <summary>
        /// create gt expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        /// <param name="namingStrategy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetGtExpression<T>(ParameterExpression parameter,
            QueryParser.ComparisonContext context,
            NamingStrategy namingStrategy=null)
        {
            if (parameter == null) throw new ArgumentException("parameter");
            if (context == null) throw new ArgumentException("context");
            
            var expressionValue =
                CommonHelper.GetMemberExpressionValue<T>(parameter, context, namingStrategy);
            if (expressionValue.Property.PropertyType == typeof(string) ||
                expressionValue.Property.PropertyType == typeof(bool) ||
                expressionValue.Property.PropertyType == typeof(char) ||
                expressionValue.Property.PropertyType == typeof(char?)||
                expressionValue.Property.PropertyType == typeof(bool?))
            {
                throw new QueryComparisonInvalidComparatorSelectionException(context);
            }
            var values = GetValues(expressionValue.Property.PropertyType, context.arguments());
            if (values == null || values.Count == 0) throw new QueryComparisonNotEnoughtArgumentException(context);
            if (values.Count > 1) throw new QueryComparisonTooManyArgumentException(context);

            return Expression.Lambda<Func<T, bool>>(Expression.GreaterThan(
                expressionValue.Expression,
                Expression.Constant(values[0], expressionValue.Property.PropertyType)), parameter);
        }
        
        /// <summary>
        /// create ge expression
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="context"></param>
        /// <param name="namingStrategy"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetGeExpression<T>(ParameterExpression parameter,
            QueryParser.ComparisonContext context,
            NamingStrategy namingStrategy=null)
        {
            if (parameter == null) throw new ArgumentException("parameter");
            if (context == null) throw new ArgumentException("context");
            
            var expressionValue =
                CommonHelper.GetMemberExpressionValue<T>(parameter, context, namingStrategy);
            if (expressionValue.Property.PropertyType == typeof(string) ||
                expressionValue.Property.PropertyType == typeof(bool) ||
                expressionValue.Property.PropertyType == typeof(char) ||
                expressionValue.Property.PropertyType == typeof(char?)||
                expressionValue.Property.PropertyType == typeof(bool?))
            {
                throw new QueryComparisonInvalidComparatorSelectionException(context);
            }
            var values = GetValues(expressionValue.Property.PropertyType, context.arguments());
            if (values == null || values.Count == 0) throw new QueryComparisonNotEnoughtArgumentException(context);
            if (values.Count > 1) throw new QueryComparisonTooManyArgumentException(context);

            return Expression.Lambda<Func<T, bool>>(Expression.GreaterThanOrEqual(
                expressionValue.Expression,
                Expression.Constant(values[0], expressionValue.Property.PropertyType)), parameter);
        }

        /// <summary>
        /// create like expression
        /// </summary>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetLkExpression<T>(ParameterExpression parameter,
            QueryParser.ComparisonContext context,
            NamingStrategy namingStrategy=null)
        {
            var expressionValue =
                CommonHelper.GetMemberExpressionValue<T>(parameter, context, namingStrategy);
            if (expressionValue.Property.PropertyType != typeof(string))
            {
                throw new QueryComparisonInvalidComparatorSelectionException(context);
            }
            var values = GetValues(expressionValue.Property.PropertyType, context.arguments());
            if (values == null || values.Count == 0) throw new QueryComparisonNotEnoughtArgumentException(context);
            if (values.Count > 1) throw new QueryComparisonTooManyArgumentException(context);

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
        /// create in expression
        /// </summary>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetInExpression<T>(ParameterExpression parameter,    
            QueryParser.ComparisonContext context,
            NamingStrategy namingStrategy = null)
        {
            var expressionValue =
                CommonHelper.GetMemberExpressionValue<T>(parameter, context, namingStrategy);
            var values = GetValues(expressionValue.Property.PropertyType, context.arguments());
            if (values == null || values.Count == 0) throw new QueryComparisonNotEnoughtArgumentException(context);
            return Expression.Lambda<Func<T, bool>>(
                Expression.Call(Expression.Constant(values), MethodListContains,
                    expressionValue.Expression), parameter);
        }

        /// <summary>
        /// create out expression
        /// </summary>
        /// <returns></returns>
        public static Expression<Func<T, bool>> GetOutExpression<T>(ParameterExpression parameter,
            QueryParser.ComparisonContext context,
            NamingStrategy namingStrategy=null)
        {
            var expression = GetInExpression<T>(parameter, context, namingStrategy);
            var body = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        #endregion


        #region GetValues

        private static List<object> GetStringValues(QueryParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {

                if (valueContext.single_quote() != null || valueContext.double_quote() != null)
                {
                    var value = valueContext.GetText();
                    if (value.Length == 2)
                    {
                        items.Add(string.Empty);
                    }
                    items.Add(value.Substring(1, value.Length - 2));

                }
                items.Add(valueContext.GetText());
            }
            return items;
        }

        private static List<object> GetShorts(QueryParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {
                items.Add(short.Parse(valueContext.GetText()));
            }
            return items;
        }

        private static List<object> GetInts(QueryParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {
                items.Add(int.Parse(valueContext.GetText()));
            }
            return items;
        }

        private static List<object> GetLongs(QueryParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {
                items.Add(long.Parse(valueContext.GetText()));
            }
            return items;
        }

        private static List<object> GetDoubles(QueryParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {
                items.Add(double.Parse(valueContext.GetText(), CultureInfo.InvariantCulture));
            }
            return items;
        }

        private static List<object> GetFloats(QueryParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {
                items.Add(float.Parse(valueContext.GetText(), CultureInfo.InvariantCulture));
            }
            return items;
        }

        private static List<object> GetDecimals(QueryParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {
                items.Add(decimal.Parse(valueContext.GetText(), CultureInfo.InvariantCulture));
            }
            return items;
        }

        private static List<object> GetDateTimes(QueryParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {
                items.Add(DateTime.Parse(valueContext.GetText(), CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind));
            }
            return items;
        }
        
        private static List<object> GetDateTimeOffsets(QueryParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {
                items.Add(DateTimeOffset.Parse(valueContext.GetText(), CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind));
            }
            return items;
        }
        
        
        private static List<object> GetBooleans(QueryParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {
                items.Add(bool.Parse(valueContext.GetText()));
            }
            return items;
        }

        private static List<object> GetChars(QueryParser.ArgumentsContext argumentsContext)
        {
            var items = new List<object>();
            foreach (var valueContext in argumentsContext.value())
            {
                items.Add(char.Parse(valueContext.GetText()));
            }
            return items;
        }
        
        private static List<object> GetBytes(QueryParser.ArgumentsContext argumentsContext)
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
        private static List<object> GetValues(Type type, QueryParser.ArgumentsContext argumentsContext)
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
    }
}