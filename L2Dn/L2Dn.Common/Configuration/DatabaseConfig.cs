namespace L2Dn.Configuration;

public class DatabaseConfig
{
    public string Server { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int PoolSize { get; set; } = 1024;
    public bool Trace { get; set; }
}