using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

internal struct RequestManorListPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection<GameSession> connection)
    {
        ManorListPacket manorListPacket = new();
        connection.Send(ref manorListPacket);

        return ValueTask.CompletedTask;
    }
}
