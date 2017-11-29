using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Autumn.Mvc.Data.Models.Paginations.Exceptions
{
    public class AutumnInvalidPageNumberValueException : AutumnPageableException
    {
        public AutumnInvalidPageNumberValueException(ModelBindingContext origin,object pageNumber,
            Exception innerException = null) : base(origin,
            string.Format("Invalid page number value : {0}", pageNumber), innerException)
        {
        }
    }
}