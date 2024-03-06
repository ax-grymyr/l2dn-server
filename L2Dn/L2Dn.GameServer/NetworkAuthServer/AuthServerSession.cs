using System.Collections.Concurrent;
using L2Dn.GameServer.Configuration;
using L2Dn.Network;

namespace L2Dn.GameServer.NetworkAuthServer;

internal sealed class AuthServerSession: Session
{
    private readonly ConcurrentDictionary<string, AuthServerLoginData> _logins = new();
    
    public static AuthServerSession Instance { get; } = new();

    public ConcurrentDictionary<string, AuthServerLoginData> Logins => _logins;
    
    public ServerConfig Config => ServerConfig.Instance;
}