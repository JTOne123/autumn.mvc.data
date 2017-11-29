using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Autumn.Mvc.Data.Models.Paginations.Exceptions
{
    public class AutumnUnknownSortException : AutumnPageableException
    {
        public AutumnUnknownSortException(ModelBindingContext origin,object sort,
            Exception innerException = null) : base(origin,
            string.Format("Unknown sort : {0}", sort), innerException)
        {
        }
    }
}