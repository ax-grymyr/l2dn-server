using L2Dn.Packets;

namespace L2Dn.AuthServer.Network.Client.OutgoingPackets;

internal readonly struct PIAgreementCheckPacket(int accountId, bool showAgreement): IOutgoingPacket
{
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WriteByte(0x11); // packet code
        writer.WriteInt32(accountId);
        writer.WriteBoolean(showAgreement);
    }
}
