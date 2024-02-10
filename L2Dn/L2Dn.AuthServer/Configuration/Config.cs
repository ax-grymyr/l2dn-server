namespace L2Dn.AuthServer.Configuration;

public sealed class Config
{
    public ClientListenerConfig ClientListener { get; set; } = new();
}

public sealed class ClientListenerConfig
{
    public string ListenAddress { get; set; } = "0.0.0.0";
    public int Port { get; set; } = 2106;
}


public sealed class GameServerListenerConfig
{
    public string ListenAddress { get; set; } = "127.0.0.1";
    public int Port { get; set; } = 2107;
    public bool AcceptNewGameServer { get; set; }
}