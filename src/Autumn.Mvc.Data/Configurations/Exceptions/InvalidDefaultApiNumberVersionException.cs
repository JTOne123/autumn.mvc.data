namespace Autumn.Mvc.Data.Configurations.Exceptions
{
    public class InvalidDefaultApiNumberVersionException : AutumnSettingsException
    {
        public InvalidDefaultApiNumberVersionException(string version) : base(
            string.Format("Invalid default api number version : {0}", version))
        {

        }
    }
}