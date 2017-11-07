using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Autumn.Mvc.Data.Models.Paginations.Exceptions
{
    public class OutOfRangePageSizeException : PageableException
    {
        public OutOfRangePageSizeException(ModelBindingContext origin,int pageSize,
            Exception innerException = null) : base(origin,
            string.Format("out of range page size  : {0}", pageSize), innerException)
        {
        }
    }
}