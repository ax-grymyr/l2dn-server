using L2Dn.Network;

namespace L2Dn.AuthServer.NetworkGameServer;

internal sealed class GameServerSessionFactory: ISessionFactory<GameServerSession>
{
    public GameServerSession Create() => new();
}