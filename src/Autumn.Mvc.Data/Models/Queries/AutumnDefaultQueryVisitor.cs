using System;
using System.Linq.Expressions;
using Antlr4.Runtime.Tree;
using Autumn.Mvc.Data.Models.Queries.Exceptions;
using Newtonsoft.Json.Serialization;

namespace Autumn.Mvc.Data.Models.Queries
{
    public class AutumnDefaultQueryVisitor<T> : AutumnQueryBaseVisitor<Expression<Func<T, bool>>>
    {
        private readonly NamingStrategy  _namingStrategy;
        private readonly ParameterExpression _parameter;
        
        /// <summary>
        /// create instance of object
        /// </summary>
        /// <param name="namingStrategy"></param>
        public AutumnDefaultQueryVisitor(NamingStrategy namingStrategy)
        {
            _namingStrategy = namingStrategy;
            _parameter = Expression.Parameter(typeof(T));
        }
        
        /// <summary>
        /// visit a or expression
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Expression<Func<T, bool>> VisitOr(AutumnQueryParser.OrContext context)
        {
            return AutumnQueryHelper.GetOrExpression(this, context);
        }

        /// <summary>
        /// visit a and expression
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Expression<Func<T, bool>> VisitAnd(AutumnQueryParser.AndContext context)
        {
            return AutumnQueryHelper.GetAndExpression(this, context);
        }

        /// <summary>
        /// visiti a constraint expression
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Expression<Func<T, bool>> VisitConstraint(AutumnQueryParser.ConstraintContext context)
        {
            return context.comparison() != null ? context.comparison().Accept(this) : null;
        }

        public override Expression<Func<T, bool>> VisitErrorNode(IErrorNode node)
        {
            throw new AutumnQueryErrorNodeException(node);
        }

        /// <summary>
        /// visit a comparison expression 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Expression<Func<T, bool>> VisitComparison(AutumnQueryParser.ComparisonContext context)
        {
            var comparator = context.comparator().GetText().ToLowerInvariant();
            switch (comparator)
            {
                case "=is-null=":
                case "=nil=":
                    return AutumnQueryHelper.GetIsNullExpression<T>(_parameter, context, _namingStrategy);
                case "==":
                case "=eq=":
                    return AutumnQueryHelper.GetEqExpression<T>(_parameter, context, _namingStrategy);
                case "!=":
                case "=neq=":
                    return AutumnQueryHelper.GetNeqExpression<T>(_parameter, context, _namingStrategy);
                case "<":
                case "=lt=":
                    return AutumnQueryHelper.GetLtExpression<T>(_parameter, context, _namingStrategy);
                case "<=":
                case "=le=":
                    return AutumnQueryHelper.GetLeExpression<T>(_parameter, context, _namingStrategy);
                case ">":
                case "=gt=":
                    return AutumnQueryHelper.GetGtExpression<T>(_parameter, context, _namingStrategy);
                case ">=":
                case "=ge=":
                    return AutumnQueryHelper.GetGeExpression<T>(_parameter, context, _namingStrategy);
                case "=in=":
                    return AutumnQueryHelper.GetInExpression<T>(_parameter, context, _namingStrategy);
                case "=out=":
                case "=nin=":  
                    return AutumnQueryHelper.GetOutExpression<T>(_parameter, context, _namingStrategy);
                default:
                    throw new AutumnQueryComparisonUnknownComparatorException(context);
            }
        }
    }
}