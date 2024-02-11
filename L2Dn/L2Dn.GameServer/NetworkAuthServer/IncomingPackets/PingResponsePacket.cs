using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.NetworkAuthServer.IncomingPackets;

internal struct PingResponsePacket: IIncomingPacket<AuthServerSession>
{
    private int _value;

    public void ReadContent(PacketBitReader reader)
    {
        _value = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection<AuthServerSession> connection)
    {
        throw new NotImplementedException();
    }
}