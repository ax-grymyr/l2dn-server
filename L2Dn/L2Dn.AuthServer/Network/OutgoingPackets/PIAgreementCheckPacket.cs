using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.OutgoingPackets;

internal readonly struct PIAgreementCheckPacket(int accountId, bool showAgreement): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PIAgreementCheck);
        writer.WriteInt32(accountId);
        writer.WriteBoolean(showAgreement);
    }
}
