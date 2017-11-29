using System;

namespace Autumn.Mvc.Data.Models.Queries.Exceptions
{
    public class AutumnQueryComparisonUnknownComparatorException : AutumnQueryComparisonException
    {
        public AutumnQueryComparisonUnknownComparatorException(AutumnQueryParser.ComparisonContext origin,
            Exception innerException = null) : base(origin,
            string.Format("Unknown comparator : {0}",origin.comparator().GetText()), innerException)
        {
        }
    }
}