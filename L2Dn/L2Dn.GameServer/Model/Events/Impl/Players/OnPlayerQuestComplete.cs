using L2Dn.Events;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;

namespace L2Dn.GameServer.Model.Events.Impl.Players;

/**
 * @author UnAfraid
 */
public class OnPlayerQuestComplete: EventBase
{
	private readonly Player _player;
	private readonly int _questId;
	private readonly QuestType _questType;
	
	public OnPlayerQuestComplete(Player player, int questId, QuestType questType)
	{
		_player = player;
		_questId = questId;
		_questType = questType;
	}
	
	public Player getPlayer()
	{
		return _player;
	}
	
	public int getQuestId()
	{
		return _questId;
	}
	
	public QuestType getQuestType()
	{
		return _questType;
	}
}