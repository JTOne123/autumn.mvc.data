using System;
using System.Data;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;

namespace GgTools.DataREST.Rsql
{
    public class DefaultRsqlVisitor<T> : RsqlBaseVisitor<Expression<Func<T, bool>>>
    {
        private readonly NamingStrategy  _namingStrategy;
        private readonly ParameterExpression _parameter;
        
        public DefaultRsqlVisitor(NamingStrategy namingStrategy)
        {
            _namingStrategy = namingStrategy;
            _parameter = Expression.Parameter(typeof(T));
        }

        public override Expression<Func<T, bool>> VisitAnd(RsqlParser.AndContext context)
        {
            if (context.constraint().Length == 0) return RsqlHelper.True<T>();
            var result = context.constraint()[0].Accept(this);
            if (context.constraint().Length == 1) return result;

            Expression subExpression = null;
            for (var i = 1; i < context.constraint().Length; i++)
            {
                if (subExpression == null)
                {
                    subExpression = context.constraint()[i].Accept(this);
                }
                else
                {
                    subExpression = Expression.And(subExpression, context.constraint()[i].Accept(this));
                }
            }
            subExpression = Expression.And(result, subExpression);
            return Expression.Lambda<Func<T, bool>>(subExpression, _parameter);
        }

        public override Expression<Func<T, bool>> VisitConstraint(RsqlParser.ConstraintContext context)
        {
            if (context.comparison() != null)
            {
                return context.comparison().Accept(this);
            }
            return null;
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
            Expression expression;
            switch (comparator)
            {
                case "=is-true=":
                    expression = RsqlHelper.GetIsTrueExpression<T>(_parameter,selector,_namingStrategy,context.arguments());
                    break;
                case "=is-false=":
                    expression = RsqlHelper.GetIsFalseExpression<T>(_parameter, selector,_namingStrategy,context.arguments());
                    break;
                case "=is-null=":
                    expression = RsqlHelper.GetIsNullExpression<T>(_parameter, selector,_namingStrategy,context.arguments());
                    break;
                case "=is-not-null=":
                    expression = RsqlHelper.GetNotIsNullExpression<T>(_parameter, selector,_namingStrategy,context.arguments());
                    break;
                case "==":
                case "=eq=":
                    expression = RsqlHelper.GetEqExpression<T>(_parameter, selector,_namingStrategy,context.arguments());
                    break;
                case "!=":
                case "=neq=":
                    expression = RsqlHelper.GetNeqExpression<T>(_parameter, selector,_namingStrategy,context.arguments());
                    break;
                case "<":
                case "=lt=":
                    expression = RsqlHelper.GetLtExpression<T>(_parameter, selector,_namingStrategy,context.arguments());
                    break;
                case "<=":
                case "=le=":
                    expression = RsqlHelper.GetLeExpression<T>(_parameter, selector,_namingStrategy,context.arguments());
                    break;
                case ">":
                case "=gt=":
                    expression = RsqlHelper.GetGtExpression<T>(_parameter, selector,_namingStrategy,context.arguments());
                    break;
                case ">=":
                case "=ge=":
                    expression = RsqlHelper.GetGeExpression<T>(_parameter, selector,_namingStrategy,context.arguments());
                    break;
                case "=lk=":
                    expression = RsqlHelper.GetLkExpression<T>(_parameter, selector,_namingStrategy,context.arguments());
                    break;
                case "=in=":
                    expression = RsqlHelper.GetInExpression<T>(_parameter, selector,_namingStrategy,context.arguments());
                    break;
                case "=out=":
                    expression = RsqlHelper.GetOutExpression<T>(_parameter, selector,_namingStrategy,context.arguments());
                    break;
                default:
                    expression = Expression.Equal(Expression.Constant(true), Expression.Constant(true));
                    break;
            }
            return Expression.Lambda<Func<T, bool>>(expression, _parameter);
        }
    }
}