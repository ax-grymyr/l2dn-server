using NLog;
using NLog.Conditions;
using NLog.Config;
using NLog.Targets;

namespace L2Dn;

public static class Logger
{
    private static readonly NLog.Logger _logger = LogManager.GetLogger("Default");

    public static void Configure(bool outputToConsole = true, bool outputToFile = false)
    {
        LoggingConfiguration config = new();

        if (outputToFile)
        {
            FileTarget fileTarget = new("file")
            {
                FileName = $"{DateTime.Now:yyyy''MM''dd'-'hh''mm''ss}.log",
                Layout = @"[${date:format=yyyy-MM-dd HH\:mm\:ss.fff}][${level}] ${message} ${exception}"
            };

            config.AddRule(LogLevel.Debug, LogLevel.Fatal, fileTarget);
        }

        if (outputToConsole)
        {
            ColoredConsoleTarget consoleTarget = new("console")
            {
                DetectConsoleAvailable = true,
                Layout = @"[${date:format=HH\:mm\:ss.fff}][${level}] ${message} ${exception}"
            };

            (string LogLevel, ConsoleOutputColor Color)[] colors = [
                ("Fatal", ConsoleOutputColor.Red),
                ("Error", ConsoleOutputColor.Red),
                ("Warn", ConsoleOutputColor.DarkYellow),
                ("Info", ConsoleOutputColor.White),
                ("Debug", ConsoleOutputColor.DarkGray),
                ("Trace", ConsoleOutputColor.Gray),
            ];

            foreach ((string logLevel, ConsoleOutputColor color) in colors)
            {
                consoleTarget.RowHighlightingRules.Add(new()
                {
                    Condition = ConditionParser.ParseExpression("level == LogLevel." + logLevel),
                    ForegroundColor = color
                });
            }

            config.AddRule(LogLevel.Trace, LogLevel.Fatal, consoleTarget);
        }

        LogManager.Configuration = config;
    }

    public static ILogger NLogger => _logger;

    public static void Log(LogLevel level, string message) => _logger.Log(level, message);
    public static void Log(LogLevel level, Exception exception, string message) => _logger.Log(level, exception, message);

    public static void Trace(string message) => _logger.Trace(message);
    public static void Trace(Exception exception, string message) => _logger.Trace(exception, message);

    public static void Debug(string message) => _logger.Debug(message);
    public static void Debug(Exception exception, string message) => _logger.Debug(exception, message);

    public static void Info(string message) => _logger.Info(message);
    public static void Info(Exception exception, string message) => _logger.Info(exception, message);

    public static void Warn(string message) => _logger.Warn(message);
    public static void Warn(Exception exception, string message) => _logger.Warn(exception, message);

    public static void Error(string message) => _logger.Error(message);
    public static void Error(Exception exception, string message) => _logger.Error(exception, message);

    public static void Fatal(string message) => _logger.Fatal(message);
    public static void Fatal(Exception exception, string message) => _logger.Fatal(exception, message);
}