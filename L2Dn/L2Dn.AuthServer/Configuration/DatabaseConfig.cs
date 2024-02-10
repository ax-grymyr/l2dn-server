namespace L2Dn.AuthServer.Configuration;

public sealed class DatabaseConfig
{
    public string Server { get; set; } = "localhost";
    public string DatabaseName { get; set; } = "l2dev";
    public string UserName { get; set; } = "l2dev_user";
    public string Password { get; set; } = "l2dev_user_pass";
    public bool Trace { get; set; }
}