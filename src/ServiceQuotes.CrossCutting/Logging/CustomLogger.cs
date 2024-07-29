using Microsoft.Extensions.Logging;

namespace ServiceQuotes.CrossCutting.Logging;

public class CustomLogger : ILogger
{
    readonly string loggerName;
    readonly CustomLoggerProviderConfiguration loggerConfig;

    public CustomLogger(string name, CustomLoggerProviderConfiguration config)
    {
        loggerName = name;
        loggerConfig = config;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel == loggerConfig.LogLevel;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        string message = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} [{logLevel.ToString()}]: {eventId.Id} - {formatter(state, exception)}";

        GenerateLogFile(message);
    }

    private void GenerateLogFile(string message)
    {
        var logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        var filePath = Path.Combine(logDirectory, $"{DateTime.Now.ToString("yyyy-MM-dd")}.log");
        try
        {
            Directory.CreateDirectory(logDirectory);

            using StreamWriter sw = new StreamWriter(filePath, true);
            sw.WriteLine(message);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
