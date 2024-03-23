using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Html;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct TutorialShowHtmlPacket: IOutgoingPacket
{
    // TODO: Enum
    public const int NORMAL_WINDOW = 1;
    public const int LARGE_WINDOW = 2;
	
    private readonly int _type;
    private readonly HtmlContent _html;
	
    public TutorialShowHtmlPacket(HtmlContent html, int type = NORMAL_WINDOW)
    {
        _type = type;
        _html = html;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.TUTORIAL_SHOW_HTML);
        
        writer.WriteInt32(_type);
        writer.WriteString(_html.BuildHtml(HtmlActionScope.TUTORIAL_HTML));
    }
}