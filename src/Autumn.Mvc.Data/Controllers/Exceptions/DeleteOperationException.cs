using System;

namespace Autumn.Mvc.Data.Controllers.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    /// exception of DELETE operation controller ( delete entity )
    /// </summary>
    public class DeleteOperationException: RepositoryControllerAsyncException
    {
        /// <inheritdoc />
        /// <summary>
        /// class initializer
        /// </summary>
        /// <param name="innerException">inner exception</param>
        public DeleteOperationException(Exception innerException) : base(innerException)
        {
        }
    }
}