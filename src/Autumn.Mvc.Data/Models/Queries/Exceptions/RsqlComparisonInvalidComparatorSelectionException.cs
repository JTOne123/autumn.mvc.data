using System;

namespace Autumn.Mvc.Data.Models.Queries.Exceptions
{
    public class RsqlComparisonInvalidComparatorSelectionException: RsqlComparisonException
    {
        public RsqlComparisonInvalidComparatorSelectionException(RsqlParser.ComparisonContext origin,
            Exception innerException = null) : base(origin,
            "Invalid comparator selector", innerException)
        {
        }
    }
}