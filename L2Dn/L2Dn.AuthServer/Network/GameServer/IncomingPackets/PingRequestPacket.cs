using L2Dn.AuthServer.Network.GameServer.OutgoingPacket;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.GameServer.IncomingPackets;

internal struct PingRequestPacket: IIncomingPacket<GameServerSession>
{
    private int _value;

    public void ReadContent(PacketBitReader reader)
    {
        _value = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection<GameServerSession> connection)
    {
        PingResponsePacket pingResponsePacket = new(_value);
        connection.Send(ref pingResponsePacket);
        return ValueTask.CompletedTask;
    }
}