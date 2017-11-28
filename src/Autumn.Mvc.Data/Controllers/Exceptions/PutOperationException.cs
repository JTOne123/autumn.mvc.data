using System;

namespace Autumn.Mvc.Data.Controllers.Exceptions
{
    /// <summary>
    /// exception of Put operation controller ( update entity )
    /// </summary>
    public class PutOperationException: RepositoryControllerAsyncException
    {
        /// <summary>
        /// class initializer
        /// </summary>
        /// <param name="innerException">inner exception</param>
        public PutOperationException(Exception innerException) : base(innerException)
        {
        }
    }
}