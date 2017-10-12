using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using WebApplication1.Repositories;

namespace WebApplication1.Commons
{
    public class PageableModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var queryCollection = bindingContext.ActionContext.HttpContext.Request.Query;
            StringValues pageSizeString;
            var pageSize = 100;
            if (queryCollection.TryGetValue("page_size", out pageSizeString))
            {
                int.TryParse(pageSizeString[0], out pageSize);
            }
           
            StringValues pageNumberString;
            var pageNumber = 0;
            if (queryCollection.TryGetValue("page_number", out pageNumberString))
            {
                int.TryParse(pageNumberString[0], out pageNumber);
            }
           

            StringValues sortStringValues;
            if (queryCollection.TryGetValue("sort", out sortStringValues))
            {
                var sort = new List<Sort>();
                foreach (var sortStringValue in sortStringValues)
                {
                    var property = sortStringValue;
                    var propertyKeyDirection = property + ".dir";
                    if (queryCollection.ContainsKey(propertyKeyDirection))
                    {
                        object direction;
                        if (Enum.TryParse(typeof(SortDirection), queryCollection[propertyKeyDirection][0].ToUpperInvariant(),
                            out direction))
                        {
                            if ((SortDirection) direction == SortDirection.ASC)
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
            return Task.CompletedTask;
        }
    }
}