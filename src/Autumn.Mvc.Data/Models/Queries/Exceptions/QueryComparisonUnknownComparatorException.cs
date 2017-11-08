using System;

namespace Autumn.Mvc.Data.Models.Queries.Exceptions
{
    public class QueryComparisonUnknownComparatorException : QueryComparisonException
    {
        public QueryComparisonUnknownComparatorException(QueryParser.ComparisonContext origin,
            Exception innerException = null) : base(origin,
            string.Format("Unknown comparator : {0}",origin.comparator().GetText()), innerException)
        {
        }
    }
}