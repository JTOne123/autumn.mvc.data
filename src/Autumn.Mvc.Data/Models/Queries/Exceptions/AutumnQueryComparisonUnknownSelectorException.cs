using System;

namespace Autumn.Mvc.Data.Models.Queries.Exceptions
{
    public class AutumnQueryComparisonUnknownSelectorException : AutumnQueryComparisonException
    {
        public AutumnQueryComparisonUnknownSelectorException(AutumnQueryParser.ComparisonContext origin,
            Exception innerException = null) : base(origin,
            string.Format("Unknown selector : '{0}'",origin.selector().GetText()) , innerException)
        {
        }
    }
}