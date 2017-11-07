using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Autumn.Mvc.Data.Models.Paginations.Exceptions
{
    public class InvalidPageNumberValueException : PageableException
    {
        public InvalidPageNumberValueException(ModelBindingContext origin,object pageNumber,
            Exception innerException = null) : base(origin,
            string.Format("Invalid page number value : {0}", pageNumber), innerException)
        {
        }
    }
}