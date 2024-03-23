using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Html;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct NpcQuestHtmlMessagePacket: IOutgoingPacket
{
    private readonly int _questId;
    private readonly int? _npcObjectId;
    private readonly HtmlContent _html;
	
    public NpcQuestHtmlMessagePacket(int? npcObjId, int questId, HtmlContent html)
    {
        _questId = questId;
        _npcObjectId = npcObjId;
        _html = html;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_NPC_QUEST_HTML_MESSAGE);

        writer.WriteInt32(_npcObjectId ?? 0);
        writer.WriteString(_html.BuildHtml(HtmlActionScope.NPC_QUEST_HTML, _npcObjectId));
        writer.WriteInt32(_questId);
    }
}