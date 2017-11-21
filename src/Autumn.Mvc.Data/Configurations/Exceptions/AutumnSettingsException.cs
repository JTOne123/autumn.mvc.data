using System;

namespace Autumn.Mvc.Data.Configurations.Exceptions
{
    public class AutumnSettingsException :AutumnException
    {
        public AutumnSettingsException()
        {
        }

        public AutumnSettingsException(string message) : base(message)
        {
        }

        public AutumnSettingsException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}