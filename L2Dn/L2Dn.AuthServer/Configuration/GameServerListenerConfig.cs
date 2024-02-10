using L2Dn.Configuration;

namespace L2Dn.AuthServer.Configuration;

public sealed class GameServerListenerConfig: ListenerConfigBase
{
    public string AccessKey { get; set; } = string.Empty;
    public bool AcceptNewGameServer { get; set; }
}