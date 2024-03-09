using L2Dn.GameServer.Network.OutgoingPackets.Quests;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Teleports;

public struct RequestExTeleportUiPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        connection.Send(new ExTeleportUiPacket());
        return ValueTask.CompletedTask;
    }
}