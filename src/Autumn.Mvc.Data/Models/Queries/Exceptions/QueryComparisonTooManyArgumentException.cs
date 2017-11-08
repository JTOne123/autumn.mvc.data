using System;

namespace Autumn.Mvc.Data.Models.Queries.Exceptions
{
    public class QueryComparisonTooManyArgumentException : QueryComparisonException
    {
        public QueryComparisonTooManyArgumentException(QueryParser.ComparisonContext origin,
            Exception innerException = null) : base(origin,string.Format("Too many arguments : {0}",origin.selector().GetText()), innerException)
        {
        }
    }
}