using L2Dn.Network;

namespace L2Dn.Packets;

public interface IIncomingPacket<in TSession>
{
    void ReadContent(PacketBitReader reader);
    ValueTask ProcessAsync(Connection connection, TSession session);
}