using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

internal readonly struct RequestBRVersionPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection<GameSession> connection)
    {
        // if enabled world exchange
        ExBRVersionPacket exBRVersionPacket = new();
        connection.Send(ref exBRVersionPacket);

        return ValueTask.CompletedTask;
    }
}
