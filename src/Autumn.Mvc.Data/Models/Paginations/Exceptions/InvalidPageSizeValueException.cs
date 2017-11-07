using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Autumn.Mvc.Data.Models.Paginations.Exceptions
{
    public class InvalidPageSizeValueException : PageableException
    {
        public InvalidPageSizeValueException(ModelBindingContext origin,object pageSize,
            Exception innerException = null) : base(origin,
            string.Format("Invalid page size value : {0}", pageSize), innerException)
        {
        }
    }
}