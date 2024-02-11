using L2Dn.AuthServer.Model;
using L2Dn.Network;

namespace L2Dn.AuthServer.NetworkGameServer;

internal sealed class GameServerSession: Session, ISession<GameServerSessionState>
{
    public GameServerSessionState State => GameServerSessionState.None; // State not used
    public GameServerInfo? ServerInfo { get; set; }
 }