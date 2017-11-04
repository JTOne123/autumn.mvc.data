using System;

namespace Autumn.Data.Rest.Queries.Exceptions
{
    public class RsqlComparisonTooManyArgumentException : RsqlComparisonException
    {
        public RsqlComparisonTooManyArgumentException(RsqlParser.ComparisonContext origin,
            Exception innerException = null) : base(origin,"Too many arguments", innerException)
        {
        }
    }
}