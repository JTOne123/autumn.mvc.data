using System;
using System.Linq.Expressions;
using Newtonsoft.Json.Serialization;

namespace Autumn.Data.Rest.Rsql
{
    public class DefaultRsqlVisitor<T> : RsqlBaseVisitor<Expression<Func<T, bool>>>
    {
        private readonly NamingStrategy  _namingStrategy;
        private readonly ParameterExpression _parameter;
        
        /// <summary>
        /// create instance of object
        /// </summary>
        /// <param name="namingStrategy"></param>
        public DefaultRsqlVisitor(NamingStrategy namingStrategy)
        {
            _namingStrategy = namingStrategy;
            _parameter = Expression.Parameter(typeof(T));
        }
        
        /// <summary>
        /// visit a or expression
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Expression<Func<T, bool>> VisitOr(RsqlParser.OrContext context)
        {
            return RsqlHelper.GetOrExpression(_parameter, this, context);
        }

        /// <summary>
        /// visit a and expression
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Expression<Func<T, bool>> VisitAnd(RsqlParser.AndContext context)
        {
            return RsqlHelper.GetAndExpression(_parameter, this, context);
        }

        /// <summary>
        /// visiti a constraint expression
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Expression<Func<T, bool>> VisitConstraint(RsqlParser.ConstraintContext context)
        {
            if (context.comparison() != null)
            {
                return context.comparison().Accept(this);
            }
            return null;
        }

        /// <summary>
        /// visit a comparison expression 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Expression<Func<T, bool>> VisitComparison(RsqlParser.ComparisonContext context)
        {
            var selector = context.selector().GetText();
            var comparator = context.comparator().GetText().ToLowerInvariant();
            switch (comparator)
            {
                case "=is-true=":
                    return RsqlHelper.GetIsTrueExpression<T>(_parameter, selector, _namingStrategy,
                        context.arguments());
                case "=is-false=":
                    return RsqlHelper.GetIsFalseExpression<T>(_parameter, selector, _namingStrategy,
                        context.arguments());
                case "=is-null=":
                    return RsqlHelper.GetIsNullExpression<T>(_parameter, selector, _namingStrategy,
                        context.arguments());
                case "=is-not-null=":
                    return RsqlHelper.GetNotIsNullExpression<T>(_parameter, selector, _namingStrategy,
                        context.arguments());
                case "==":
                case "=eq=":
                    return RsqlHelper.GetEqExpression<T>(_parameter, selector, _namingStrategy, context.arguments());
                case "!=":
                case "=neq=":
                    return RsqlHelper.GetNeqExpression<T>(_parameter, selector, _namingStrategy, context.arguments());
                case "<":
                case "=lt=":
                    return RsqlHelper.GetLtExpression<T>(_parameter, selector, _namingStrategy, context.arguments());
                case "<=":
                case "=le=":
                    return RsqlHelper.GetLeExpression<T>(_parameter, selector, _namingStrategy, context.arguments());
                case ">":
                case "=gt=":
                    return RsqlHelper.GetGtExpression<T>(_parameter, selector, _namingStrategy, context.arguments());
                case ">=":
                case "=ge=":
                    return RsqlHelper.GetGeExpression<T>(_parameter, selector, _namingStrategy, context.arguments());
                case "=lk=":
                    return RsqlHelper.GetLkExpression<T>(_parameter, selector, _namingStrategy, context.arguments());
                case "=in=":
                    return RsqlHelper.GetInExpression<T>(_parameter, selector, _namingStrategy, context.arguments());
                case "=out=":
                    return RsqlHelper.GetOutExpression<T>(_parameter, selector, _namingStrategy, context.arguments());
                default:
                    return Expression.Lambda<Func<T, bool>>(
                        Expression.Equal(Expression.Constant(true), Expression.Constant(true)), _parameter);
            }
        }
    }
}