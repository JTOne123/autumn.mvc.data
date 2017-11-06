using System;

namespace Autumn.Mvc.Data.Models.Queries.Exceptions
{
    public class RsqlComparisonTooManyArgumentException : RsqlComparisonException
    {
        public RsqlComparisonTooManyArgumentException(RsqlParser.ComparisonContext origin,
            Exception innerException = null) : base(origin,"Too many arguments", innerException)
        {
        }
    }
}