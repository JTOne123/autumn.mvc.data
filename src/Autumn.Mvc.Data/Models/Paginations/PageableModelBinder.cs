using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autumn.Mvc.Data.Configurations;
using Autumn.Mvc.Data.Models.Helpers;
using Autumn.Mvc.Data.Models.Paginations.Exceptions;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Autumn.Mvc.Data.Models.Paginations
{
    public class PageableModelBinder<T> : IModelBinder where T : class
    {
        private readonly AutumnSettings _autumnSettings;

        public PageableModelBinder(AutumnSettings autumnSettings)
        {
            _autumnSettings = autumnSettings;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var queryCollection = bindingContext.ActionContext.HttpContext.Request.Query;
            var pageSize = _autumnSettings.DefaultPageSize;
            if (queryCollection.TryGetValue(_autumnSettings.PageSizeFieldName, out var pageSizeString))
            {
                if (int.TryParse(pageSizeString[0], out pageSize))
                {
                    if (pageSize < 0)
                    {
                        throw new OutOfRangePageSizeException(bindingContext,pageSize);
                    }
                }
                else
                {
                    throw new InvalidPageSizeValueException(bindingContext, pageSizeString[0]);
                }
            }
            var pageNumber = 0;
            if (queryCollection.TryGetValue(_autumnSettings.PageNumberFieldName, out var pageNumberString))
            {
                if (int.TryParse(pageNumberString[0], out pageNumber))
                {
                    if (pageNumber < 0)
                    {
                        throw new OutOfRangePageNumberException(bindingContext, pageNumber);
                    }
                }
                else
                {
                    throw new InvalidPageNumberValueException(bindingContext, pageNumberString[0]);
                }
            }

            Sort<T> sort = null;
            if (queryCollection.TryGetValue(_autumnSettings.SortFieldName, out var sortStringValues))
            {
                var parameter = Expression.Parameter(typeof(T));

                var orderBy = new List<Expression<Func<T, object>>>();
                var orderDescendingBy = new List<Expression<Func<T, object>>>();
     
                foreach (var sortStringValue in sortStringValues)
                {
                    CommonHelper.ExpressionValue expressionValue;
                    try
                    {
                        expressionValue =
                            CommonHelper.GetMemberExpressionValue<T>(parameter, sortStringValue, _autumnSettings.NamingStrategy);
                    }
                    catch (Exception e)
                    {
                        throw new UnknownSortException(bindingContext, sortStringValue, e);
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
                            throw new InvalidSortDirectionException(bindingContext, sortDirection);
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
                sort = new Sort<T>(orderBy, orderDescendingBy);
            }

            bindingContext.Result = ModelBindingResult.Success(new Pageable<T>(pageNumber, pageSize, sort));
            return Task.CompletedTask;
        }
    }
}