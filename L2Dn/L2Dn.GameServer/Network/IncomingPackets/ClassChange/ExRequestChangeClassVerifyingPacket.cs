using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Quests;
using L2Dn.GameServer.Network.OutgoingPackets.ClassChange;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.ClassChange;

public struct ExRequestChangeClassVerifyingPacket: IIncomingPacket<GameSession>
{
    private CharacterClass _classId;

    public void ReadContent(PacketBitReader reader)
    {
        _classId = (CharacterClass)reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
		Player? player = session.Player;
		if (player == null)
			return ValueTask.CompletedTask;
		
		if (_classId != player.getClassId())
			return ValueTask.CompletedTask;
		
		if (player.isInCategory(CategoryType.FOURTH_CLASS_GROUP))
			return ValueTask.CompletedTask;
		
		if (player.isInCategory(CategoryType.THIRD_CLASS_GROUP))
		{
			if (!thirdClassCheck(player))
				return ValueTask.CompletedTask;
		}
		else if (player.isInCategory(CategoryType.SECOND_CLASS_GROUP))
		{
			if (!secondClassCheck(player))
				return ValueTask.CompletedTask;
		}
		else if (player.isInCategory(CategoryType.FIRST_CLASS_GROUP))
		{
			if (!firstClassCheck(player))
				return ValueTask.CompletedTask;
		}
		
		connection.Send(ExClassChangeSetAlarmPacket.STATIC_PACKET);
		return ValueTask.CompletedTask;
	}
	
	private static bool firstClassCheck(Player player)
	{
		QuestState qs = null;
		if (player.isDeathKnight())
		{
			Quest quest = QuestManager.getInstance().getQuest(10101);
			qs = player.getQuestState(quest.getName());
		}
		else if (player.isAssassin())
		{
			Quest quest = QuestManager.getInstance().getQuest(10123);
			qs = player.getQuestState(quest.getName());
		}
		else
		{
			switch (player.getRace())
			{
				case Race.HUMAN:
				{
					if (player.getClassId() == CharacterClass.FIGHTER)
					{
						Quest quest = QuestManager.getInstance().getQuest(10009);
						qs = player.getQuestState(quest.getName());
					}
					else
					{
						Quest quest = QuestManager.getInstance().getQuest(10020);
						qs = player.getQuestState(quest.getName());
					}
					break;
				}
				case Race.ELF:
				{
					Quest quest = QuestManager.getInstance().getQuest(10033);
					qs = player.getQuestState(quest.getName());
					break;
				}
				case Race.DARK_ELF:
				{
					Quest quest = QuestManager.getInstance().getQuest(10046);
					qs = player.getQuestState(quest.getName());
					break;
				}
				case Race.ORC:
				{
					Quest quest = QuestManager.getInstance().getQuest(10057);
					qs = player.getQuestState(quest.getName());
					break;
				}
				case Race.DWARF:
				{
					Quest quest = QuestManager.getInstance().getQuest(10079);
					qs = player.getQuestState(quest.getName());
					break;
				}
				case Race.KAMAEL:
				{
					Quest quest = QuestManager.getInstance().getQuest(10090);
					qs = player.getQuestState(quest.getName());
					break;
				}
				case Race.SYLPH:
				{
					Quest quest = QuestManager.getInstance().getQuest(10112);
					qs = player.getQuestState(quest.getName());
					break;
				}
			}
		}
		
		return (qs != null) && qs.isCompleted();
	}
	
	private static bool secondClassCheck(Player player)
	{
		// SecondClassChange.java has only level check.
		return player.getLevel() >= 40;
	}
	
	private static bool thirdClassCheck(Player player)
	{
		Quest quest = QuestManager.getInstance().getQuest(19900);
		QuestState qs = player.getQuestState(quest.getName());
		return (qs != null) && qs.isCompleted();
	}
}