using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct TutorialCloseHtmlPacket: IOutgoingPacket
{
    public static readonly TutorialCloseHtmlPacket STATIC_PACKET = new();

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.TUTORIAL_CLOSE_HTML);
    }
}