using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Quests;
using L2Dn.GameServer.Network.OutgoingPackets.ClassChange;
using L2Dn.Model;
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
	    // TODO: for now, check only the level.
	    // in the future, level and quest requirements must be in the data files
	    // (for example in ClassTree.xml)
	    // TODO: also in ExRequestClassChangePacket
	    return player.getLevel() >= 20;
	    int questId;
	    if (player.isDeathKnight())
		    questId = 10101;
	    else if (player.isAssassin())
		    questId = 10123;
	    else
		    questId = player.getRace() switch
		    {
			    Race.HUMAN => player.getClassId() == CharacterClass.FIGHTER ? 10009 : 10020,
			    Race.ELF => 10033,
			    Race.DARK_ELF => 10046,
			    Race.ORC => 10057,
			    Race.DWARF => 10079,
			    Race.KAMAEL => 10090,
			    Race.SYLPH => 10112,
			    _ => 0
		    };

	    return questId > 0 && IsQuestCompleted(player, questId);
    }

    private static bool secondClassCheck(Player player)
	{
		// SecondClassChange.java has only level check.
		return player.getLevel() >= 40;
	}

	private static bool thirdClassCheck(Player player) => IsQuestCompleted(player, 19900);

	private static bool IsQuestCompleted(Player player, int questId)
	{
		Quest quest = QuestManager.getInstance().getQuest(questId);
		if (quest is null)
			return false;
		
		QuestState qs = player.getQuestState(quest.getName());
		return qs != null && qs.isCompleted();
	}
}