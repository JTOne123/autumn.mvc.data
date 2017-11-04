using System;

namespace Autumn.Data.Rest.Queries.Exceptions
{
    public class RsqlComparisonInvalidComparatorSelectionException: RsqlException<RsqlParser.ComparisonContext>
    {
        public RsqlComparisonInvalidComparatorSelectionException(RsqlParser.ComparisonContext origin,
            Exception innerException = null) : base(origin,
            "Invalid comparator selection", innerException)
        {
        }
    }
}