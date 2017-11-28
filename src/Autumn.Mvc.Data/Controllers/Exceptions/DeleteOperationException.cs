using System;

namespace Autumn.Mvc.Data.Controllers.Exceptions
{
    /// <summary>
    /// exception of DELETE operation controller ( delete entity )
    /// </summary>
    public class DeleteOperationException: RepositoryControllerAsyncException
    {
        /// <summary>
        /// class initializer
        /// </summary>
        /// <param name="innerException">inner exception</param>
        public DeleteOperationException(Exception innerException) : base(innerException)
        {
        }
    }
}