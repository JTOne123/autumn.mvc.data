using System;

namespace Autumn.Mvc.Data.Models.Queries.Exceptions
{
    public class RsqlException<T> : Exception
    {
        public T Origin { get; }

        protected RsqlException(T origin, string message, Exception innerException = null) : base(message, innerException)
        {
            Origin = origin;
        }
    }


}