using System;

namespace Autumn.Mvc.Data.Controllers.Exceptions
{
    /// <summary>
    /// exception of Post operation controller ( add entity )
    /// </summary>
    public class AutumnPostOperationException: AutumnRepositoryControllerAsyncException
    {
        /// <summary>
        /// class initializer
        /// </summary>
        /// <param name="innerException">inner exception</param>
        public AutumnPostOperationException(Exception innerException) : base(innerException)
        {
        }
    }
}