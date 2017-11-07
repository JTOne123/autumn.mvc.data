using System;

namespace Autumn.Mvc.Data
{
    public abstract class AutumnException : Exception
    {
        protected AutumnException()
        {
        }

        protected AutumnException(string message) : base(message)
        {
        }

        protected AutumnException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}