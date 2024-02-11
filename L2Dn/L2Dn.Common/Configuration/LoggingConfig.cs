using NLog;
using NLog.Config;

namespace L2Dn.Configuration;

public class LoggingConfig
{
    public FileLoggerConfig File { get; set; } = new();
    public ConsoleLoggerConfig Console { get; set; } = new();

    public LoggingConfig()
    {
        Console.Enabled = true;
    }
    
    public void ConfigureLogger()
    {
        LoggingConfiguration config = new();
        File.ConfigureLogger(config);
        Console.ConfigureLogger(config);
        LogManager.Configuration = config;
    }
}