namespace L2Dn.AuthServer.Configuration;

public sealed class GameServerListenerConfig
{
    public string ListenAddress { get; set; } = "127.0.0.1";
    public int Port { get; set; } = 2107;
    public string AccessKey { get; set; } = string.Empty;
    public bool AcceptNewGameServer { get; set; }
}