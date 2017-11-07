using System;

namespace Autumn.Mvc.Data.Models.Queries.Exceptions
{
    public class RsqlComparisonUnknownSelectorException : RsqlComparisonException
    {
        public RsqlComparisonUnknownSelectorException(RsqlParser.ComparisonContext origin,
            Exception innerException = null) : base(origin,
            string.Format("Unknown selector : '{0}'",origin.selector().GetText()) , innerException)
        {
        }
    }
}