using System;

namespace Autumn.Mvc.Data.Models.Queries.Exceptions
{
    public abstract class RsqlComparisonException : RsqlException<RsqlParser.ComparisonContext>
    {
        protected RsqlComparisonException(RsqlParser.ComparisonContext origin, string message, Exception innerException = null) : base(origin, message, innerException)
        {
        }
    }
}