using System;

namespace Autumn.Data.Rest.Queries.Exceptions
{
    public class RsqlComparisonUnknownSelectorException : RsqlException<RsqlParser.ComparisonContext>
    {
        public RsqlComparisonUnknownSelectorException(RsqlParser.ComparisonContext origin,
            Exception innerException = null) : base(origin,
            "Unknown selector", innerException)
        {
        }
    }
}