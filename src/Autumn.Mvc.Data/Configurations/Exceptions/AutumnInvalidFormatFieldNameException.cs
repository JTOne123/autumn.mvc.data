namespace Autumn.Mvc.Data.Configurations.Exceptions
{
    public class AutumnInvalidFormatFieldNameException : AutumnOptionBuilderException
    {
        public AutumnInvalidFormatFieldNameException(string fieldName, string value) : base(
            string.Format("Field name {1} already used by {0}", fieldName, value))
        {

        }
    }
}