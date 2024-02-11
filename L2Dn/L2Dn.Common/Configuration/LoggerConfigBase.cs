using NLog;
using NLog.Config;

namespace L2Dn.Configuration;

public abstract class LoggerConfigBase
{
    public bool Enabled { get; set; }
    public LogLevel LogLevel { get; set; } = LogLevel.Error;

    public abstract void ConfigureLogger(LoggingConfiguration config);
}