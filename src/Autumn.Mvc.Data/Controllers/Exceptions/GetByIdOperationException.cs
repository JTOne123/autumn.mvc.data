using System;

namespace Autumn.Mvc.Data.Controllers.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    /// exception of GetByID operation controller ( find by Id )
    /// </summary>
    public class GetByIdOperationException : RepositoryControllerAsyncException
    {
        /// <inheritdoc />
        /// <summary>
        /// class initializer
        /// </summary>
        /// <param name="innerException">inner exception</param>
        public GetByIdOperationException(Exception innerException) : base(innerException)
        {
        }
    }
}