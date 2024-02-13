using L2Dn.AuthServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.IncomingPackets;

internal struct RequestPIAgreementCheckPacket: IIncomingPacket<AuthSession>
{
    private int _accountId;

    public void ReadContent(PacketBitReader reader)
    {
        _accountId = reader.ReadInt32();
        // 3 bytes - padding0
        // 4 bytes - checksum
        // 12 bytes - padding1
    }

    public ValueTask ProcessAsync(Connection connection, AuthSession session)
    {
        const bool showAgreement = false;
        PIAgreementCheckPacket piAgreementCheckPacket = new(_accountId, showAgreement);
        connection.Send(ref piAgreementCheckPacket);
        return ValueTask.CompletedTask;
    }
}
