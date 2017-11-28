using System;

namespace Autumn.Mvc.Data.Controllers.Exceptions
{
    /// <summary>
    /// exception of Get operation controller ( find by criteria )
    /// </summary>
    public class GetOperationException : RepositoryControllerAsyncException
    {
        /// <inheritdoc />
        /// <summary>
        /// class initializer
        /// </summary>
        /// <param name="innerException">inner exception</param>
        public GetOperationException(Exception innerException) : base(innerException)
        {
        }
    }
}