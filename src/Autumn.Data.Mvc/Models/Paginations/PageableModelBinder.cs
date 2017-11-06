using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autumn.Data.Mvc.Models.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Serialization;

namespace Autumn.Data.Mvc.Models.Paginations
{
    public class PageableModelBinder<T> : IModelBinder where T : class 
    {
        private readonly string _pageSizeField;
        private readonly string _pageNumberField;
        private readonly string _sortField;
        private readonly NamingStrategy _namingStrategy;

        /// <summary>
        /// class initializer
        /// </summary>
        /// <param name="pageSizeField"></param>
        /// <param name="pageNumberField"></param>
        /// <param name="sortField"></param>µ
        /// <param name="namingStrategy"></param>
        public PageableModelBinder(string pageSizeField, string pageNumberField, string sortField,
            NamingStrategy namingStrategy)
        {
            _pageSizeField = pageSizeField;
            _pageNumberField = pageNumberField;
            _sortField = sortField;
            _namingStrategy = namingStrategy;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var queryCollection = bindingContext.ActionContext.HttpContext.Request.Query;
            var pageSize = 100;
            if (queryCollection.TryGetValue(_pageSizeField, out var pageSizeString))
            {
                int.TryParse(pageSizeString[0], out pageSize);
            }
            var pageNumber = 0;
            if (queryCollection.TryGetValue(_pageNumberField, out var pageNumberString))
            {
                int.TryParse(pageNumberString[0], out pageNumber);
            }

            Sort<T> sort = null;
            if (queryCollection.TryGetValue(_sortField, out var sortStringValues))
            {
                var parameter = Expression.Parameter(typeof(T));

                var orderBy = new List<Expression<Func<T, object>>>();
                var orderDescendingBy = new List<Expression<Func<T, object>>>();
     
                foreach (var sortStringValue in sortStringValues)
                {
                    var expressionValue =
                        CommonHelper.GetMemberExpressionValue<T>(parameter, sortStringValue, _namingStrategy);
                    var expression = Expression.Convert(expressionValue.Expression, typeof(object));
                    var orderExpression = Expression.Lambda<Func<T, object>>(expression, parameter);
                    var propertyKeyDirection = sortStringValue + ".dir";
                    var isDescending = false;
                    if (queryCollection.ContainsKey(propertyKeyDirection))
                    {
                        var sortDirection = queryCollection[propertyKeyDirection][0].ToLowerInvariant();
                        isDescending = sortDirection == "desc";
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