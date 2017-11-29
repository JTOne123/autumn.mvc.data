using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Autumn.Mvc.Data.Models.Paginations.Exceptions
{
    public class AutumnInvalidSortDirectionException : AutumnPageableException
    {
        public AutumnInvalidSortDirectionException(ModelBindingContext origin,object sortDirection,
            Exception innerException = null) : base(origin,
            string.Format("Invalid sort direction : {0}", sortDirection), innerException)
        {
        }
    }
}