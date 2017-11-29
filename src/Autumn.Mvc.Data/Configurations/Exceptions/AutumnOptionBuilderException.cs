using System;

namespace Autumn.Mvc.Data.Configurations.Exceptions
{
    public class AutumnOptionBuilderException : AutumnException
    {
        /// <summary>
        /// class initializer
        /// </summary>
        protected AutumnOptionBuilderException()
        {
        }

        /// <summary>
        /// class initializer
        /// <param name="message">message exception</param>
        /// </summary>
        protected AutumnOptionBuilderException(string message) : base(message)
        {
        }

        /// <summary>
        /// class initializer
        /// <param name="message">message exception</param>
        /// <param name="innerException">inner exception</param>
        /// </summary>
        protected AutumnOptionBuilderException(string message, Exception innerException) : base(message,
            innerException)
        {
        }

        /// <summary>
        /// class initializer
        /// <param name="innerException">message exception</param>
        /// </summary>
        protected AutumnOptionBuilderException(Exception innerException) : base(innerException)
        {
        }
    }
}