using System.Collections.Concurrent;
using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.NetworkAuthServer.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Utilities;

namespace L2Dn.GameServer.NetworkAuthServer;

public sealed class AuthServerSession: Session
{
    private readonly ConcurrentDictionary<string, AuthServerLoginData> _logins = new();
    
    public static AuthServerSession Instance { get; } = new();

    public ConcurrentDictionary<string, AuthServerLoginData> Logins => _logins;
    
    public ServerConfig Config => ServerConfig.Instance;

    protected override long GetState() => AuthServerSessionState.Default.ToInt64();
    
    public void setServerStatus(bool online)
    {
        Connection? connection = Connection;
        if (connection != null)
        {
            int playerCount = World.getInstance().getPlayers().Count;
            connection.Send(new UpdateStatusPacket(online, (ushort)playerCount));
        }
    }
}