using L2Dn.AuthServer.Model;
using L2Dn.Network;

namespace L2Dn.AuthServer.NetworkGameServer;

internal sealed class GameServerSession: Session
{
    public GameServerInfo? ServerInfo { get; set; }
}