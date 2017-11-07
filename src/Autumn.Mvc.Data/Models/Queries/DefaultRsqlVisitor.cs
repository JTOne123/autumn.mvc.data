using System;
using System.Linq.Expressions;
using Antlr4.Runtime.Tree;
using Autumn.Mvc.Data.Models.Queries.Exceptions;
using Autumn.Mvc.Models.Helpers;
using Newtonsoft.Json.Serialization;

namespace Autumn.Mvc.Data.Models.Queries
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
            return RsqlHelper.GetOrExpression(this, context);
        }

        /// <summary>
        /// visit a and expression
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Expression<Func<T, bool>> VisitAnd(RsqlParser.AndContext context)
        {
            return RsqlHelper.GetAndExpression(this, context);
        }

        /// <summary>
        /// visiti a constraint expression
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Expression<Func<T, bool>> VisitConstraint(RsqlParser.ConstraintContext context)
        {
            return context.comparison() != null ? context.comparison().Accept(this) : null;
        }

        public override Expression<Func<T, bool>> VisitErrorNode(IErrorNode node)
        {
            throw new RsqlErrorNodeException(node);
        }

        /// <summary>
        /// visit a comparison expression 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Expression<Func<T, bool>> VisitComparison(RsqlParser.ComparisonContext context)
        {
            var comparator = context.comparator().GetText().ToLowerInvariant();
            switch (comparator)
            {
                case "=is-true=":
                    return RsqlHelper.GetIsTrueExpression<T>(_parameter, context, _namingStrategy);
                case "=is-false=":
                    return RsqlHelper.GetIsFalseExpression<T>(_parameter, context, _namingStrategy);
                case "=is-null=":
                    return RsqlHelper.GetIsNullExpression<T>(_parameter, context, _namingStrategy);
                case "=is-not-null=":
                    return RsqlHelper.GetNotIsNullExpression<T>(_parameter, context, _namingStrategy);
                case "==":
                case "=eq=":
                    return RsqlHelper.GetEqExpression<T>(_parameter, context, _namingStrategy);
                case "!=":
                case "=neq=":
                    return RsqlHelper.GetNeqExpression<T>(_parameter, context, _namingStrategy);
                case "<":
                case "=lt=":
                    return RsqlHelper.GetLtExpression<T>(_parameter, context, _namingStrategy);
                case "<=":
                case "=le=":
                    return RsqlHelper.GetLeExpression<T>(_parameter, context, _namingStrategy);
                case ">":
                case "=gt=":
                    return RsqlHelper.GetGtExpression<T>(_parameter, context, _namingStrategy);
                case ">=":
                case "=ge=":
                    return RsqlHelper.GetGeExpression<T>(_parameter, context, _namingStrategy);
                case "=lk=":
                    return RsqlHelper.GetLkExpression<T>(_parameter, context, _namingStrategy);
                case "=in=":
                    return RsqlHelper.GetInExpression<T>(_parameter, context, _namingStrategy);
                case "=out=":
                    return RsqlHelper.GetOutExpression<T>(_parameter, context, _namingStrategy);
                default:
                    throw new RsqlComparisonUnknownComparatorException(context);
            }
        }
    }
}