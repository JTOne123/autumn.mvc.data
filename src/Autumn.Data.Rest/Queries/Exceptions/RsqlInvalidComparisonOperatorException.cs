using System;

namespace Autumn.Data.Rest.Queries.Exceptions
{
    public class RsqlInvalidComparisonOperatorException: RsqlException<RsqlParser.SelectorContext>
    {
        public RsqlInvalidComparisonOperatorException(RsqlParser.SelectorContext origin) : base(origin,"invalid comparison operator for selector")
        {
        }

        public RsqlInvalidComparisonOperatorException(Exception innerException) : base(innerException)
        {
        }
    }
}