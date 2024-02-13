using L2Dn.AuthServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.IncomingPackets;

internal struct RequestPIAgreementPacket: IIncomingPacket<AuthSession>
{
    private int _accountId;
    private byte _status;

    public void ReadContent(PacketBitReader reader)
    {
        _accountId = reader.ReadInt32();
        _status = reader.ReadByte();
    }

    public ValueTask ProcessAsync(Connection connection, AuthSession session)
    {
        PIAgreementAckPacket piAgreementAckPacket = new(_accountId, _status);
        connection.Send(ref piAgreementAckPacket);
        return ValueTask.CompletedTask;
    }
}
