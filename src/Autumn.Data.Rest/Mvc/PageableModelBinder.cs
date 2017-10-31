using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Autumn.Data.Rest.Commons;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Serialization;

namespace Autumn.Data.Rest.Mvc
{
    public class PageableModelBinder : IModelBinder
    {
        private readonly string _pageSizeField;
        private readonly string _pageNumberField;
        private readonly string _sortField;
       
        /// <summary>
        /// class initializer
        /// </summary>
        /// <param name="pageSizeField"></param>
        /// <param name="pageNumberField"></param>
        /// <param name="sortField"></param>
        public PageableModelBinder(string pageSizeField, string pageNumberField, string sortField)
        {
            _pageSizeField = pageSizeField;
            _pageNumberField = pageNumberField;
            _sortField = sortField;
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

            if (queryCollection.TryGetValue(_sortField, out var sortStringValues))
            {
                var sort = new List<Sort>();
                foreach (var sortStringValue in sortStringValues)
                {
                    var property = sortStringValue;
                    var propertyKeyDirection = property + ".dir";
                    if (queryCollection.ContainsKey(propertyKeyDirection))
                    {
                        if (Enum.TryParse<SortDirection>(
                            queryCollection[propertyKeyDirection][0].ToUpperInvariant(), true,
                            out var direction))
                        {
                            if (direction == SortDirection.Asc)
                            {
                                sort.Add(Sort.Asc(property));
                            }
                            else
                            {
                                sort.Add(Sort.Desc(property));
                            }
                        }
                    }
                }
                bindingContext.Result = ModelBindingResult.Success(new Pageable(pageNumber, pageSize, sort.ToArray()));
            }
            else
            {
                bindingContext.Result = ModelBindingResult.Success(new Pageable(pageNumber, pageSize, new Sort[] { }));
            }
            return Task.CompletedTask;
        }
    }
}