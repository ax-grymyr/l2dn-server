using L2Dn.AuthServer.Network.GameServer.IncomingPackets;
using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.GameServer;

internal sealed class GameServerPacketHandler: PacketHandler<GameServerSession, GameServerSessionState>
{
    public GameServerPacketHandler()
    {
        RegisterPacket<RegisterGameServerPacket>(IncomingPacketCodes.RegisterGameServer);
        RegisterPacket<PingRequestPacket>(IncomingPacketCodes.PingRequest);
    }
}