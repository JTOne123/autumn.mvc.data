using System;

namespace Autumn.Mvc.Data.Models.Queries.Exceptions
{
    public abstract class RsqlException<T> : AutumnException
    {
        public T Origin { get; }

        protected RsqlException(T origin, string message, Exception innerException = null) : base(message, innerException)
        {
            Origin = origin;
        }
    }

}