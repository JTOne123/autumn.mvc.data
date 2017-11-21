namespace Autumn.Mvc.Data.Configurations.Exceptions
{
    public class InvalidPluralizeControllerException : AutumnSettingsException
    {
        public InvalidPluralizeControllerException(string option) : base(
            string.Format("Invalid pluralize controller option : {0}", option))
        {

        }
    }
}