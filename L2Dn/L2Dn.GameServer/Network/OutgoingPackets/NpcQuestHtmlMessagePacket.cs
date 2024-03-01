using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct NpcQuestHtmlMessagePacket: IOutgoingPacket
{
    private readonly HtmlPacketHelper? _helper;
    private readonly int _questId;
    private readonly int _npcObjectId;
	
    public NpcQuestHtmlMessagePacket(int npcObjId, int questId, HtmlPacketHelper helper)
    {
        _questId = questId;
        _npcObjectId = npcObjId;
        _helper = helper;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_NPC_QUEST_HTML_MESSAGE);

        writer.WriteInt32(_npcObjectId);
        if (_helper is not null)
            _helper.WriteHtml(writer);
        else
            writer.WriteString(HtmlPacketHelper.MissingHtml);
        
        writer.WriteInt32(_questId);
    }
}