using System;

namespace Autumn.Mvc.Data.Models.Queries.Exceptions
{
    public class QueryComparisonNotEnoughtArgumentException : QueryComparisonException
    {
        public QueryComparisonNotEnoughtArgumentException(QueryParser.ComparisonContext origin,
            Exception innerException = null) : base(origin,
            string.Format("Not enought argument : {0}",origin.selector().GetText())    , innerException)
        {
        }
    }
}