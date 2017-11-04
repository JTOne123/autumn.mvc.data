using System;

namespace Autumn.Data.Rest.Queries.Exceptions
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