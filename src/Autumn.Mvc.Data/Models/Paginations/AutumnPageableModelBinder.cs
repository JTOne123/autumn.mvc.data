using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autumn.Mvc.Data.Models.Paginations.Exceptions;
using Autumn.Mvc.Data.Models.Queries;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Autumn.Mvc.Data.Models.Paginations
{
    public class AutumnPageableModelBinder<T> : IModelBinder where T : class
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var queryCollection = bindingContext.ActionContext.HttpContext.Request.Query;
            var pageSize = AutumnApplication.Current.DefaultPageSize;
            if (queryCollection.TryGetValue(AutumnApplication.Current.PageSizeFieldName, out var pageSizeString))
            {
                if (int.TryParse(pageSizeString[0], out pageSize))
                {
                    if (pageSize < 0)
                    {
                        throw new AutumnOutOfRangePageSizeException(bindingContext,pageSize);
                    }
                }
                else
                {
                    throw new AutumnInvalidPageSizeValueException(bindingContext, pageSizeString[0]);
                }
            }
            var pageNumber = 0;
            if (queryCollection.TryGetValue(AutumnApplication.Current.PageNumberFieldName, out var pageNumberString))
            {
                if (int.TryParse(pageNumberString[0], out pageNumber))
                {
                    if (pageNumber < 0)
                    {
                        throw new AutumnOutOfRangePageNumberException(bindingContext, pageNumber);
                    }
                }
                else
                {
                    throw new AutumnInvalidPageNumberValueException(bindingContext, pageNumberString[0]);
                }
            }

            AutumnSort<T> autumnSort = null;
            if (queryCollection.TryGetValue(AutumnApplication.Current.SortFieldName, out var sortStringValues))
            {
                var parameter = Expression.Parameter(typeof(T));

                var orderBy = new List<Expression<Func<T, object>>>();
                var orderDescendingBy = new List<Expression<Func<T, object>>>();
     
                foreach (var sortStringValue in sortStringValues)
                {
                    AutumnQueryHelper.ExpressionValue expressionValue;
                    try
                    {
                        expressionValue =
                            AutumnQueryHelper.GetMemberExpressionValue<T>(parameter, sortStringValue, AutumnApplication.Current.NamingStrategy);
                    }
                    catch (Exception e)
                    {
                        throw new AutumnUnknownSortException(bindingContext, sortStringValue, e);
                    }
                    var expression = Expression.Convert(expressionValue.Expression, typeof(object));
                    var orderExpression = Expression.Lambda<Func<T, object>>(expression, parameter);
                    var propertyKeyDirection = sortStringValue + ".dir";
                    var isDescending = false;
                    if (queryCollection.ContainsKey(propertyKeyDirection))
                    {
                        var sortDirection = queryCollection[propertyKeyDirection][0];
                        if (sortDirection.ToLowerInvariant() != "asc" && sortDirection.ToLowerInvariant() != "desc")
                        {
                            throw new AutumnInvalidSortDirectionException(bindingContext, sortDirection);
                        }
                        isDescending = sortDirection.ToLowerInvariant()  == "desc";
                    }
                    if (isDescending)
                    {
                        orderDescendingBy.Add(orderExpression);
                    }
                    else
                    {
                        orderBy.Add(orderExpression);
                    }
                }
                autumnSort = new AutumnSort<T>(orderBy, orderDescendingBy);
            }

            bindingContext.Result = ModelBindingResult.Success(new AutumnPageable<T>(pageNumber, pageSize, autumnSort));
            return Task.CompletedTask;
        }
    }
}