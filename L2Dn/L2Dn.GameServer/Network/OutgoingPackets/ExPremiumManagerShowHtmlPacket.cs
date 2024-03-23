using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Html;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExPremiumManagerShowHtmlPacket: IOutgoingPacket
{
    private readonly HtmlContent _html;

    public ExPremiumManagerShowHtmlPacket(HtmlContent html)
    {
        _html = html;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PREMIUM_MANAGER_SHOW_HTML);
        
        writer.WriteInt32(0); // npc object id
        writer.WriteString(_html.BuildHtml(HtmlActionScope.PREMIUM_HTML));
        writer.WriteInt32(-1);
        writer.WriteInt32(0);
    }
}