using System;

namespace Autumn.Mvc.Data.Controllers.Exceptions
{
    /// <summary>
    /// exception of GetByID operation controller ( find by Id )
    /// </summary>
    public class GetByIdOperationException : RepositoryControllerAsyncException
    {
        /// <summary>
        /// class initializer
        /// </summary>
        /// <param name="innerException">inner exception</param>
        public GetByIdOperationException(Exception innerException) : base(innerException)
        {
        }
    }
}