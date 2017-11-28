using System;

namespace Autumn.Mvc.Data.Controllers.Exceptions
{
    /// <summary>
    /// exception of Post operation controller ( add entity )
    /// </summary>
    public class PostOperationException: RepositoryControllerAsyncException
    {
        /// <summary>
        /// class initializer
        /// </summary>
        /// <param name="innerException">inner exception</param>
        public PostOperationException(Exception innerException) : base(innerException)
        {
        }
    }
}