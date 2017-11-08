using System;

namespace Autumn.Mvc.Data.Models.Queries.Exceptions
{
    public abstract class QueryComparisonException : QueryException<QueryParser.ComparisonContext>
    {
        protected QueryComparisonException(QueryParser.ComparisonContext origin, string message, Exception innerException = null) : base(origin, message, innerException)
        {
        }
    }
}