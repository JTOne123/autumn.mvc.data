using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Autumn.Mvc.Data.Models.Paginations.Exceptions
{
    public class InvalidSortDirectionException : PageableException
    {
        public InvalidSortDirectionException(ModelBindingContext origin,object sortDirection,
            Exception innerException = null) : base(origin,
            string.Format("Invalid sort direction : {0}", sortDirection), innerException)
        {
        }
    }
}