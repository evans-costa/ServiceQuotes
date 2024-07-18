using Microsoft.Extensions.Logging;
using ServiceQuotes.CrossCutting.Logging;

namespace ServiceQuotes.CrossCutting.IoC;
public static class LoggingExtensions
{
    public static ILoggingBuilder AddCustomLogger(this ILoggingBuilder loggingBuilder)
    {
        loggingBuilder.ClearProviders();
        loggingBuilder.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
        {
            LogLevel = LogLevel.Information,
        }));

        return loggingBuilder;
    }
}
