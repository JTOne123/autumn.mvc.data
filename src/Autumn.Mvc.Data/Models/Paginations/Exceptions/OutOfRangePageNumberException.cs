using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Autumn.Mvc.Data.Models.Paginations.Exceptions
{
    public class OutOfRangePageNumberException : PageableException
    {
        public OutOfRangePageNumberException(ModelBindingContext origin,int pageNumber,
            Exception innerException = null) : base(origin,
            string.Format("Out of range page number  : {0}", pageNumber), innerException)
        {
        }
    }
}
