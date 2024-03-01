using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Quests;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct QuestListPacket: IOutgoingPacket
{
    private readonly List<QuestState> _activeQuests;
    private readonly byte[] _oneTimeQuestMask;
	
    public QuestListPacket(Player player)
    {
        _activeQuests = new List<QuestState>();
        _oneTimeQuestMask = new byte[128];
        foreach (QuestState qs in player.getAllQuestStates())
        {
            int questId = qs.getQuest().getId();
            if (questId > 0)
            {
                if (qs.isStarted())
                {
                    _activeQuests.Add(qs);
                }
                else if (qs.isCompleted() && !(((questId > 255) && (questId < 10256)) || (questId > 11023)))
                {
                    _oneTimeQuestMask[(questId % 10000) / 8] |= (byte)(1 << (questId % 8));
                }
            }
        }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.QUEST_LIST);
        
        writer.WriteInt16((short)_activeQuests.Count);
        foreach (QuestState qs in _activeQuests)
        {
            writer.WriteInt32(qs.getQuest().getId());
            writer.WriteInt32(qs.getCondBitSet());
        }
        
        writer.WriteBytes(_oneTimeQuestMask);
    }
}