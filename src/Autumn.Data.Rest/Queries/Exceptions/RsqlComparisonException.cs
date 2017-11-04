using System;

namespace Autumn.Data.Rest.Queries.Exceptions
{
    public class RsqlComparisonException : RsqlException<RsqlParser.ComparisonContext>
    {
        protected RsqlComparisonException(RsqlParser.ComparisonContext origin, string message, Exception innerException = null) : base(origin, message, innerException)
        {
        }
    }
}