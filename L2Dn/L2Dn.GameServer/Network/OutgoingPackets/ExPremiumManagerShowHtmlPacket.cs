using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExPremiumManagerShowHtmlPacket: IOutgoingPacket
{
    private readonly string _html;

    public ExPremiumManagerShowHtmlPacket(string html)
    {
        _html = html;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PREMIUM_MANAGER_SHOW_HTML);
        
        writer.WriteInt32(0); // getNpcObjId()
        writer.WriteString(_html);
        writer.WriteInt32(-1);
        writer.WriteInt32(0);
    }
}