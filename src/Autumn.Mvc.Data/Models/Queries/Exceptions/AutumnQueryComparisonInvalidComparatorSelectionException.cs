using System;

namespace Autumn.Mvc.Data.Models.Queries.Exceptions
{
    public class AutumnQueryComparisonInvalidComparatorSelectionException: AutumnQueryComparisonException
    {
        public AutumnQueryComparisonInvalidComparatorSelectionException(AutumnQueryParser.ComparisonContext origin,
            Exception innerException = null) : base(origin,
            string.Format("Invalid selector : {0}",origin.selector().GetText()), innerException)
        {
        }
    }
}