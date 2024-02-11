using NLog;
using NLog.Conditions;
using NLog.Config;
using NLog.Targets;

namespace L2Dn.Utilities;

public class LogUtil
{
    public static void ConfigureConsoleOutput()
    {
        LoggingConfiguration config = new();
        
        ColoredConsoleTarget consoleTarget = new("console")
        {
            DetectConsoleAvailable = true,
            Layout = @"[${date:format=HH\:mm\:ss.fff}][${level}] ${message} ${exception}"
        };

        (string LogLevel, ConsoleOutputColor Color)[] colors =
        [
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
        LogManager.Configuration = config;
    }
}