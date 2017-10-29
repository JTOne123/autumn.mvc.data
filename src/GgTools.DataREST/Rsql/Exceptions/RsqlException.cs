﻿using System;

namespace GgTools.DataREST.Rsql.Exceptions
{
    public class RsqlException<T> : Exception
    {
        public T Origin { get; }

        public RsqlException(T origin, string message) : base(message)
        {
            Origin = origin;
        }
        
        public RsqlException(Exception innerException) : base(innerException?.Message, innerException)
        {
            
        }
    }
}