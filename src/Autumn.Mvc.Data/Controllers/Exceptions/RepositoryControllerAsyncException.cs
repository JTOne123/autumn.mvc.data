using System;

namespace Autumn.Mvc.Data.Controllers.Exceptions
{
    /// <summary>
    /// exception of repository
    /// </summary>
    public class RepositoryControllerAsyncException : AutumnException
    {
        /// <summary>
        /// class initializer
        /// </summary>
        protected RepositoryControllerAsyncException()
        {
        }

        /// <summary>
        /// class initializer
        /// <param name="message">message exception</param>
        /// </summary>
        protected RepositoryControllerAsyncException(string message) : base(message)
        {
        }

        /// <summary>
        /// class initializer
        /// <param name="message">message exception</param>
        /// <param name="innerException">inner exception</param>
        /// </summary>
        protected RepositoryControllerAsyncException(string message, Exception innerException) : base(message,
            innerException)
        {
        }

        /// <summary>
        /// class initializer
        /// <param name="innerException">message exception</param>
        /// </summary>
        protected RepositoryControllerAsyncException(Exception innerException) : base(innerException)
        {
        }
    }
}