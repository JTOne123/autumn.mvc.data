using System;

namespace Autumn.Mvc.Data.Models.Queries.Exceptions
{
    public class AutumnQueryComparisonNotEnoughtArgumentException : AutumnQueryComparisonException
    {
        public AutumnQueryComparisonNotEnoughtArgumentException(AutumnQueryParser.ComparisonContext origin,
            Exception innerException = null) : base(origin,
            string.Format("Not enought argument : {0}",origin.selector().GetText())    , innerException)
        {
        }
    }
}