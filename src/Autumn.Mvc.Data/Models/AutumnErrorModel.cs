using System;

namespace Autumn.Mvc.Data.Models
{
    public class AutumnErrorModel
    {
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public string Origin { get; set; }
        public string StackTrace { get; set; }

        public AutumnErrorModel()
        {
            Timestamp=DateTime.UtcNow;
        }
    }
}