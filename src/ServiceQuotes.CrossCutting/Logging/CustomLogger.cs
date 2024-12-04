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
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", $"{DateTime.Now:yyyy-MM-dd}.log");
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        using FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
        using StreamWriter sw = new StreamWriter(fs);
        sw.WriteLine(message);
    }
}
