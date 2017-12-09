using System;

namespace Autumn.Mvc.Data.Models
{
    public class ErrorModelInternalError
    {
        public String Message { get; set; }
        public string StackTrace { get; set; }

        public ErrorModelInternalError()
        {
            
        }


        public ErrorModelInternalError(Exception exception)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            Message = exception.Message;
            StackTrace = exception.StackTrace;
        }
    }
}