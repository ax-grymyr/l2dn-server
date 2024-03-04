using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct TutorialShowHtmlPacket: IOutgoingPacket
{
    // TODO: Enum
    public const int NORMAL_WINDOW = 1;
    public const int LARGE_WINDOW = 2;
	
    private readonly int _type;
    private readonly string _html;
	
    public TutorialShowHtmlPacket(String html)
    {
        _type = NORMAL_WINDOW;
        _html = html;
    }
	
    public TutorialShowHtmlPacket(string html, int type)
    {
        _type = type;
        _html = html;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.TUTORIAL_SHOW_HTML);
        
        writer.WriteInt32(_type);
        writer.WriteString(_html);
    }
}