using System;

namespace Autumn.Mvc.Data.Controllers.Exceptions
{
    /// <inheritdoc />
    /// <summary>
    /// exception of DELETE operation controller ( delete entity )
    /// </summary>
    public class AutumnDeleteOperationException: AutumnRepositoryControllerAsyncException
    {
        /// <inheritdoc />
        /// <summary>
        /// class initializer
        /// </summary>
        /// <param name="innerException">inner exception</param>
        public AutumnDeleteOperationException(Exception innerException) : base(innerException)
        {
        }
    }
}