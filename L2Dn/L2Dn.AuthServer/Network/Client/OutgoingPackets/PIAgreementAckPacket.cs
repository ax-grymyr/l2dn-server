using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.Client.OutgoingPackets;

internal readonly struct PIAgreementAckPacket(int accountId, byte status): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(0x12); // packet code
        writer.WriteInt32(accountId);
        writer.WriteByte(status);
    }
}