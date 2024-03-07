using L2Dn.AuthServer.Model;
using L2Dn.AuthServer.NetworkGameServer.IncomingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.AuthServer.NetworkGameServer;

internal sealed class GameServerPacketHandler: PacketHandler<GameServerSession>
{
    public GameServerPacketHandler()
    {
        SetDefaultAllowedStates(GameServerSessionState.Default);
        
        RegisterPacket<RegisterGameServerPacket>(IncomingPacketCodes.RegisterGameServer);
        RegisterPacket<PingRequestPacket>(IncomingPacketCodes.PingRequest);
        RegisterPacket<UpdateStatusPacket>(IncomingPacketCodes.UpdateStatus);
    }

    protected override void OnDisconnected(Connection connection, GameServerSession session)
    {
        GameServerInfo? serverInfo = session.ServerInfo;
        if (serverInfo is not null)
        {
            serverInfo.Connection = null;
            serverInfo.IsOnline = false;
        }
    }
}