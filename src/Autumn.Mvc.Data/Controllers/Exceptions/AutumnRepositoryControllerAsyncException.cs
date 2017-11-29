using System;

namespace Autumn.Mvc.Data.Controllers.Exceptions
{
    /// <summary>
    /// exception of repository
    /// </summary>
    public class AutumnRepositoryControllerAsyncException : AutumnException
    {
        /// <summary>
        /// class initializer
        /// </summary>
        protected AutumnRepositoryControllerAsyncException()
        {
        }

        /// <summary>
        /// class initializer
        /// <param name="message">message exception</param>
        /// </summary>
        protected AutumnRepositoryControllerAsyncException(string message) : base(message)
        {
        }

        /// <summary>
        /// class initializer
        /// <param name="message">message exception</param>
        /// <param name="innerException">inner exception</param>
        /// </summary>
        protected AutumnRepositoryControllerAsyncException(string message, Exception innerException) : base(message,
            innerException)
        {
        }

        /// <summary>
        /// class initializer
        /// <param name="innerException">message exception</param>
        /// </summary>
        protected AutumnRepositoryControllerAsyncException(Exception innerException) : base(innerException)
        {
        }
    }
}