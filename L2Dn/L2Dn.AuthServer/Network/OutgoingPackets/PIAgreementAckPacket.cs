using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.OutgoingPackets;

internal readonly struct PIAgreementAckPacket(int accountId, byte status): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PIAgreementAck);
        writer.WriteInt32(accountId);
        writer.WriteByte(status);
    }
}