using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Autumn.Data.Mvc
{
    public static class ApplicationLogging
    {
        public static ILoggerFactory LoggerFactory { get; } 
        public static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
        public static ILogger CreateLogger(string name) => LoggerFactory.CreateLogger(name);

        static ApplicationLogging()
        {
            LoggerFactory=new LoggerFactory();
            LoggerFactory.AddProvider(new ConsoleLoggerProvider((text, logLevel) => logLevel >= LogLevel.Information , true));
        }
    }
}