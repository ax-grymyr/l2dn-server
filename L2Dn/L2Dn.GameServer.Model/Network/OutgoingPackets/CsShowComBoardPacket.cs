using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct CsShowComBoardPacket: IOutgoingPacket
{
    private readonly byte[] _html;
	
    public CsShowComBoardPacket(byte[] html)
    {
        _html = html;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.SHOW_BOARD);

        writer.WriteByte(1); // c4 1 to show community 00 to hide
        writer.WriteBytes(_html);
    }
}