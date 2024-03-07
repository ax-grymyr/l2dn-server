using L2Dn.AuthServer.Model;
using L2Dn.Network;
using L2Dn.Utilities;

namespace L2Dn.AuthServer.NetworkGameServer;

internal sealed class GameServerSession: Session
{
    public GameServerInfo? ServerInfo { get; set; }

    protected override long GetState() => GameServerSessionState.Default.ToInt64();
}