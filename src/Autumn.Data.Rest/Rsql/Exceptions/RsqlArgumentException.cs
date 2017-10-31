using System;

namespace Autumn.Data.Rest.Rsql.Exceptions
{
    public class RsqlArgumentException : RsqlException<RsqlParser.ArgumentsContext>
    {
        public RsqlArgumentException(RsqlParser.ArgumentsContext origin, string message) : base(origin, message)
        {
        }

        public RsqlArgumentException(Exception innerException) : base(innerException)
        {
        }
    }
}