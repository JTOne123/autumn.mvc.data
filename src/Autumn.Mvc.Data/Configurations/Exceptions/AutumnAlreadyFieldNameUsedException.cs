namespace Autumn.Mvc.Data.Configurations.Exceptions
{
    public class AutumnAlreadyFieldNameUsedException: AutumnOptionBuilderException
    {
        public AutumnAlreadyFieldNameUsedException(string fieldName, string value) : base(
            string.Format("Field identifier {1} : {0} does not respect the expected format", fieldName, value))
        {

        }
    }
}