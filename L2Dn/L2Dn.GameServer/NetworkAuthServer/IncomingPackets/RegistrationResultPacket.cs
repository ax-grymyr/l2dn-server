using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.NetworkAuthServer.IncomingPackets;

internal struct RegistrationResultPacket: IIncomingPacket<AuthServerSession>
{
    private RegistrationResult _result;

    public void ReadContent(PacketBitReader reader)
    {
        _result = reader.ReadEnum<RegistrationResult>();
    }

    public ValueTask ProcessAsync(Connection connection, AuthServerSession session)
    {
        throw new NotImplementedException();
    }
}