using System;

namespace Autumn.Mvc.Data.Controllers.Exceptions
{
    /// <summary>
    /// exception of Put operation controller ( update entity )
    /// </summary>
    public class AutumnPutOperationException: AutumnRepositoryControllerAsyncException
    {
        /// <summary>
        /// class initializer
        /// </summary>
        /// <param name="innerException">inner exception</param>
        public AutumnPutOperationException(Exception innerException) : base(innerException)
        {
        }
    }
}