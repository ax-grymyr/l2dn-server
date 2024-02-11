namespace L2Dn.Configuration;

public class ConfigBase
{
    public DatabaseConfig Database { get; set; } = new();
    public LoggingConfig Logging { get; set; } = new();
}