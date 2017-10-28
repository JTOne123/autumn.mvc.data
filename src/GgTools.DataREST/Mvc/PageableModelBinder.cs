using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GgTools.DataREST.Commons;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GgTools.DataREST.Mvc
{
    public class PageableModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var queryCollection = bindingContext.ActionContext.HttpContext.Request.Query;
            var pageSize = 100;
            if (queryCollection.TryGetValue("page_size", out var pageSizeString))
            {
                int.TryParse(pageSizeString[0], out pageSize);
            }
            var pageNumber = 0;
            if (queryCollection.TryGetValue("page_number", out var pageNumberString))
            {
                int.TryParse(pageNumberString[0], out pageNumber);
            }

            if (queryCollection.TryGetValue("sort", out var sortStringValues))
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
                bindingContext.Result = ModelBindingResult.Success(new Pageable(pageNumber, pageSize, new Sort[]{}));
            }
            return Task.CompletedTask;
        }
    }
}