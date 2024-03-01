using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Quests;
using L2Dn.GameServer.Model.Quests.NewQuestData;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Quests;

public readonly struct ExQuestAcceptableListPacket: IOutgoingPacket
{
    private readonly Player _player;
	
    public ExQuestAcceptableListPacket(Player player)
    {
        _player = player;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_QUEST_ACCEPTABLE_LIST);

        List<Quest> availableQuests = new List<Quest>();
        ICollection<NewQuest> newQuests = NewQuestData.getInstance().getQuests();
        QuestManager questManager = QuestManager.getInstance();
		
        foreach (NewQuest newQuest in newQuests)
        {
            Quest quest = questManager.getQuest(newQuest.getId());
            if ((quest != null) && quest.canStartQuest(_player))
            {
                QuestState questState = _player.getQuestState(quest.getName());
                if (questState == null)
                {
                    availableQuests.Add(quest);
                }
            }
        }
		
        writer.WriteInt32(availableQuests.Count);
        foreach (Quest quest in availableQuests)
        {
            writer.WriteInt32(quest.getId());
        }
    }
}