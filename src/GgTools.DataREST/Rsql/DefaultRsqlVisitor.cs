using System;
using System.Linq.Expressions;
using GgTools.DataREST.Helpers;
using Newtonsoft.Json.Serialization;

namespace GgTools.DataREST.Rsql
{
    public class DefaultRsqlVisitor<T> : RsqlBaseVisitor<Expression<Func<T, bool>>>
    {
        private readonly NamingStrategy  _namingStrategy;
        
        public DefaultRsqlVisitor(NamingStrategy namingStrategy)
        {
            _namingStrategy = namingStrategy;
        }
     
        /// <summary>
        /// visit a comparison
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Expression<Func<T, bool>> VisitComparison(RsqlParser.ComparisonContext context)
        {
            var selector = context.selector().GetText();
            var comparator = context.comparator().GetText().ToLowerInvariant();
            var parameter = Expression.Parameter(typeof(T));
            var entityProperty = RsqlHelper.GetProperty(typeof(T), selector, _namingStrategy);
            var argument = RsqlHelper.GetArgumentValue(comparator, context.arguments());
            Expression expression;
            switch (comparator)
            {
                case "=is-true=":
                    expression = RsqlHelper.GetIsTrueExpression(parameter, entityProperty);
                    break;
                case "=is-false=":
                    expression = RsqlHelper.GetIsFalseExpression(parameter, entityProperty);
                    break;
                case "=is-null=":
                    expression = RsqlHelper.GetIsNullExpression(parameter, entityProperty);
                    break;
                case "=is-not-null=":
                    expression = RsqlHelper.GetNotIsNullExpression(parameter, entityProperty);
                    break;
                case "==":
                case "=eq=":
                    expression = RsqlHelper.GetEqExpression(parameter, entityProperty, argument);
                    break;
                case "!=":
                case "=neq=":
                    expression = RsqlHelper.GetNeqExpression(parameter, entityProperty, argument);
                    break;
                case "<":
                case "=lt=":
                    expression = RsqlHelper.GetLtExpression(parameter, entityProperty, argument);
                    break;
                case "<=":
                case "=le=":
                    expression = RsqlHelper.GetLeExpression(parameter, entityProperty, argument);
                    break;
                case ">":
                case "=gt=":
                    expression = RsqlHelper.GetGtExpression(parameter, entityProperty, argument);
                    break;
                case ">=":
                case "=ge=":
                    expression = RsqlHelper.GetGeExpression(parameter, entityProperty, argument);
                    break;
                case "=lk=":
                    expression = RsqlHelper.GetLkExpression(parameter, entityProperty, (string) argument);
                    break;
                default:
                    expression = Expression.Equal(Expression.Constant(true), Expression.Constant(true));
                    break;
            }
            return Expression.Lambda<Func<T, bool>>(expression, parameter);
        }
    }
}