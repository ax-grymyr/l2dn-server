using L2Dn.Network;

namespace L2Dn.Packets;

public interface IIncomingPacket<TSession>
    where TSession: ISession
{
    void ReadContent(PacketBitReader reader);
    ValueTask ProcessAsync(Connection<TSession> connection);
}