using System;
using Microsoft.Extensions.Logging;

namespace Shop.Catalog.Api
{
    public class ConsoleLogger<T> : ILogger<T>
    {
        private readonly LogLevel _logLevel;
        public ConsoleLogger(string logLevel)
        {
            if (!Enum.TryParse(logLevel, out _logLevel))
            {
                _logLevel = LogLevel.Information;
            }
            Console.WriteLine($"[ConsoleLogger:Constructor] LogLevel set to : {_logLevel}");
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (IsEnabled(logLevel))
            {
                Console.WriteLine(formatter(state,exception));
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            if (_logLevel == LogLevel.None)
                return false;

            return logLevel >= _logLevel;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }
    }
}
