using System;

namespace Autumn.Mvc.Data.Models.Queries.Exceptions
{
    public class RsqlComparisonNotEnoughtArgumentException : RsqlComparisonException
    {
        public RsqlComparisonNotEnoughtArgumentException(RsqlParser.ComparisonContext origin,
            Exception innerException = null) : base(origin,
            "Not enougth argument", innerException)
        {
        }
    }
}