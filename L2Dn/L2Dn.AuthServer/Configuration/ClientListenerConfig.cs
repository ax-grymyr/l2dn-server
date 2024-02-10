namespace L2Dn.AuthServer.Configuration;

public sealed class ClientListenerConfig
{
    public string ListenAddress { get; set; } = "0.0.0.0";
    public int Port { get; set; } = 2106;
}