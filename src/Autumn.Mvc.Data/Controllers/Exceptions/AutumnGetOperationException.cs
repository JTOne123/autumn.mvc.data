using System;

namespace Autumn.Mvc.Data.Controllers.Exceptions
{
    /// <summary>
    /// exception of Get operation controller ( find by criteria )
    /// </summary>
    public class AutumnGetOperationException : AutumnRepositoryControllerAsyncException
    {
        /// <inheritdoc />
        /// <summary>
        /// class initializer
        /// </summary>
        /// <param name="innerException">inner exception</param>
        public AutumnGetOperationException(Exception innerException) : base(innerException)
        {
        }
    }
}