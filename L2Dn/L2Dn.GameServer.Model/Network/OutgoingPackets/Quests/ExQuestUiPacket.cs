using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Quests;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Quests;

public readonly struct ExQuestUiPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly ICollection<QuestState> _allQuests;
	
    public ExQuestUiPacket(Player player)
    {
        _player = player;
        _allQuests = player.getAllQuestStates();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        if (_player == null)
        {
            return;
        }
		
        writer.WritePacketCode(OutgoingPacketCodes.EX_QUEST_UI);
        
        if (!_allQuests.isEmpty())
        {
            List<QuestState> activeQuests = new List<QuestState>();
            foreach (QuestState qs in _allQuests)
            {
                if (qs.isStarted() && !qs.isCompleted())
                {
                    activeQuests.add(qs);
                }
            }
			
            writer.WriteInt32(_allQuests.Count);
            foreach (QuestState qs in _allQuests)
            {
                writer.WriteInt32(qs.getQuest().getId());
                writer.WriteInt32(qs.getCount());
                writer.WriteByte(qs.getState());
            }

            writer.WriteInt32(activeQuests.size());
        }
        else
        {
            writer.WriteInt32(0);
            writer.WriteInt32(0);
        }
    }
}