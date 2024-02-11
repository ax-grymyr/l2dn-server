using L2Dn.Configuration;

namespace L2Dn.GameServer.Configuration;

public sealed class ClientListenerConfig: ListenerConfigBase
{
    public int Protocol { get; set; } = 447;
    public bool Encryption { get; set; } = true;
}