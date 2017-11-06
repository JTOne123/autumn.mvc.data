using System;

namespace Autumn.Data.Mvc.Models.Queries.Exceptions
{
    public class RsqlComparisonUnknownComparatorException : RsqlComparisonException
    {
        public RsqlComparisonUnknownComparatorException(RsqlParser.ComparisonContext origin,
            Exception innerException = null) : base(origin,
            "Unknown comparator", innerException)
        {
        }
    }
}