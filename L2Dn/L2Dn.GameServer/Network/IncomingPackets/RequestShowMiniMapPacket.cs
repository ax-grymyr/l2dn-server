using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

internal readonly struct RequestShowMiniMapPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection<GameSession> connection)
    {
        ShowMiniMapPacket showMiniMapPacket = new(0, 0);
        connection.Send(ref showMiniMapPacket);

        return ValueTask.CompletedTask;
    }
}
