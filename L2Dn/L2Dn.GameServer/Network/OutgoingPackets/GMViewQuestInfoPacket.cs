using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Quests;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct GMViewQuestInfoPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly ICollection<Quest> _questList;
	
    public GMViewQuestInfoPacket(Player player)
    {
        _player = player;
        _questList = player.getAllActiveQuests();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.GM_VIEW_QUEST_INFO);
        
        writer.WriteString(_player.getName());
        writer.WriteInt16((short)_questList.Count); // quest count
        foreach (Quest quest in _questList)
        {
            QuestState qs = _player.getQuestState(quest.getName());
            writer.WriteInt32(quest.getId());
            writer.WriteInt32(qs == null ? 0 : (int)qs.getCond());
        }
        
        writer.WriteInt16(0); // some size
        // for size; ddQQ
    }
}